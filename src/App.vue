<template>
  <div class="min-h-screen bg-gray-900">
    <!-- Header -->
    <header class="bg-gray-800 border-b border-gray-700 py-4">
      <div class="container mx-auto px-6 flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold text-white">⚔️ Boss Fight Overlay</h1>
          <p class="text-gray-400 text-sm mt-1">Stream Interactive Boss Battle</p>
        </div>
        <div class="text-right">
          <div class="text-xl font-bold text-yellow-400">Wave {{ currentWave }}</div>
          <div class="text-sm text-gray-400">DPS: {{ calculatedDPS }}</div>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="container mx-auto px-6 py-6">
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">

        <!-- Área de Visualização (2/3) -->
        <div class="lg:col-span-2 bg-gray-800 rounded-xl border border-gray-700 overflow-hidden" style="height: 500px;">
          <VisualizationArea :heroStats="heroStats" :bestWave="bestWave" @monsterDefeated="onMonsterDefeated" />
        </div>

        <!-- Controles (1/3) -->
        <div class="bg-gray-800 rounded-xl border border-gray-700 p-6">
          <h2 class="text-xl font-semibold text-white mb-4">🎮 Event Simulator</h2>
          <p class="text-gray-400 text-sm mb-4">Simule eventos de stream para testar</p>

          <!-- Botões de Eventos -->
          <div class="space-y-3 mb-6">
            <button @click="simulateEvent('follow')"
              class="w-full py-3 px-4 bg-purple-600 hover:bg-purple-500 text-white rounded-lg font-medium transition-colors flex items-center justify-between">
              <span>👤 Follow</span>
              <span class="text-purple-200 text-sm">+2% Crit</span>
            </button>

            <button @click="simulateEvent('sub1')"
              class="w-full py-3 px-4 bg-blue-600 hover:bg-blue-500 text-white rounded-lg font-medium transition-colors flex items-center justify-between">
              <span>⭐ Sub Tier 1</span>
              <span class="text-blue-200 text-sm">+0.5 SPD</span>
            </button>

            <button @click="simulateEvent('sub2')"
              class="w-full py-3 px-4 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg font-medium transition-colors flex items-center justify-between">
              <span>⭐⭐ Sub Tier 2</span>
              <span class="text-indigo-200 text-sm">+1.0 SPD</span>
            </button>

            <button @click="simulateEvent('sub3')"
              class="w-full py-3 px-4 bg-pink-600 hover:bg-pink-500 text-white rounded-lg font-medium transition-colors flex items-center justify-between">
              <span>⭐⭐⭐ Sub Tier 3</span>
              <span class="text-pink-200 text-sm">+1.5 SPD, +10 ATK</span>
            </button>

            <button @click="simulateEvent('donate5')"
              class="w-full py-3 px-4 bg-green-600 hover:bg-green-500 text-white rounded-lg font-medium transition-colors flex items-center justify-between">
              <span>💵 Donate $5</span>
              <span class="text-green-200 text-sm">+5 ATK</span>
            </button>

            <button @click="simulateEvent('donate10')"
              class="w-full py-3 px-4 bg-yellow-600 hover:bg-yellow-500 text-white rounded-lg font-medium transition-colors flex items-center justify-between">
              <span>💰 Donate $10+</span>
              <span class="text-yellow-200 text-sm">+20 ATK, +15% Crit DMG</span>
            </button>
          </div>

          <!-- Stats do Herói -->
          <div class="bg-gray-900 rounded-lg p-4 border border-gray-700">
            <h3 class="text-lg font-semibold text-white mb-3">📊 Hero Stats</h3>
            <div class="space-y-2 text-sm">
              <div class="flex justify-between">
                <span class="text-cyan-400">⚔️ ATK</span>
                <span class="text-white font-mono">{{ heroStats.atk }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-green-400">⚡ SPD</span>
                <span class="text-white font-mono">{{ heroStats.spd.toFixed(1) }}/s</span>
              </div>
              <div class="flex justify-between">
                <span class="text-yellow-400">🎯 Crit %</span>
                <span class="text-white font-mono">{{ heroStats.critChance }}%</span>
              </div>
              <div class="flex justify-between">
                <span class="text-orange-400">💥 Crit DMG</span>
                <span class="text-white font-mono">{{ heroStats.critDamage }}%</span>
              </div>
            </div>
          </div>

          <!-- Reset Button -->
          <button @click="resetStats"
            class="w-full mt-4 py-2 px-4 bg-red-600 hover:bg-red-500 text-white rounded-lg font-medium transition-colors">
            🔄 Reset Stats
          </button>

          <!-- Wave Progress / Records -->
          <div class="bg-gray-900 rounded-lg p-4 border border-gray-700 mt-4">
            <h3 class="text-lg font-semibold text-white mb-3">🏆 Progresso Mensal</h3>
            <div class="text-sm text-gray-400 mb-3">
              Reseta dia 1° de cada mês
            </div>

            <!-- Current Month Progress -->
            <div
              class="bg-gradient-to-r from-yellow-900/30 to-orange-900/30 rounded p-4 border border-yellow-700/50 mb-3">
              <div class="text-xs text-yellow-400 uppercase tracking-wide mb-1">{{ getCurrentMonth() }}</div>
              <div class="flex justify-between items-center">
                <span class="text-gray-300">Wave Atual</span>
                <span class="text-3xl font-bold text-yellow-400">{{ currentWave }}</span>
              </div>
            </div>

            <!-- History of Previous Months -->
            <div class="space-y-2">
              <div class="text-xs text-gray-500 uppercase tracking-wide">Meses Anteriores</div>
              <div v-for="(record, index) in monthlyRecords" :key="index"
                class="flex justify-between text-sm py-2 px-3 bg-gray-800 rounded">
                <span class="text-gray-400">{{ record.month }}</span>
                <span class="text-white font-bold">Wave {{ record.wave }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { reactive, computed, ref } from 'vue'
import VisualizationArea from './components/VisualizationArea.vue'

const currentWave = ref(1)

const heroStats = reactive({
  atk: 10,
  spd: 1.0,
  critChance: 5,
  critDamage: 150
})

const calculatedDPS = computed(() => {
  const baseDPS = heroStats.atk * heroStats.spd
  const critBonus = (heroStats.critChance / 100) * (heroStats.critDamage / 100 - 1) * baseDPS
  return (baseDPS + critBonus).toFixed(1)
})

const simulateEvent = (eventType) => {
  switch (eventType) {
    case 'follow':
      heroStats.critChance = Math.min(100, heroStats.critChance + 2)
      break
    case 'sub1':
      heroStats.spd = Math.min(10, heroStats.spd + 0.5)
      break
    case 'sub2':
      heroStats.spd = Math.min(10, heroStats.spd + 1.0)
      break
    case 'sub3':
      heroStats.spd = Math.min(10, heroStats.spd + 1.5)
      heroStats.atk += 10
      break
    case 'donate5':
      heroStats.atk += 5
      break
    case 'donate10':
      heroStats.atk += 20
      heroStats.critDamage += 15
      break
  }
}

const onMonsterDefeated = (wave) => {
  currentWave.value = wave
}

const resetStats = () => {
  // Reseta apenas os stats do herói, NÃO a wave (wave é contínua no mês)
  heroStats.atk = 10
  heroStats.spd = 1.0
  heroStats.critChance = 5
  heroStats.critDamage = 150
}

// Histórico de meses anteriores (placeholder - futuramente conectar com backend)
const monthlyRecords = [
  { month: 'Dezembro 2025', wave: 47 },
  { month: 'Novembro 2025', wave: 32 },
  { month: 'Outubro 2025', wave: 28 }
]

// Melhor wave já alcançada (histórico + atual)
const bestWave = computed(() => {
  const historicalBest = Math.max(...monthlyRecords.map(r => r.wave))
  return Math.max(currentWave.value, historicalBest)
})

// Obter mês atual formatado
const getCurrentMonth = () => {
  const months = ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
    'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro']
  return months[new Date().getMonth()] + ' ' + new Date().getFullYear()
}
</script>
