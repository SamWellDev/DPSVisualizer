<template>
    <div class="overlay-container">
        <VisualizationArea :heroStats="heroStats" :bestWave="bestWave" :dps="calculatedDPS" :layoutMode="layoutMode"
            :showDamageNumbers="true" :isLive="true" :twitchId="twitchId" @monsterDefeated="onMonsterDefeated"
            @buffApplied="onBuffApplied" />
    </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import VisualizationArea from '../components/VisualizationArea.vue'
import { configApi } from '../services/api'

const route = useRoute()

// Get twitchId from query param
const twitchId = ref(route.query.id || null)
const layoutMode = ref(route.query.layout || 'full') // 'full' or 'monster'

// Base stats (loaded from backend config)
const heroStats = reactive({
    atk: 10,
    spd: 1.0,
    critChance: 5,
    critDamage: 150
})

const currentWave = ref(1)
const bestWave = ref(1)

const calculatedDPS = computed(() => {
    const baseDPS = heroStats.atk * heroStats.spd
    const critBonus = (heroStats.critChance / 100) * (heroStats.critDamage / 100 - 1) * baseDPS
    return (baseDPS + critBonus).toFixed(1)
})

const onMonsterDefeated = (wave) => {
    currentWave.value = wave
    if (wave > bestWave.value) {
        bestWave.value = wave
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

// Load config on mount
onMounted(async () => {
    if (!twitchId.value) {
        console.error('No twitchId provided in URL. Use /overlay?id=YOUR_TWITCH_ID')
        return
    }

    console.log('Overlay initialized for twitchId:', twitchId.value)

    // Fetch game config
    try {
        const { data } = await configApi.getGameConfig()

        if (data.Hero) {
            heroStats.atk = data.Hero.BaseAtk || 10
            heroStats.spd = data.Hero.BaseSpd || 1.0
            heroStats.critChance = data.Hero.BaseCritChance || 5
            heroStats.critDamage = data.Hero.BaseCritDamage || 150
        }

        console.log('Overlay config loaded')
    } catch (err) {
        console.warn('Failed to load config:', err.message)
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
