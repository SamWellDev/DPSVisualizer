<template>
    <div class="min-h-screen bg-gray-900 flex flex-col">
        <!-- Header -->
        <header class="bg-gray-800 border-b border-gray-700 px-6 py-4">
            <div class="container mx-auto flex items-center justify-between">
                <div class="flex items-center gap-4">
                    <router-link to="/dashboard" class="text-gray-400 hover:text-white transition-colors">
                        ← Back to Dashboard
                    </router-link>
                    <h1 class="text-xl font-bold text-white">🛒 Hero Skins Shop</h1>
                </div>
            </div>
        </header>

        <!-- Main Content -->
        <main class="container mx-auto px-6 py-8 flex-1">
            <!-- Current Selection -->
            <div class="bg-gray-800 rounded-xl border border-gray-700 p-6 mb-8">
                <h2 class="text-lg font-semibold text-white mb-4">Currently Equipped</h2>
                <div class="flex items-center gap-4">
                    <div class="w-24 h-24 bg-gray-900 rounded-lg flex items-center justify-center overflow-hidden">
                        <img :src="currentSkinPreview" alt="Current skin" class="w-20 h-20 object-contain" />
                    </div>
                    <div>
                        <p class="text-white font-medium">{{ currentSkinName }}</p>
                        <p class="text-gray-400 text-sm">Your active hero skin</p>
                    </div>
                </div>
            </div>

            <!-- Skins Grid -->
            <h2 class="text-lg font-semibold text-white mb-4">Available Skins</h2>
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                <!-- Default Skin -->
                <div @click="selectSkin('hero_1')" :class="[
                    'bg-gray-800 rounded-xl border p-6 cursor-pointer transition-all hover:scale-105',
                    selectedSkin === 'hero_1' ? 'border-purple-500 ring-2 ring-purple-500' : 'border-gray-700 hover:border-gray-600'
                ]">
                    <div
                        class="aspect-square bg-gray-900 rounded-lg mb-4 flex items-center justify-center overflow-hidden">
                        <img src="/sprites/hero_1.png" alt="Default" class="w-32 h-32 object-contain" />
                    </div>
                    <h3 class="text-white font-semibold mb-1">Default</h3>
                    <p class="text-gray-400 text-sm mb-3">The classic soldier</p>
                    <div class="flex items-center justify-between">
                        <span class="text-green-400 text-sm font-medium">✓ Owned</span>
                        <button v-if="selectedSkin !== 'hero_1'"
                            class="px-3 py-1 bg-purple-600 hover:bg-purple-500 text-white text-sm rounded-lg transition-colors">
                            Equip
                        </button>
                        <span v-else class="text-purple-400 text-sm">Equipped</span>
                    </div>
                </div>

                <!-- Brute Skin -->
                <div @click="selectSkin('brute')" :class="[
                    'bg-gray-800 rounded-xl border p-6 cursor-pointer transition-all hover:scale-105',
                    selectedSkin === 'brute' ? 'border-purple-500 ring-2 ring-purple-500' : 'border-gray-700 hover:border-gray-600'
                ]">
                    <div
                        class="aspect-square bg-gray-900 rounded-lg mb-4 flex items-center justify-center overflow-hidden">
                        <img src="/sprites/brute.png" alt="Brute" class="w-32 h-32 object-contain" />
                    </div>
                    <h3 class="text-white font-semibold mb-1">Brute</h3>
                    <p class="text-gray-400 text-sm mb-3">Heavy armor warrior</p>
                    <div class="flex items-center justify-between">
                        <span class="text-green-400 text-sm font-medium">✓ Owned</span>
                        <button v-if="selectedSkin !== 'brute'"
                            class="px-3 py-1 bg-purple-600 hover:bg-purple-500 text-white text-sm rounded-lg transition-colors">
                            Equip
                        </button>
                        <span v-else class="text-purple-400 text-sm">Equipped</span>
                    </div>
                </div>

                <!-- Green Skin -->
                <div @click="selectSkin('green')" :class="[
                    'bg-gray-800 rounded-xl border p-6 cursor-pointer transition-all hover:scale-105',
                    selectedSkin === 'green' ? 'border-purple-500 ring-2 ring-purple-500' : 'border-gray-700 hover:border-gray-600'
                ]">
                    <div
                        class="aspect-square bg-gray-900 rounded-lg mb-4 flex items-center justify-center overflow-hidden">
                        <img src="/sprites/green.png" alt="Green" class="w-32 h-32 object-contain" />
                    </div>
                    <h3 class="text-white font-semibold mb-1">Green</h3>
                    <p class="text-gray-400 text-sm mb-3">Toxic commando</p>
                    <div class="flex items-center justify-between">
                        <span class="text-green-400 text-sm font-medium">✓ Owned</span>
                        <button v-if="selectedSkin !== 'green'"
                            class="px-3 py-1 bg-purple-600 hover:bg-purple-500 text-white text-sm rounded-lg transition-colors">
                            Equip
                        </button>
                        <span v-else class="text-purple-400 text-sm">Equipped</span>
                    </div>
                </div>

                <!-- Slim Skin -->
                <div @click="selectSkin('slim')" :class="[
                    'bg-gray-800 rounded-xl border p-6 cursor-pointer transition-all hover:scale-105',
                    selectedSkin === 'slim' ? 'border-purple-500 ring-2 ring-purple-500' : 'border-gray-700 hover:border-gray-600'
                ]">
                    <div
                        class="aspect-square bg-gray-900 rounded-lg mb-4 flex items-center justify-center overflow-hidden">
                        <img src="/sprites/slim.png" alt="Slim" class="w-32 h-32 object-contain" />
                    </div>
                    <h3 class="text-white font-semibold mb-1">Slim</h3>
                    <p class="text-gray-400 text-sm mb-3">Agile assassin</p>
                    <div class="flex items-center justify-between">
                        <span class="text-green-400 text-sm font-medium">✓ Owned</span>
                        <button v-if="selectedSkin !== 'slim'"
                            class="px-3 py-1 bg-purple-600 hover:bg-purple-500 text-white text-sm rounded-lg transition-colors">
                            Equip
                        </button>
                        <span v-else class="text-purple-400 text-sm">Equipped</span>
                    </div>
                </div>
            </div>

            <!-- Coming Soon Section -->
            <div class="mt-12">
                <h2 class="text-lg font-semibold text-gray-500 mb-4">🔒 Coming Soon</h2>
                <p class="text-gray-600 text-sm">
                    More skins will be available for purchase with channel points, bits, or subscriptions!
                </p>
            </div>
        </main>
    </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { userConfigApi } from '../services/api'

