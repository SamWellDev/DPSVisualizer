<template>
    <div class="min-h-screen bg-gray-900 flex flex-col">
        <!-- Header -->
        <AppHeader :wave="currentWave" :dps="calculatedDPS" @openShop="openShop" @logout="logout" @exportOBS="exportOBS"
            @reset="showResetModal = true" />

        <!-- Main Content -->
        <main class="container mx-auto px-6 py-6 flex-1">
            <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">

                <!-- Visualization Area (2/3) -->
                <div class="lg:col-span-2">
                    <div class="bg-gray-800 rounded-xl border border-gray-700 overflow-hidden" style="height: 500px;">
                        <!-- Config Tab: Show status panel instead of game -->
                        <div v-if="activeTab === 'config'"
                            class="h-full flex flex-col items-center justify-center p-8 text-center">
                            <div class="text-6xl mb-4">🎮</div>
                            <h2 class="text-2xl font-bold text-white mb-2">Game Status</h2>
                            <p class="text-gray-400 mb-6">The real game runs in the Overlay (OBS)</p>

                            <div class="grid grid-cols-2 gap-4 w-full max-w-md">
                                <div class="bg-gray-900 rounded-lg p-4 border border-gray-700">
                                    <div class="text-gray-400 text-sm">Current Wave</div>
                                    <div class="text-3xl font-bold text-cyan-400">{{ currentWave }}</div>
                                </div>
                                <div class="bg-gray-900 rounded-lg p-4 border border-gray-700">
                                    <div class="text-gray-400 text-sm">Best Wave</div>
                                    <div class="text-3xl font-bold text-yellow-400">{{ bestWave }}</div>
                                </div>
                            </div>

                            <div class="mt-6 grid grid-cols-4 gap-3 w-full max-w-lg">
                                <div class="bg-gray-900 rounded-lg p-3 border border-gray-700">
                                    <div class="text-cyan-400 text-xs">ATK</div>
                                    <div class="text-xl font-bold text-white">{{ liveStats.atk }}</div>
                                </div>
                                <div class="bg-gray-900 rounded-lg p-3 border border-gray-700">
                                    <div class="text-green-400 text-xs">SPD</div>
                                    <div class="text-xl font-bold text-white">{{ liveStats.spd.toFixed(1) }}</div>
                                </div>
                                <div class="bg-gray-900 rounded-lg p-3 border border-gray-700">
                                    <div class="text-yellow-400 text-xs">CRIT%</div>
                                    <div class="text-xl font-bold text-white">{{ liveStats.critChance }}%</div>
                                </div>
                                <div class="bg-gray-900 rounded-lg p-3 border border-gray-700">
                                    <div class="text-orange-400 text-xs">CRIT DMG</div>
                                    <div class="text-xl font-bold text-white">{{ liveStats.critDamage }}%</div>
                                </div>
                            </div>

                            <p class="mt-6 text-gray-500 text-sm flex items-center gap-2">
                                <span class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></span>
                                Auto-sync every 10s
                            </p>
                        </div>

                        <!-- Test Tab: Show game with testMode (Dummy) -->
                        <VisualizationArea v-else :key="activeTab" :heroStats="testStats" :bestWave="bestWave"
                            :dps="calculatedDPS" :layoutMode="layoutMode" :showDamageNumbers="showDamageNumbers"
                            :showHUD="showHUD" :isLive="true" :testMode="true" :monsterConfig="monsterConfig" />
                    </div>

                    <!-- Stats Row below Visualization -->
                    <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mt-4">
                        <EventCounters :counts="eventCounts" periodLabel="This month" />
                        <MonthlyProgress :currentWave="currentWave" :records="monthlyRecords" />
                        <GlobalRankings :rankings="globalRankings" :currentUserRank="currentUserRank" />
                    </div>
                </div>

                <!-- Controls Panel (1/3) -->
                <div class="bg-gray-800 rounded-xl border border-gray-700 p-6">

                    <!-- Tab Headers -->
                    <div class="flex mb-4 bg-gray-900 rounded-lg p-1">
                        <button @click="activeTab = 'config'" :class="[
                            'flex-1 py-2 px-4 rounded-md text-sm font-medium transition-colors',
                            activeTab === 'config'
                                ? 'bg-purple-600 text-white'
                                : 'text-gray-400 hover:text-white'
                        ]">
                            ⚙️ Config
                        </button>
                        <button @click="switchToTest" :class="[
                            'flex-1 py-2 px-4 rounded-md text-sm font-medium transition-colors',
                            activeTab === 'test'
                                ? 'bg-green-600 text-white'
                                : 'text-gray-400 hover:text-white'
                        ]">
                            🧪 Test
                        </button>
                    </div>

                    <!-- CONFIG TAB -->
                    <div v-if="activeTab === 'config'">
                        <p class="text-gray-400 text-sm mb-4">
                            Configure buff values for each event
                        </p>

                        <!-- Layout Mode Toggle -->
                        <div class="bg-gray-900 rounded-lg p-4 border border-gray-700 mb-4">
                            <div class="flex items-center justify-between mb-2">
                                <span class="text-white font-medium">📺 Layout Mode</span>
                            </div>
                            <div class="flex gap-2">
                                <button @click="layoutMode = 'full'" :class="[
                                    'flex-1 py-2 px-3 rounded-lg text-sm font-medium transition-colors',
                                    layoutMode === 'full'
                                        ? 'bg-purple-600 text-white'
                                        : 'bg-gray-800 text-gray-400 hover:text-white'
                                ]">
                                    Full Layout
                                </button>
                                <button @click="layoutMode = 'monster'" :class="[
                                    'flex-1 py-2 px-3 rounded-lg text-sm font-medium transition-colors',
                                    layoutMode === 'monster'
                                        ? 'bg-purple-600 text-white'
                                        : 'bg-gray-800 text-gray-400 hover:text-white'
                                ]">
                                    Monster Only
                                </button>
                            </div>
                            <p class="text-xs text-gray-500 mt-2">Monster Only: Shows only monster HP bar for limited
                                stream space</p>
                        </div>

                        <!-- Show Damage Numbers Toggle -->
                        <div class="bg-gray-900 rounded-lg p-4 border border-gray-700 mb-4">
                            <label class="flex items-center justify-between cursor-pointer">
                                <span class="text-white font-medium">💫 Show Damage Numbers</span>
                                <input type="checkbox" v-model="showDamageNumbers"
                                    class="w-5 h-5 rounded bg-gray-800 border-gray-600 text-purple-600 focus:ring-purple-500 focus:ring-offset-gray-900" />
                            </label>
                        </div>

                        <!-- Show HUD Toggle -->
                        <div class="bg-gray-900 rounded-lg p-4 border border-gray-700 mb-4">
                            <label class="flex items-center justify-between cursor-pointer">
                                <span class="text-white font-medium">📊 Show HUD</span>
                                <input type="checkbox" v-model="showHUD"
                                    class="w-5 h-5 rounded bg-gray-800 border-gray-600 text-purple-600 focus:ring-purple-500 focus:ring-offset-gray-900" />
                            </label>
                            <p class="text-xs text-gray-500 mt-2">Stats, wave info, DPS display</p>
                        </div>

                        <!-- Stream Status Indicator -->
                        <div class="bg-gray-900 rounded-lg p-4 border border-gray-700 mb-4">
                            <div class="flex items-center justify-between">
                                <span class="text-white font-medium">📡 Stream Status</span>
                                <div class="flex items-center gap-2">
                                    <span v-if="isLive" class="w-2 h-2 bg-red-500 rounded-full animate-pulse"></span>
                                    <span v-else class="w-2 h-2 bg-gray-500 rounded-full"></span>
                                    <span :class="isLive ? 'text-red-400' : 'text-gray-400'">{{ isLive ? 'LIVE' :
                                        'Offline' }}</span>
                                </div>
                            </div>
                            <p class="text-xs text-gray-500 mt-2">Auto-detected from Twitch every 30s</p>
                        </div>

                        <!-- Save Button -->
                        <button @click="saveConfig"
                            class="w-full py-3 px-4 bg-gray-700 hover:bg-gray-600 text-white rounded-lg font-medium transition-colors">
                            💾 Save Configuration
                        </button>
                    </div>

                    <!-- TEST TAB -->
                    <div v-if="activeTab === 'test'">
                        <p class="text-gray-400 text-sm mb-4">
                            Simulate events to test (does not affect live)
                        </p>

                        <!-- Test Buttons -->
                        <div class="space-y-3 mb-6">
                            <button @click="simulateEvent('follow')"
                                class="w-full py-3 px-4 bg-purple-600 hover:bg-purple-500 text-white rounded-lg font-medium transition-colors flex items-center justify-between">
                                <span>👤 Follow</span>
                                <span class="text-purple-200 text-sm">+{{ buffConfig.follow.critChance }}% Crit</span>
                            </button>

                            <button @click="simulateEvent('sub1')"
                                class="w-full py-3 px-4 bg-blue-600 hover:bg-blue-500 text-white rounded-lg font-medium transition-colors flex items-center justify-between">
                                <span>⭐ Sub Tier 1</span>
                                <span class="text-blue-200 text-sm">+{{ buffConfig.sub1.spd }} SPD</span>
                            </button>

                            <button @click="simulateEvent('sub2')"
                                class="w-full py-3 px-4 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg font-medium transition-colors flex items-center justify-between">
                                <span>⭐⭐ Sub Tier 2</span>
                                <span class="text-indigo-200 text-sm">+{{ buffConfig.sub2.spd }} SPD</span>
                            </button>

                            <button @click="simulateEvent('sub3')"
                                class="w-full py-3 px-4 bg-pink-600 hover:bg-pink-500 text-white rounded-lg font-medium transition-colors flex items-center justify-between">
                                <span>⭐⭐⭐ Sub Tier 3</span>
                                <span class="text-pink-200 text-sm">+{{ buffConfig.sub3.spd }} SPD, +{{
                                    buffConfig.sub3.atk }} ATK</span>
                            </button>

                            <button @click="simulateEvent('donate5')"
                                class="w-full py-3 px-4 bg-green-600 hover:bg-green-500 text-white rounded-lg font-medium transition-colors flex items-center justify-between">
                                <span>💵 Donate $5</span>
                                <span class="text-green-200 text-sm">+{{ buffConfig.donate5.atk }} ATK</span>
                            </button>

                            <button @click="simulateEvent('donate10')"
                                class="w-full py-3 px-4 bg-yellow-600 hover:bg-yellow-500 text-white rounded-lg font-medium transition-colors flex items-center justify-between">
                                <span>💰 Donate $10+</span>
                                <span class="text-yellow-200 text-sm">+{{ buffConfig.donate10.atk }} ATK, +{{
                                    buffConfig.donate10.critDamage }}% Crit DMG</span>
                            </button>

                            <button @click="simulateEvent('bits')"
                                class="w-full py-3 px-4 bg-orange-600 hover:bg-orange-500 text-white rounded-lg font-medium transition-colors flex items-center justify-between">
                                <span>💎 100 Bits</span>
                                <span class="text-orange-200 text-sm">+{{ (buffConfig.bits.critDamage * 100).toFixed(0)
                                }}% Crit DMG</span>
                            </button>
                        </div>

                        <!-- Test Stats Display -->
                        <StatsDisplay :stats="testStats" variant="test" />

                        <!-- Reset Test Button -->
                        <button @click="resetTestStats"
                            class="w-full mt-4 py-2 px-4 bg-red-600 hover:bg-red-500 text-white rounded-lg font-medium transition-colors">
                            🔄 Reset Test
                        </button>
                    </div>
                </div>
            </div>

            <!-- Achievements Section (Full Width) -->
            <div class="mt-6">
                <Achievements :achievements="achievements" />
            </div>

            <!-- Support Banner -->
            <div class="mt-6 bg-gradient-to-r from-purple-600 to-pink-600 rounded-xl p-6 text-center">
                <h3 class="text-xl font-bold text-white mb-2">💜 Enjoying Twitch Fighter?</h3>
                <p class="text-purple-100 mb-4">Help keep this project alive and growing!</p>
                <div class="flex flex-wrap justify-center gap-4">
                    <a href="#"
                        class="inline-flex items-center gap-2 px-6 py-3 bg-yellow-400 hover:bg-yellow-300 text-gray-900 font-bold rounded-lg transition-colors">
                        ☕ Buy me a coffee
                    </a>
                    <a href="#"
                        class="inline-flex items-center gap-2 px-6 py-3 bg-indigo-500 hover:bg-indigo-400 text-white font-bold rounded-lg transition-colors">
                        💬 Chat on Discord
                    </a>
                </div>
            </div>
        </main>

        <!-- Footer -->
        <AppFooter />

        <!-- Reset Confirmation Modal -->
        <div v-if="showResetModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div class="bg-gray-800 rounded-xl border border-gray-700 p-6 max-w-md w-full mx-4">
                <h3 class="text-xl font-bold text-white mb-4">⚠️ Reset Progress</h3>
                <p class="text-gray-400 mb-6">
                    Are you sure you want to reset? This will:
                </p>
                <ul class="text-gray-300 mb-6 space-y-2">
                    <li>• Reset wave to 1</li>
                    <li>• Reset all hero stats to base values</li>
                    <li>• Clear all received buffs</li>
                </ul>
                <div class="flex gap-3">
                    <button @click="showResetModal = false"
                        class="flex-1 py-3 px-4 bg-gray-700 hover:bg-gray-600 text-white rounded-lg font-medium transition-colors">
                        Cancel
                    </button>
                    <button @click="confirmReset"
                        class="flex-1 py-3 px-4 bg-red-600 hover:bg-red-500 text-white rounded-lg font-medium transition-colors">
                        Reset
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { reactive, computed, ref, onMounted, onUnmounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import VisualizationArea from '../components/VisualizationArea.vue'
import AppHeader from '../components/AppHeader.vue'
import AppFooter from '../components/AppFooter.vue'
import StatsDisplay from '../components/StatsDisplay.vue'
import MonthlyProgress from '../components/MonthlyProgress.vue'
import EventCounters from '../components/EventCounters.vue'
import GlobalRankings from '../components/GlobalRankings.vue'
import Achievements from '../components/Achievements.vue'
import { configApi, userApi, statsApi, rankingsApi, streamApi, userConfigApi } from '../services/api'
import { useSignalR } from '../composables/signalr'

const router = useRouter()
const route = useRoute()

// SignalR for broadcasting game state to overlay
const { connect: connectSignalR, broadcastGameState, broadcastDamage, broadcastBuff, disconnect: disconnectSignalR } = useSignalR()

// User authentication state
const twitchId = ref(localStorage.getItem('twitch_id') || null)
const userId = ref(localStorage.getItem('user_id') || null)
const accessToken = ref(localStorage.getItem('access_token') || null)

const currentWave = ref(1)
const activeTab = ref('config')
const layoutMode = ref('full') // 'full' or 'monster'
const showDamageNumbers = ref(true)
const showHUD = ref(true) // Show stats, wave info, DPS panels
const showResetModal = ref(false)
const isLive = ref(false) // Will be auto-detected from Twitch
const streamStatusInterval = ref(null)

// Handle auth callback params on mount
onMounted(async () => {
    const urlParams = new URLSearchParams(window.location.search)
    const token = urlParams.get('token')
    const id = urlParams.get('userId')
    const twitch = urlParams.get('twitchId')

    if (token && id && twitch) {
        // Store auth data
        localStorage.setItem('access_token', token)
        localStorage.setItem('user_id', id)
        localStorage.setItem('twitch_id', twitch)
        accessToken.value = token
        userId.value = id
        twitchId.value = twitch

        // Clean URL
        window.history.replaceState({}, document.title, '/dashboard')

        console.log('Auth successful! Twitch ID:', twitch)
    }

    // Fetch game config from backend
    try {
        const { data } = await configApi.getGameConfig()
        gameConfig.value = data
        configLoaded.value = true

        // Update buff config with backend values
        if (data.Buffs) {
            buffConfig.follow.critChance = data.Buffs.Follow?.Value || 2
            buffConfig.sub1.spd = data.Buffs.SubTier1?.Value || 0.5
            buffConfig.sub2.spd = data.Buffs.SubTier2?.Value || 1.0
            buffConfig.sub3.spd = data.Buffs.SubTier3?.Value || 1.5
            buffConfig.sub3.atk = data.Buffs.SubTier3?.BonusAtk || 10
            buffConfig.bits.critDamage = data.Buffs.BitsPerHundred?.Value || 10
        }

        // Update base stats from config
        if (data.Hero) {
            baseStats.atk = data.Hero.BaseAtk || 10
            baseStats.spd = data.Hero.BaseSpd || 1.0
            baseStats.critChance = data.Hero.BaseCritChance || 5
            baseStats.critDamage = data.Hero.BaseCritDamage || 150

            // Also update live and test stats
            Object.assign(liveStats, baseStats)
            Object.assign(testStats, baseStats)
        }

        // Update monster config
        if (data.Monster) {
            monsterConfig.baseHp = data.Monster.BaseHp || 50
            monsterConfig.hpMultiplier = data.Monster.HpMultiplierPerWave || 1.5
        }

        console.log('Game config loaded:', data)
    } catch (err) {
        console.warn('Failed to load game config, using defaults:', err.message)
    }

    // Load user progress and stats from backend
    if (userId.value) {
        try {
            // Load progress (wave, bestWave)
            const progressRes = await userApi.getProgress(userId.value)
            currentWave.value = progressRes.data.currentWave || 1
            console.log('Progress loaded:', progressRes.data)
        } catch (err) {
            console.warn('Failed to load progress (new user?):', err.message)
        }

        try {
            // Load stats (ATK, SPD, Crit)
            const statsRes = await statsApi.getStats(userId.value)
            liveStats.atk = statsRes.data.atk || baseStats.atk
            liveStats.spd = Number(statsRes.data.spd) || baseStats.spd
            liveStats.critChance = statsRes.data.critChance || baseStats.critChance
            liveStats.critDamage = statsRes.data.critDamage || baseStats.critDamage

            // Load event counters
            if (statsRes.data.counters) {
                eventCounts.follows = statsRes.data.counters.follows || 0
                eventCounts.subs = statsRes.data.counters.subs || 0
                eventCounts.bits = statsRes.data.counters.bits || 0
                eventCounts.donations = statsRes.data.counters.donations || 0
            }
            console.log('Stats loaded:', statsRes.data)
        } catch (err) {
            console.warn('Failed to load stats (new user?):', err.message)
        }
    }

    // Start stream status polling
    if (twitchId.value) {
        checkStreamStatus()
        streamStatusInterval.value = setInterval(checkStreamStatus, 30000) // Check every 30 seconds
    }

    // Start auto-sync from backend (every 10 seconds)
    if (userId.value) {
        setInterval(syncFromBackend, 10000)
    }
})

// Check if Twitch stream is live
const checkStreamStatus = async () => {
    if (!twitchId.value) return

    try {
        const res = await streamApi.getStatus(twitchId.value)
        isLive.value = res.data.isLive
        console.log('Stream status:', res.data.isLive ? 'LIVE' : 'OFFLINE')
    } catch (err) {
        console.warn('Failed to check stream status:', err.message)
    }
}

// Cleanup on unmount
onUnmounted(() => {
    if (streamStatusInterval.value) {
        clearInterval(streamStatusInterval.value)
    }
})

// Save display config to backend (called when user clicks Save button)
const saveConfig = async () => {
    if (!twitchId.value) {
        alert('⚠️ Please login with Twitch first.')
        return
    }

    try {
        await userConfigApi.save(twitchId.value, {
            layoutMode: layoutMode.value,
            showDamageNumbers: showDamageNumbers.value,
            showHUD: showHUD.value
        })
        console.log('Display config saved to backend')
        alert('✅ Configuration saved! OBS overlay will update within 10 seconds.')
    } catch (err) {
        console.error('Failed to save display config:', err.message)
        alert('❌ Failed to save configuration')
    }
}

// Buff configuration (loaded from backend)
const buffConfig = reactive({
    follow: { critChance: 2 },
    sub1: { spd: 0.5 },
    sub2: { spd: 1.0 },
    sub3: { spd: 1.5, atk: 10 },
    donate5: { atk: 5 },
    donate10: { atk: 20, critDamage: 15 },
    bits: { critDamage: 10 } // per 100 bits
})

// Game config loaded from backend
const gameConfig = ref(null)
const configLoaded = ref(false)
const monsterConfig = reactive({ baseHp: 50, hpMultiplier: 1.5 })

// Event counters (for display)
const eventCounts = reactive({
    follows: 0,
    subs: 0,
    donations: 0,
    bits: 0
})

// Base stats (loaded from backend config)
const baseStats = reactive({
    atk: 10,
    spd: 1.0,
    critChance: 5,
    critDamage: 150
})

// LIVE stats (what's actually active)
const liveStats = reactive({ ...baseStats })

// TEST stats (sandbox, resets when switching tabs)
const testStats = reactive({ ...baseStats })

// Active stats (which one is being used in canvas)
const activeStats = computed(() => {
    return activeTab.value === 'config' ? liveStats : testStats
})

const calculatedDPS = computed(() => {
    const stats = activeStats.value
    const baseDPS = stats.atk * stats.spd
    const critBonus = (stats.critChance / 100) * (stats.critDamage / 100 - 1) * baseDPS
    return (baseDPS + critBonus).toFixed(1)
})

// Switch to test tab (resets test stats)
const switchToTest = () => {
    activeTab.value = 'test'
    resetTestStats()
}

// Simulate event (applies to TEST stats only, never live)
const simulateEvent = (eventType) => {
    // Always use testStats for simulations - never affect live!
    const stats = testStats

    switch (eventType) {
        case 'follow':
            stats.critChance = Math.min(100, stats.critChance + buffConfig.follow.critChance)
            break
        case 'sub1':
            stats.spd = Math.min(10, stats.spd + buffConfig.sub1.spd)
            break
        case 'sub2':
            stats.spd = Math.min(10, stats.spd + buffConfig.sub2.spd)
            break
        case 'sub3':
            stats.spd = Math.min(10, stats.spd + buffConfig.sub3.spd)
            stats.atk += buffConfig.sub3.atk
            break
        case 'donate5':
            stats.atk += buffConfig.donate5.atk
            break
        case 'donate10':
            stats.atk += buffConfig.donate10.atk
            stats.critDamage += buffConfig.donate10.critDamage
            break
        case 'bits':
            stats.critDamage += buffConfig.bits.critDamage * 100 // Simulate 100 bits
            break
    }
}

// Sync wave and stats from backend (auto-called every 10s)
const syncFromBackend = async () => {
    if (!userId.value) return

    try {
        // Load progress (wave, bestWave)
        const progressRes = await userApi.getProgress(userId.value)
        currentWave.value = progressRes.data.currentWave || 1

        // Load stats (ATK, SPD, Crit)
        const statsRes = await statsApi.getStats(userId.value)
        liveStats.atk = statsRes.data.atk || baseStats.atk
        liveStats.spd = Number(statsRes.data.spd) || baseStats.spd
        liveStats.critChance = statsRes.data.critChance || baseStats.critChance
        liveStats.critDamage = statsRes.data.critDamage || baseStats.critDamage

        // Load event counters
        if (statsRes.data.counters) {
            eventCounts.follows = statsRes.data.counters.follows || 0
            eventCounts.subs = statsRes.data.counters.subs || 0
            eventCounts.bits = statsRes.data.counters.bits || 0
            eventCounts.donations = statsRes.data.counters.donations || 0
        }
    } catch (err) {
        // Silent fail for auto-sync
        console.warn('Auto-sync failed:', err.message)
    }
}

// Handle buffs from SignalR (real Twitch events)
const onBuffApplied = async (buff) => {
    console.log('Buff received from SignalR:', buff)

    // Apply buff to LIVE stats (not test stats)
    switch (buff.type) {
        case 'atk':
            liveStats.atk += buff.value
            break
        case 'spd':
            liveStats.spd = Math.min(10, liveStats.spd + buff.value)
            break
        case 'crit_chance':
            liveStats.critChance = Math.min(100, liveStats.critChance + buff.value)
            break
        case 'crit_damage':
            liveStats.critDamage += buff.value
            break
    }

    // Update event counters
    if (buff.eventType === 'follow') {
        eventCounts.follows++
    } else if (buff.eventType === 'subscription') {
        eventCounts.subs++
    } else if (buff.eventType === 'bits') {
        eventCounts.bits += buff.value * 10 // Approximate bits from crit bonus
    }

    // Persist buff to backend
    if (userId.value) {
        try {
            await statsApi.applyBuff(userId.value, {
                eventType: buff.eventType,
                amount: buff.amount,
                username: buff.userName
            })
            console.log('Buff persisted to backend')
        } catch (err) {
            console.error('Failed to persist buff:', err.message)
        }
    }
}

const resetTestStats = () => {
    testStats.atk = baseStats.atk
    testStats.spd = baseStats.spd
    testStats.critChance = baseStats.critChance
    testStats.critDamage = baseStats.critDamage
}

// Note: saveConfig is defined above (line ~371) and saves display config to backend

const openShop = () => {
    router.push('/shop')
}

const exportOBS = async () => {
    if (!twitchId.value) {
        alert('⚠️ Please login with Twitch first to get your overlay URL.')
        return
    }

    const baseUrl = window.location.origin
    const overlayUrl = `${baseUrl}/overlay?id=${twitchId.value}&layout=${layoutMode.value}`

    try {
        await navigator.clipboard.writeText(overlayUrl)
        alert(`✅ URL copied to clipboard!\n\n${overlayUrl}\n\nAdd this as a Browser Source in OBS.\nRecommended size: 1920x1080`)
    } catch (err) {
        // Fallback for older browsers
        prompt('📋 Copy this URL to use in OBS:', overlayUrl)
    }
}

const confirmReset = async () => {
    // Reset wave
    currentWave.value = 1

    // Reset live stats to base values
    liveStats.atk = baseStats.atk
    liveStats.spd = baseStats.spd
    liveStats.critChance = baseStats.critChance
    liveStats.critDamage = baseStats.critDamage

    // Reset event counters
    eventCounts.follows = 0
    eventCounts.subs = 0
    eventCounts.donations = 0
    eventCounts.bits = 0

    // Reset on backend
    if (userId.value) {
        try {
            await statsApi.resetMonthly(userId.value)
            console.log('Stats reset on backend')
        } catch (err) {
            console.error('Failed to reset on backend:', err.message)
        }
    }

    // Close modal
    showResetModal.value = false

    // Force page reload to reset visualization
    window.location.reload()
}

const logout = () => {
    localStorage.removeItem('twitch_token')
    localStorage.removeItem('twitch_user')
    router.push('/')
}

// Previous months history (will be loaded from backend)
const monthlyRecords = []

// Global rankings (will be loaded from backend)
const globalRankings = ref([
    { name: 'xQc', wave: 245, avatar: null },
    { name: 'Pokimane', wave: 198, avatar: null },
    { name: 'Shroud', wave: 187, avatar: null },
    { name: 'Ninja', wave: 156, avatar: null },
    { name: 'DrLupo', wave: 142, avatar: null },
    { name: 'TimTheTatman', wave: 128, avatar: null },
    { name: 'Summit1g', wave: 115, avatar: null },
])

// Current user's rank (will be loaded from backend)
const currentUserRank = ref({ rank: 42, wave: currentWave.value, avatar: null })

// Achievements (will be loaded from backend)
const achievements = ref([
    {
        id: 'first_kill',
        name: 'First Blood',
        description: 'Defeat your first monster',
        icon: '/cheevos/first_kill.png',
        unlocked: true,
        unlockedAt: '2 days ago',
        isNew: true,
        hint: 'Defeat a monster'
    },
    {
        id: 'wave_10',
        name: 'Getting Started',
        description: 'Reach wave 10',
        icon: '/cheevos/first_kill.png',
        unlocked: false,
        unlockedAt: null,
        isNew: false,
        hint: 'Keep fighting!'
    },
    {
        id: 'wave_50',
        name: 'Veteran',
        description: 'Reach wave 50',
        icon: '/cheevos/first_kill.png',
        unlocked: false,
        unlockedAt: null,
        isNew: false,
        hint: 'Half way there!'
    },
    {
        id: 'wave_100',
        name: 'Centurion',
        description: 'Reach wave 100',
        icon: '/cheevos/first_kill.png',
        unlocked: false,
        unlockedAt: null,
        isNew: false,
        hint: 'The century awaits!'
    },
    {
        id: 'first_sub',
        name: 'Community Support',
        description: 'Receive your first subscription',
        icon: '/cheevos/first_kill.png',
        unlocked: false,
        unlockedAt: null,
        isNew: false,
        hint: 'Get a subscriber'
    },
    {
        id: 'crit_master',
        name: 'Critical Master',
        description: 'Reach 50% critical chance',
        icon: '/cheevos/first_kill.png',
        unlocked: false,
        unlockedAt: null,
        isNew: false,
        hint: 'Followers boost crit!'
    },
    {
        id: 'speed_demon',
        name: 'Speed Demon',
        description: 'Reach 5.0 attack speed',
        icon: '/cheevos/first_kill.png',
        unlocked: false,
        unlockedAt: null,
        isNew: false,
        hint: 'Subs boost speed!'
    },
    {
        id: 'whale',
        name: 'Whale Watcher',
        description: 'Receive 10,000 bits total',
        icon: '/cheevos/first_kill.png',
        unlocked: false,
        unlockedAt: null,
        isNew: false,
        hint: 'Bits add up!'
    }
])

// Best wave achieved (history + current)
const bestWave = computed(() => {
    const historicalBest = monthlyRecords.length > 0 ? Math.max(...monthlyRecords.map(r => r.wave)) : 0
    return Math.max(currentWave.value, historicalBest)
})
</script>
