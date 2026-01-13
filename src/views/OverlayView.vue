<template>
    <div class="overlay-container">
        <VisualizationArea :key="componentKey" :heroStats="heroStats" :bestWave="bestWave" :dps="calculatedDPS"
            :layoutMode="layoutMode" :showDamageNumbers="showDamageNumbers" :showHUD="showHUD" :isLive="isLive"
            :twitchId="twitchId" :heroSkin="heroSkin" :initialWave="currentWave" :initialMonsterHp="monsterCurrentHp"
            :monsterConfig="monsterConfig" @monsterDefeated="onMonsterDefeated" @buffApplied="onBuffApplied"
            @stateChanged="onStateChanged" />
    </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onUnmounted } from 'vue'
import { useRoute } from 'vue-router'
import VisualizationArea from '../components/VisualizationArea.vue'
import { configApi, streamApi, userConfigApi, statsApi, userApi } from '../services/api'

const route = useRoute()

// Get twitchId from query param
const twitchId = ref(route.query.id || null)
const isLive = ref(false) // Auto-detected from Twitch API
const streamStatusInterval = ref(null)
const configRefreshInterval = ref(null)

// Key to force component remount on reset
const componentKey = ref(0)
const lastBackendWave = ref(null) // Track last known backend wave

// Display config (fetched from backend, synced with dashboard)
const layoutMode = ref(route.query.layout || 'full')
const showDamageNumbers = ref(true)
const showHUD = ref(true)
const heroSkin = ref('hero_1')

// Base stats (loaded from backend)
const heroStats = reactive({
    atk: 10,
    spd: 1.0,
    critChance: 5,
    critDamage: 150
})

const currentWave = ref(1)
const bestWave = ref(1)
const monsterCurrentHp = ref(0) // 0 = spawn with full HP
const lastModified = ref(null) // Track backend timestamp for stale-write prevention
const userId = ref(null)
const monsterConfig = ref({ baseHp: 50, hpMultiplier: 1.5 })


const calculatedDPS = computed(() => {
    const baseDPS = heroStats.atk * heroStats.spd
    const critBonus = (heroStats.critChance / 100) * (heroStats.critDamage / 100 - 1) * baseDPS
    return (baseDPS + critBonus).toFixed(1)
})

const onMonsterDefeated = async (wave) => {
    // OVERLAY IS MASTER - calls recordDefeat to save progress
    // The backend will increment the wave and return the new value

    if (userId.value) {
        try {
            const res = await userApi.recordDefeat(userId.value)
            // Use the wave from backend response (source of truth)
            currentWave.value = res.data.currentWave
            if (res.data.currentWave > bestWave.value) {
                bestWave.value = res.data.currentWave
            }
            // Update lastModified to prevent future saves from being rejected as stale
            if (res.data?.updatedAt) {
                lastModified.value = res.data.updatedAt
            }
            console.log('Overlay: Monster defeated, wave saved:', res.data.currentWave)
        } catch (err) {
            console.warn('Failed to save progress:', err.message)
        }
    }
}

// Handle buffs from SignalR (real Twitch events)
const onBuffApplied = (buff) => {
    console.log('Overlay received buff:', buff)

    switch (buff.type) {
        case 'atk':
            heroStats.atk += buff.value
            break
        case 'spd':
            heroStats.spd = Math.min(10, heroStats.spd + buff.value)
            break
        case 'crit_chance':
            heroStats.critChance = Math.min(100, heroStats.critChance + buff.value)
            break
        case 'crit_damage':
            heroStats.critDamage += buff.value
            break
    }
}

// Save monster state to backend (called every 30 seconds by VisualizationArea)
const onStateChanged = async (state) => {
    if (!userId.value) return

    try {
        const res = await userApi.saveMonsterState(
            userId.value,
            state.monsterHp,  // Only save HP - wave is managed by recordDefeat
            lastModified.value
        )

        if (res.data.saved) {
            lastModified.value = res.data.updatedAt
            console.log(`Saved monster HP: ${state.monsterHp}`)
        } else {
            console.log(`Save rejected (stale): local timestamp older than backend`)
        }
    } catch (err) {
        console.warn('Failed to save monster state:', err.message)
    }
}

// Check if Twitch stream is live
const checkStreamStatus = async () => {
    if (!twitchId.value) return

    try {
        const res = await streamApi.getStatus(twitchId.value)
        isLive.value = res.data.isLive
        console.log('Overlay stream status:', res.data.isLive ? 'LIVE' : 'OFFLINE')
    } catch (err) {
        console.warn('Failed to check stream status:', err.message)
    }
}