// Skin selection
const selectedSkin = ref('hero_1')
const twitchId = localStorage.getItem('twitch_id') // Only use localStorage for auth

const skinNames = {
    'hero_1': 'Default',
    'brute': 'Brute',
    'green': 'Green',
    'slim': 'Slim'
}

const currentSkinName = computed(() => skinNames[selectedSkin.value] || 'Default')

const currentSkinPreview = computed(() => {
    if (selectedSkin.value === 'hero_1') {
        return '/sprites/hero_1.png'
    }
    return `/sprites/${selectedSkin.value}.png`
})

// Load skin from backend on mount
onMounted(async () => {
    if (twitchId) {
        try {
            const res = await userConfigApi.get(twitchId)
            selectedSkin.value = res.data.heroSkin || 'hero_1'
            console.log('Skin loaded from backend:', selectedSkin.value)
        } catch (err) {
            console.warn('Failed to load skin from backend:', err.message)
        }
    }
})

// Save skin to backend when selected
const selectSkin = async (skin) => {
    selectedSkin.value = skin

    if (twitchId) {
        try {
            // Get current config and update just the skin
            const currentConfig = await userConfigApi.get(twitchId)
            await userConfigApi.save(twitchId, {
                ...currentConfig.data,
                heroSkin: skin
            })
            console.log('Skin saved to backend:', skin)
        } catch (err) {
            console.error('Failed to save skin:', err.message)
        }
    }
}
</script>
