<template>
    <div class="bg-gray-900 rounded-lg p-4 border border-gray-700">
        <div class="flex items-center justify-between mb-3">
            <h3 class="text-lg font-semibold text-white">🏅 Achievements</h3>
            <span class="text-xs text-gray-500">{{ unlockedCount }}/{{ achievements.length }}</span>
        </div>

        <!-- Achievement Grid -->
        <div class="grid grid-cols-4 gap-3">
            <div v-for="achievement in achievements" :key="achievement.id" class="relative group cursor-pointer"
                @click="showAchievementDetails(achievement)">

                <!-- Badge Container -->
                <div :class="[
                    'w-full aspect-square rounded-lg border-2 overflow-hidden transition-all duration-300',
                    achievement.unlocked
                        ? 'border-yellow-500/50 bg-gray-800 hover:border-yellow-400 hover:scale-105 hover:shadow-lg hover:shadow-yellow-500/20'
                        : 'border-gray-700 bg-gray-800/50 grayscale opacity-40'
                ]">
                    <img :src="achievement.icon" :alt="achievement.name" class="w-full h-full object-contain p-1" />
                </div>

                <!-- Locked Overlay -->
                <div v-if="!achievement.unlocked" class="absolute inset-0 flex items-center justify-center">
                    <span class="text-2xl">🔒</span>
                </div>

                <!-- New Badge Indicator -->
                <div v-if="achievement.unlocked && achievement.isNew"
                    class="absolute -top-1 -right-1 w-3 h-3 bg-red-500 rounded-full animate-pulse">
                </div>
            </div>
        </div>

        <!-- Recent Achievement Banner -->
        <div v-if="recentAchievement"
            class="mt-4 bg-gradient-to-r from-yellow-900/40 to-orange-900/40 rounded-lg p-3 border border-yellow-600/50">
            <div class="flex items-center gap-3">
                <img :src="recentAchievement.icon" class="w-12 h-12 object-contain" />
                <div class="flex-1">
                    <div class="text-xs text-yellow-400 uppercase tracking-wide">Latest Unlock!</div>
                    <div class="text-white font-semibold">{{ recentAchievement.name }}</div>
                    <div class="text-xs text-gray-400">{{ recentAchievement.description }}</div>
                </div>
            </div>
        </div>

        <!-- Achievement Detail Modal -->
        <div v-if="selectedAchievement" class="fixed inset-0 bg-black/70 flex items-center justify-center z-50"
            @click.self="selectedAchievement = null">
            <div class="bg-gray-800 rounded-xl border border-gray-700 p-6 max-w-sm w-full mx-4 text-center">
                <img :src="selectedAchievement.icon" :class="[
                    'w-24 h-24 mx-auto mb-4 object-contain',
                    !selectedAchievement.unlocked && 'grayscale opacity-50'
                ]" />
                <h4 class="text-xl font-bold text-white mb-2">{{ selectedAchievement.name }}</h4>
                <p class="text-gray-400 text-sm mb-4">{{ selectedAchievement.description }}</p>

                <div v-if="selectedAchievement.unlocked" class="text-green-400 text-sm">
                    ✅ Unlocked {{ selectedAchievement.unlockedAt }}
                </div>
                <div v-else class="text-gray-500 text-sm">
                    🔒 {{ selectedAchievement.hint || 'Keep playing to unlock!' }}
                </div>

                <button @click="selectedAchievement = null"
                    class="mt-4 py-2 px-6 bg-gray-700 hover:bg-gray-600 text-white rounded-lg transition-colors">
                    Close
                </button>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
    achievements: {
        type: Array,
        default: () => []
        // Expected format: 
        // [{ 
        //   id: 'first_kill', 
        //   name: 'First Blood', 
        //   description: 'Defeat your first monster',
        //   icon: '/cheevos/first_kill.png',
        //   unlocked: true,
        //   unlockedAt: '2 days ago',
        //   isNew: false,
        //   hint: 'Defeat a monster'
        // }]
    }
})

const selectedAchievement = ref(null)

const unlockedCount = computed(() => {
    return props.achievements.filter(a => a.unlocked).length
})

const recentAchievement = computed(() => {
    return props.achievements.find(a => a.unlocked && a.isNew)
})

const showAchievementDetails = (achievement) => {
    selectedAchievement.value = achievement
}
</script>
