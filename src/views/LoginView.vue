<template>
    <div
        class="min-h-screen bg-gradient-to-br from-gray-900 via-purple-900/20 to-gray-900 flex items-center justify-center p-4">
        <!-- Login Card -->
        <div class="w-full max-w-md">
            <!-- Logo / Title -->
            <div class="text-center mb-8">
                <div class="text-6xl mb-4">⚔️</div>
                <h1 class="text-3xl font-bold text-white mb-2">Twitch Fighter</h1>
                <p class="text-gray-400">Turn your stream into an epic battle</p>
            </div>

            <!-- Login Card -->
            <div class="bg-gray-800/80 backdrop-blur-sm rounded-2xl border border-gray-700 p-8 shadow-2xl">
                <div class="text-center mb-6">
                    <h2 class="text-xl font-semibold text-white mb-2">Welcome!</h2>
                    <p class="text-gray-400 text-sm">
                        Connect your Twitch account to start using the interactive overlay on your streams.
                    </p>
                </div>

                <!-- Twitch Login Button -->
                <button @click="loginWithTwitch"
                    class="w-full py-4 px-6 bg-purple-600 hover:bg-purple-500 text-white rounded-xl font-semibold text-lg transition-all duration-200 flex items-center justify-center gap-3 hover:scale-[1.02] hover:shadow-lg hover:shadow-purple-500/25">
                    <svg class="w-6 h-6" viewBox="0 0 24 24" fill="currentColor">
                        <path
                            d="M11.571 4.714h1.715v5.143H11.57zm4.715 0H18v5.143h-1.714zM6 0L1.714 4.286v15.428h5.143V24l4.286-4.286h3.428L22.286 12V0zm14.571 11.143l-3.428 3.428h-3.429l-3 3v-3H6.857V1.714h13.714Z" />
                    </svg>
                    Login with Twitch
                </button>

                <!-- Info -->
                <div class="mt-6 text-center">
                    <p class="text-xs text-gray-500">
                        By connecting, you authorize access to your stream events.
                    </p>
                </div>
            </div>

            <!-- Features Preview -->
            <div class="mt-8 grid grid-cols-3 gap-4 text-center">
                <div class="bg-gray-800/50 rounded-xl p-4 border border-gray-700/50">
                    <div class="text-2xl mb-2">👤</div>
                    <div class="text-xs text-gray-400">Follows = Crit</div>
                </div>
                <div class="bg-gray-800/50 rounded-xl p-4 border border-gray-700/50">
                    <div class="text-2xl mb-2">⭐</div>
                    <div class="text-xs text-gray-400">Subs = Speed</div>
                </div>
                <div class="bg-gray-800/50 rounded-xl p-4 border border-gray-700/50">
                    <div class="text-2xl mb-2">💰</div>
                    <div class="text-xs text-gray-400">Bits = ATK</div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// Backend auth URL
const BACKEND_AUTH_URL = 'http://localhost:5041/api/auth/login'

// Check if already logged in
onMounted(() => {
    const twitchId = localStorage.getItem('twitch_id')
    if (twitchId) {
        router.push('/dashboard')
    }
})

const loginWithTwitch = () => {
    // Redirect to backend which handles OAuth
    window.location.href = BACKEND_AUTH_URL
}
</script>
