<template>
    <div class="bg-gray-900 rounded-lg p-4 border border-gray-700">
        <h3 class="text-lg font-semibold text-white mb-3">🏆 Monthly Progress</h3>
        <div class="text-sm text-gray-400 mb-3">
            Resets on the 1st of each month
        </div>

        <!-- Current Month Progress -->
        <div class="bg-gradient-to-r from-yellow-900/30 to-orange-900/30 rounded p-4 border border-yellow-700/50 mb-3">
            <div class="text-xs text-yellow-400 uppercase tracking-wide mb-1">{{ currentMonth }}</div>
            <div class="flex justify-between items-center">
                <span class="text-gray-300">Current Wave</span>
                <span class="text-3xl font-bold text-yellow-400">{{ currentWave }}</span>
            </div>
        </div>

        <!-- History of Previous Months -->
        <div class="space-y-2">
            <div class="text-xs text-gray-500 uppercase tracking-wide">Previous Months</div>
            <div v-for="(record, index) in records" :key="index"
                class="flex justify-between text-sm py-2 px-3 bg-gray-800 rounded">
                <span class="text-gray-400">{{ record.month }}</span>
                <span class="text-white font-bold">Wave {{ record.wave }}</span>
            </div>
        </div>
    </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
    currentWave: { type: Number, default: 1 },
    records: {
        type: Array,
        default: () => []
    }
})

const currentMonth = computed(() => {
    const months = ['January', 'February', 'March', 'April', 'May', 'June',
        'July', 'August', 'September', 'October', 'November', 'December']
    return months[new Date().getMonth()] + ' ' + new Date().getFullYear()
})
</script>