// Fetch display config from backend (synced with dashboard)
const fetchUserConfig = async () => {
    if (!twitchId.value) return

    try {
        const res = await userConfigApi.get(twitchId.value)
        layoutMode.value = res.data.layoutMode || 'full'
        showDamageNumbers.value = res.data.showDamageNumbers !== false
        showHUD.value = res.data.showHUD !== false
        heroSkin.value = res.data.heroSkin || 'hero_1'
    } catch (err) {
        console.warn('Failed to load user config:', err.message)
    }
}

// Fetch user stats and progress from backend (syncs with dashboard reset)
const fetchUserStats = async () => {
    if (!twitchId.value) return

    try {
        // Get userId if we don't have it yet - use Twitch ID lookup endpoint
        if (!userId.value) {
            const userRes = await userApi.getUserByTwitchId(twitchId.value)
            if (userRes.data?.id) {
                userId.value = userRes.data.id
            }
        }

        if (userId.value) {
            // Load progress to detect reset
            const progressRes = await userApi.getProgress(userId.value)
            if (progressRes.data) {
                const backendWave = progressRes.data.currentWave || 1

                // On first load, sync everything
                if (lastBackendWave.value === null) {
                    currentWave.value = backendWave  // ✅ Only sync on first load
                    lastBackendWave.value = backendWave
                    monsterCurrentHp.value = progressRes.data.monsterCurrentHp || 0
                    lastModified.value = progressRes.data.updatedAt || null
                    console.log(`Initial state: Wave ${backendWave}, Saved HP ${monsterCurrentHp.value}`)
                }
                // Detect reset: if backend wave decreased, FORCE PAGE RELOAD
                else if (backendWave < lastBackendWave.value) {
                    console.log(`RESET DETECTED! Backend wave ${lastBackendWave.value} → ${backendWave}. RELOADING PAGE...`)
                    window.location.reload()
                    return
                }
                // ✅ DON'T overwrite currentWave - let the game control it
                else {
                    lastBackendWave.value = backendWave
                }

                // Update best wave from backend (this one should always sync)
                bestWave.value = progressRes.data.bestWave || 1
            }

            // Load and sync stats from backend
            const statsRes = await statsApi.getStats(userId.value)
            if (statsRes.data) {
                heroStats.atk = statsRes.data.atk || 10
                heroStats.spd = Number(statsRes.data.spd) || 1.0
                heroStats.critChance = statsRes.data.critChance || 5
                heroStats.critDamage = statsRes.data.critDamage || 150
            }
        }
    } catch (err) {
        console.warn('Failed to load user stats:', err.message)
    }
}

// Load config on mount
onMounted(async () => {
    if (!twitchId.value) {
        console.error('No twitchId provided in URL. Use /overlay?id=YOUR_TWITCH_ID')
        return
    }

    console.log('Overlay initialized for twitchId:', twitchId.value)

    // Fetch user display config (auto-sync with dashboard)
    await fetchUserConfig()

    // Fetch game config
    try {
        const { data } = await configApi.getGameConfig()

        if (data.Hero) {
            heroStats.atk = data.Hero.BaseAtk || 10
            heroStats.spd = data.Hero.BaseSpd || 1.0
            heroStats.critChance = data.Hero.BaseCritChance || 5
            heroStats.critDamage = data.Hero.BaseCritDamage || 150
        }

        if (data.Monster) {
            monsterConfig.value = {
                baseHp: data.Monster.BaseHp || 50,
                hpMultiplier: data.Monster.HpMultiplierPerWave || 1.5
            }
        }

        console.log('Overlay config loaded')
    } catch (err) {
        console.warn('Failed to load config:', err.message)
    }

    // Load initial user stats
    await fetchUserStats()

    // Start stream status polling
    checkStreamStatus()
    streamStatusInterval.value = setInterval(checkStreamStatus, 30000) // Check every 30 seconds

    // Start config + stats refresh polling (sync with dashboard changes including reset)
    configRefreshInterval.value = setInterval(async () => {
        await fetchUserConfig()
        await fetchUserStats()
    }, 10000) // Refresh every 10 seconds
})

// Cleanup on unmount
onUnmounted(() => {
    if (streamStatusInterval.value) {
        clearInterval(streamStatusInterval.value)
    }
    if (configRefreshInterval.value) {
        clearInterval(configRefreshInterval.value)
    }
})
</script>

<style scoped>
.overlay-container {
    width: 100vw;
    height: 100vh;
    background: transparent;
    overflow: hidden;
}
</style>
