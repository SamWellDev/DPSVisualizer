<template>
    <div class="bg-gray-900 rounded-lg p-4 border border-gray-700">
        <!-- Header with Navigation -->
        <div class="flex items-center justify-between mb-3">
            <button @click="prevView"
                class="w-8 h-8 flex items-center justify-center text-gray-500 hover:text-white hover:bg-gray-800 rounded-lg transition-colors">
                ←
            </button>
            <div class="text-center">
                <h3 class="text-lg font-semibold text-white">
                    {{ currentView === 'global' ? '🌍 Global Rankings' : '👥 Friends Rankings' }}
                </h3>
                <div class="text-sm text-gray-400">
                    {{ currentView === 'global' ? 'Top streamers this month' : 'Your friends this month' }}
                </div>
            </div>
            <button @click="nextView"
                class="w-8 h-8 flex items-center justify-center text-gray-500 hover:text-white hover:bg-gray-800 rounded-lg transition-colors">
                →
            </button>
        </div>

        <!-- View Indicator Dots -->
        <div class="flex justify-center gap-2 mb-6">
            <div
                :class="['w-2 h-2 rounded-full transition-colors', currentView === 'global' ? 'bg-purple-500' : 'bg-gray-600']">
            </div>
            <div
                :class="['w-2 h-2 rounded-full transition-colors', currentView === 'friends' ? 'bg-purple-500' : 'bg-gray-600']">
            </div>
        </div>

        <!-- Top 3 Podium -->
        <div class="flex justify-center items-end gap-2 mb-4">
            <!-- 2nd Place -->
            <div v-if="activeRankings[1]" class="text-center">
                <div
                    class="w-16 h-16 mx-auto mb-1 rounded-full bg-gray-700 border-2 border-gray-500 flex items-center justify-center overflow-hidden">
                    <img v-if="activeRankings[1].avatar" :src="activeRankings[1].avatar"
                        class="w-full h-full object-cover" />
                    <span v-else class="text-2xl">🥈</span>
                </div>
                <div class="text-xs text-gray-400 truncate max-w-16">{{ activeRankings[1].name }}</div>
                <div class="text-sm font-bold text-gray-300">Wave {{ activeRankings[1].wave }}</div>
            </div>

            <!-- 1st Place -->
            <div v-if="activeRankings[0]" class="text-center -mt-4">
                <div
                    class="w-20 h-20 mx-auto mb-1 rounded-full bg-gradient-to-br from-yellow-400 to-yellow-600 border-4 border-yellow-300 flex items-center justify-center overflow-hidden shadow-lg shadow-yellow-500/30">
                    <img v-if="activeRankings[0].avatar" :src="activeRankings[0].avatar"
                        class="w-full h-full object-cover" />
                    <span v-else class="text-3xl">👑</span>
                </div>
                <div class="text-xs text-yellow-400 truncate max-w-20 font-medium">{{ activeRankings[0].name }}</div>
                <div class="text-lg font-bold text-yellow-300">Wave {{ activeRankings[0].wave }}</div>
            </div>

            <!-- 3rd Place -->
            <div v-if="activeRankings[2]" class="text-center">
                <div
                    class="w-16 h-16 mx-auto mb-1 rounded-full bg-gray-700 border-2 border-orange-700 flex items-center justify-center overflow-hidden">
                    <img v-if="activeRankings[2].avatar" :src="activeRankings[2].avatar"
                        class="w-full h-full object-cover" />
                    <span v-else class="text-2xl">🥉</span>
                </div>
                <div class="text-xs text-gray-400 truncate max-w-16">{{ activeRankings[2].name }}</div>
                <div class="text-sm font-bold text-gray-300">Wave {{ activeRankings[2].wave }}</div>
            </div>
        </div>

        <!-- Rest of Rankings (4-10) -->
        <div class="space-y-2">
            <div v-for="(player, index) in activeRankings.slice(3, 10)" :key="index + 3"
                class="flex items-center justify-between text-sm py-2 px-3 bg-gray-800 rounded hover:bg-gray-750 transition-colors">
                <div class="flex items-center gap-3">
                    <span class="text-gray-500 font-mono w-5">{{ index + 4 }}</span>
                    <div class="w-8 h-8 rounded-full bg-gray-700 flex items-center justify-center overflow-hidden">
                        <img v-if="player.avatar" :src="player.avatar" class="w-full h-full object-cover" />
                        <span v-else class="text-sm">👤</span>
                    </div>
                    <span class="text-gray-300 truncate max-w-24">{{ player.name }}</span>
                </div>
                <span class="text-white font-bold">Wave {{ player.wave }}</span>
            </div>
        </div>

        <!-- Empty State -->
        <div v-if="activeRankings.length === 0" class="text-center py-8">
            <div class="text-4xl mb-2">{{ currentView === 'global' ? '🏆' : '👥' }}</div>
            <div class="text-gray-500 text-sm">
                {{ currentView === 'global' ? 'No rankings yet this month' : 'No friends playing yet' }}
            </div>
            <div class="text-gray-600 text-xs">
                {{ currentView === 'global' ? 'Be the first to climb the waves!' : 'Invite your friends to join!' }}
            </div>
        </div>

        <!-- Your Rank -->
        <div v-if="currentUserRank" class="mt-4 pt-4 border-t border-gray-700">
            <div
                class="flex items-center justify-between text-sm py-2 px-3 bg-purple-900/30 rounded border border-purple-700/50">
                <div class="flex items-center gap-3">
                    <span class="text-purple-400 font-mono w-5">#{{ currentUserRank.rank }}</span>
                    <div class="w-8 h-8 rounded-full bg-purple-700 flex items-center justify-center overflow-hidden">
                        <img v-if="currentUserRank.avatar" :src="currentUserRank.avatar"
                            class="w-full h-full object-cover" />
                        <span v-else class="text-sm">⭐</span>
                    </div>
                    <span class="text-purple-300 font-medium">You</span>
                </div>
                <span class="text-purple-200 font-bold">Wave {{ currentUserRank.wave }}</span>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
    rankings: {
        type: Array,
        default: () => []
        // Expected format: [{ name: 'username', wave: 42, avatar: 'url' }, ...]
    },
    friendsRankings: {
        type: Array,
        default: () => []
        // Same format as rankings
    },
    currentUserRank: {
        type: Object,
        default: null
        // Expected format: { rank: 15, name: 'you', wave: 25, avatar: 'url' }
    }
})

const currentView = ref('global') // 'global' or 'friends'

const activeRankings = computed(() => {
    return currentView.value === 'global' ? props.rankings : props.friendsRankings
})

const nextView = () => {
    currentView.value = currentView.value === 'global' ? 'friends' : 'global'
}

const prevView = () => {
    currentView.value = currentView.value === 'global' ? 'friends' : 'global'
}
</script>
