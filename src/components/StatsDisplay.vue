<template>
    <div class="bg-gradient-to-r rounded-lg p-4 border" :class="variant === 'live'
        ? 'from-purple-900/30 to-pink-900/30 border-purple-700/50'
        : 'from-green-900/30 to-emerald-900/30 border-green-700/50'">
        <h3 class="text-lg font-semibold text-white mb-3">
            {{ variant === 'live' ? '📡 Live Stats' : '🧪 Test Stats' }}
        </h3>
        <div class="space-y-2 text-sm">
            <div class="flex justify-between">
                <span class="text-cyan-400">⚔️ ATK</span>
                <span class="text-white font-mono">{{ stats.atk }}</span>
            </div>
            <div class="flex justify-between">
                <span class="text-green-400">⚡ SPD</span>
                <span class="text-white font-mono">{{ stats.spd.toFixed(1) }}/s</span>
            </div>
            <div class="flex justify-between">
                <span class="text-yellow-400">🎯 Crit %</span>
                <span class="text-white font-mono">{{ stats.critChance }}%</span>
            </div>
            <div class="flex justify-between">
                <span class="text-orange-400">💥 Crit DMG</span>
                <span class="text-white font-mono">{{ stats.critDamage }}%</span>
            </div>
        </div>
    </div>
</template>

<script setup>
defineProps({
    stats: {
        type: Object,
        default: () => ({ atk: 10, spd: 1.0, critChance: 5, critDamage: 150 })
    },
    variant: {
        type: String,
        default: 'live' // 'live' or 'test'
    }
})
</script>
