<template>
    <div class="bg-gradient-to-r from-purple-900/30 to-pink-900/30 rounded-lg p-4 border border-purple-700/50">
        <h3 class="text-lg font-semibold text-white mb-3">📊 Live Events</h3>
        <div class="space-y-2 text-sm">
            <div class="flex justify-between">
                <span class="text-purple-400">👤 Follows</span>
                <span class="text-white font-mono">{{ counts.follows }}</span>
            </div>
            <div class="flex justify-between">
                <span class="text-blue-400">⭐ Subscriptions</span>
                <span class="text-white font-mono">{{ counts.subs }}</span>
            </div>
            <div class="flex justify-between">
                <span class="text-yellow-400">💰 Donations</span>
                <span class="text-white font-mono">${{ counts.donations.toFixed(2) }}</span>
            </div>
            <div class="flex justify-between">
                <span class="text-orange-400">💎 Bits</span>
                <span class="text-white font-mono">{{ counts.bits }}</span>
            </div>
        </div>
        <div class="mt-3 pt-3 border-t border-purple-700/50 text-xs text-gray-400 text-center">
            {{ periodLabel }}
        </div>
    </div>
</template>

<script setup>
defineProps({
    counts: {
        type: Object,
        default: () => ({ follows: 0, subs: 0, donations: 0, bits: 0 })
    },
    periodLabel: {
        type: String,
        default: 'This month'
    }
})
</script>
