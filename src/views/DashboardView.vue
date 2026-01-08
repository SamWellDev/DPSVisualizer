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
                        <VisualizationArea :heroStats="activeStats" :bestWave="bestWave" :dps="calculatedDPS"
                            :layoutMode="layoutMode" :showDamageNumbers="showDamageNumbers" :isLive="isLive"
                            @monsterDefeated="onMonsterDefeated" />
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

                        <!-- Live Toggle Button -->
                        <button @click="toggleLive" :class="[
                            'w-full py-3 px-4 rounded-lg font-medium transition-colors flex items-center justify-center gap-2 mb-4',
                            isLive
                                ? 'bg-red-600 hover:bg-red-500 text-white'
                                : 'bg-green-600 hover:bg-green-500 text-white'
                        ]">
                            <span v-if="isLive" class="w-3 h-3 bg-white rounded-full animate-pulse"></span>
                            {{ isLive ? '⏹️ Manual Shutdown' : '▶️ Go Live' }}
                        </button>

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
import { reactive, computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import VisualizationArea from '../components/VisualizationArea.vue'
import AppHeader from '../components/AppHeader.vue'
import AppFooter from '../components/AppFooter.vue'
import StatsDisplay from '../components/StatsDisplay.vue'
import MonthlyProgress from '../components/MonthlyProgress.vue'
import EventCounters from '../components/EventCounters.vue'
import GlobalRankings from '../components/GlobalRankings.vue'
import Achievements from '../components/Achievements.vue'

const router = useRouter()

const currentWave = ref(1)
const activeTab = ref('config')
const layoutMode = ref('full') // 'full' or 'monster'
const showDamageNumbers = ref(true)
const showResetModal = ref(false)
const isLive = ref(true) // Live/Offline toggle

const toggleLive = () => {
    isLive.value = !isLive.value
}

// Buff configuration (fixed values - not editable in UI)
const buffConfig = reactive({
    follow: { critChance: 2 },
    sub1: { spd: 0.5 },
    sub2: { spd: 1.0 },
    sub3: { spd: 1.5, atk: 10 },
    donate5: { atk: 5 },
    donate10: { atk: 20, critDamage: 15 },
    bits: { critDamage: 1 } // 1 bit = 1% crit damage
})

// Event counters (for display)
const eventCounts = reactive({
    follows: 0,
    subs: 0,
    donations: 0,
    bits: 0
})

// Base stats (initial values)
const baseStats = {
    atk: 10,
    spd: 1.0,
    critChance: 5,
    critDamage: 150
}

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

// Simulate event (applies to active stats)
const simulateEvent = (eventType) => {
    const stats = activeStats.value

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

const onMonsterDefeated = (wave) => {
    currentWave.value = wave
}

const resetTestStats = () => {
    testStats.atk = baseStats.atk
    testStats.spd = baseStats.spd
    testStats.critChance = baseStats.critChance
    testStats.critDamage = baseStats.critDamage
}

const saveConfig = () => {
    localStorage.setItem('buffConfig', JSON.stringify(buffConfig))
    alert('✅ Configuration saved!')
}

const openShop = () => {
    alert('🛒 Shop coming soon!')
}

const exportOBS = () => {
    alert('📤 Export for OBS coming soon!\n\nThis will generate a Browser Source URL that you can add to OBS to display the overlay on your stream.')
}

const confirmReset = () => {
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
