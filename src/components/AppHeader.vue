<template>
    <header class="bg-gray-800 border-b border-gray-700 py-4">
        <div class="container mx-auto px-6 flex justify-between items-center">
            <div>
                <h1 class="text-2xl font-bold text-white">⚔️ Twitch Fighter</h1>
                <p class="text-gray-400 text-sm mt-1">Stream Interactive Boss Battle</p>
            </div>
            <div class="flex items-center gap-4">
                <div class="text-right">
                    <div class="text-xl font-bold text-yellow-400">Wave {{ wave }}</div>
                    <div class="text-sm text-gray-400">DPS: {{ dps }}</div>
                </div>
                <!-- Shop Button -->
                <button @click="$emit('openShop')"
                    class="px-4 py-2 bg-yellow-600 hover:bg-yellow-500 text-white rounded-lg text-sm font-medium transition-colors">
                    🛒 Shop
                </button>
                <!-- Export for OBS -->
                <button @click="$emit('exportOBS')"
                    class="px-4 py-2 bg-blue-600 hover:bg-blue-500 text-white rounded-lg text-sm font-medium transition-colors">
                    📤 Export OBS
                </button>
                <!-- Reset Button -->
                <button @click="$emit('reset')"
                    class="px-4 py-2 bg-red-600 hover:bg-red-500 text-white rounded-lg text-sm font-medium transition-colors">
                    🔄 Reset
                </button>
                <!-- Logout -->
                <button @click="$emit('logout')"
                    class="px-4 py-2 bg-gray-700 hover:bg-gray-600 text-gray-300 rounded-lg text-sm transition-colors">
                    Logout
                </button>
            </div>
        </div>
    </header>
</template>

<script setup>
defineProps({
    wave: { type: Number, default: 1 },
    dps: { type: [Number, String], default: 0 }
})

defineEmits(['openShop', 'logout', 'exportOBS', 'reset'])
</script>
