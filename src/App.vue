<template>
  <div class="min-h-screen bg-white">
    <!-- Header -->
    <header class="bg-white border-b border-blue-100 py-6">
      <div class="container mx-auto px-6">
        <h1 class="text-3xl font-bold text-gray-800">Visualizador de DPS</h1>
        <p class="text-gray-600 mt-2">Calcule e visualize o dano por segundo em tempo real</p>
      </div>
    </header>

    <!-- Main Content -->
    <main class="container mx-auto px-6 py-8">
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 h-[600px]">
        <!-- Área de Visualização (Esquerda) -->
        <div class="bg-gradient-to-br from-blue-50 to-blue-100 rounded-xl border border-blue-200 p-6 flex items-center justify-center">
          <VisualizationArea 
            :numberOfShots="controls.numberOfShots"
            :rateOfFire="controls.rateOfFire"
            :power="controls.power"
          />
        </div>

        <!-- Controles (Direita) -->
        <div class="bg-white rounded-xl border border-gray-200 p-6 shadow-sm">
          <h2 class="text-2xl font-semibold text-gray-800 mb-6">Controles de DPS</h2>
          
          <div class="space-y-8">
            <!-- Number of Shots -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-3">
                Number of Shots: {{ controls.numberOfShots }}
              </label>
              <input
                type="range"
                min="1"
                max="10"
                v-model="controls.numberOfShots"
                class="w-full h-2 bg-blue-100 rounded-lg appearance-none cursor-pointer slider"
              />
              <div class="flex justify-between text-xs text-gray-500 mt-1">
                <span>1</span>
                <span>10</span>
              </div>
            </div>

            <!-- Rate of Fire -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-3">
                Rate of Fire: {{ controls.rateOfFire }}
              </label>
              <input
                type="range"
                min="1"
                max="10"
                v-model="controls.rateOfFire"
                class="w-full h-2 bg-blue-100 rounded-lg appearance-none cursor-pointer slider"
              />
              <div class="flex justify-between text-xs text-gray-500 mt-1">
                <span>1</span>
                <span>10</span>
              </div>
            </div>

            <!-- Power -->
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-3">
                Power: {{ controls.power }}
              </label>
              <input
                type="range"
                min="1"
                max="10"
                v-model="controls.power"
                class="w-full h-2 bg-blue-100 rounded-lg appearance-none cursor-pointer slider"
              />
              <div class="flex justify-between text-xs text-gray-500 mt-1">
                <span>1</span>
                <span>10</span>
              </div>
            </div>

            <!-- DPS Display -->
            <div class="bg-blue-50 rounded-lg p-4 border border-blue-200">
              <h3 class="text-lg font-semibold text-blue-800 mb-2">DPS Calculado</h3>
              <div class="text-3xl font-bold text-blue-600">
                {{ calculatedDPS }}
              </div>
              <p class="text-sm text-blue-600 mt-1">Dano por Segundo</p>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import VisualizationArea from './components/VisualizationArea.vue'

const controls = ref({
  numberOfShots: 5,
  rateOfFire: 5,
  power: 5
})

const calculatedDPS = computed(() => {
  return (controls.value.numberOfShots * controls.value.rateOfFire * controls.value.power * 2).toFixed(1)
})
</script>

