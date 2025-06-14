<template>
  <div class="relative w-full h-full overflow-hidden bg-gray-900">
    <canvas ref="canvas" class="w-full h-full"></canvas>
    
    <!-- Stats Display -->
    <div class="absolute bottom-4 left-4 bg-black bg-opacity-70 rounded-lg p-3 text-sm text-white">
      <div class="font-mono flex space-x-4">
        <div>
          <div class="text-cyan-400">TIROS:</div>
          <div class="text-center">{{ numberOfShots }}</div>
        </div>
        <div>
          <div class="text-green-400">TAXA:</div>
          <div class="text-center">{{ rateOfFire }}/s</div>
        </div>
        <div>
          <div class="text-red-400">PODER:</div>
          <div class="text-center">{{ power }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, watch } from 'vue';

const props = defineProps({
  numberOfShots: Number,
  rateOfFire: Number,
  power: Number
});

const canvas = ref(null);
const ctx = ref(null);
const stars = ref([]);
const projectiles = ref([]);
const explosions = ref([]);
let animationFrameId = null;
let lastShotTime = 0;
// POSIÇÕES CORRIGIDAS: nave no lado esquerdo, meteoro no direito
const shipPosition = { x: 15, y: 50 }; // 15% da largura
const targetPosition = { x: 85, y: 50 }; // 85% da largura

// Configuração inicial
const setupCanvas = () => {
  ctx.value = canvas.value.getContext('2d');
  canvas.value.width = canvas.value.clientWidth;
  canvas.value.height = canvas.value.clientHeight;
  
  // Criar estrelas de fundo
  stars.value = [];
  for (let i = 0; i < 100; i++) {
    stars.value.push({
      x: Math.random() * canvas.value.width,
      y: Math.random() * canvas.value.height,
      size: Math.random() * 1.5,
      opacity: Math.random() * 0.8 + 0.2,
      speed: Math.random() * 0.05
    });
  }
};

// Cria novos projéteis baseado na taxa
const createProjectiles = () => {
  const newProjectiles = [];
  const baseY = canvas.value.height * (shipPosition.y / 100);
  const spacing = 20; // Espaçamento entre tiros
  
  for (let i = 0; i < props.numberOfShots; i++) {
    const penalty = i * 0.15; // Penalidade de 15% por tiro extra
    const damagePercent = Math.max(0, 1 - penalty);
    
    newProjectiles.push({
      x: canvas.value.width * (shipPosition.x / 100),
      y: baseY + (i - (props.numberOfShots - 1)/2) * spacing,
      startX: canvas.value.width * (shipPosition.x / 100),
      targetX: canvas.value.width * (targetPosition.x / 100),
      speed: 0.8 + props.rateOfFire * 0.03,
      size: 2 + props.power * 0.08,
      damagePercent,
      traveled: 0,
      distance: canvas.value.width * (targetPosition.x - shipPosition.x) / 100,
      color: `hsl(${200 * damagePercent}, 100%, 70%)`,
      trail: []
    });
  }
  
  return newProjectiles;
};

// Atualiza posições e estado
const updateProjectiles = () => {
  projectiles.value = projectiles.value.filter(proj => {
    // Atualizar posição
    proj.x += proj.speed * (proj.targetX - proj.startX) / proj.distance;
    proj.traveled += proj.speed;
    
    // Adicionar ponto de rastro (a cada 5px)
    if (proj.traveled % 5 < 1) {
      proj.trail.push({ x: proj.x, y: proj.y, size: proj.size, opacity: 1 });
    }
    
    // Atualizar rastro
    proj.trail = proj.trail.filter(point => {
      point.opacity -= 0.05;
      return point.opacity > 0;
    });
    
    // Verificar se atingiu o alvo
    if (proj.x >= proj.targetX) {
      createExplosion(proj.x, proj.y, proj.size * 2);
      return false;
    }
    
    return true;
  });
};

// Cria efeito de explosão
const createExplosion = (x, y, size) => {
  explosions.value.push({
    x,
    y,
    size: size * 0.5,
    maxSize: size * 3,
    growth: size * 0.2,
    opacity: 1,
    particles: Array.from({ length: 10 }, () => ({
      angle: Math.random() * Math.PI * 2,
      distance: Math.random() * size * 1.5,
      speed: Math.random() * 2 + 1,
      size: Math.random() * 3 + 1,
      opacity: 1
    }))
  });
};

// Atualiza explosões
const updateExplosions = () => {
  explosions.value = explosions.value.filter(explosion => {
    explosion.size += explosion.growth;
    explosion.opacity -= 0.02;
    
    // Atualizar partículas
    explosion.particles.forEach(p => {
      p.distance += p.speed;
      p.opacity -= 0.03;
    });
    
    return explosion.size < explosion.maxSize && explosion.opacity > 0;
  });
};

// Renderiza todos os elementos
const renderScene = () => {
  // Limpar canvas
  ctx.value.clearRect(0, 0, canvas.value.width, canvas.value.height);
  
  // Fundo gradiente
  const gradient = ctx.value.createLinearGradient(0, 0, 0, canvas.value.height);
  gradient.addColorStop(0, "#0f172a");
  gradient.addColorStop(1, "#1e293b");
  ctx.value.fillStyle = gradient;
  ctx.value.fillRect(0, 0, canvas.value.width, canvas.value.height);
  
  // Desenhar estrelas
  stars.value.forEach(star => {
    star.x -= star.speed;
    if (star.x < 0) star.x = canvas.value.width;
    
    ctx.value.beginPath();
    ctx.value.arc(star.x, star.y, star.size, 0, Math.PI * 2);
    ctx.value.fillStyle = `rgba(255, 255, 255, ${star.opacity})`;
    ctx.value.fill();
  });
  
  // Desenhar rastros de projéteis
  projectiles.value.forEach(proj => {
    proj.trail.forEach(point => {
      ctx.value.beginPath();
      ctx.value.arc(point.x, point.y, point.size, 0, Math.PI * 2);
      ctx.value.fillStyle = `rgba(100, 200, 255, ${point.opacity * 0.6})`;
      ctx.value.fill();
    });
  });
  
  // Desenhar projéteis
  projectiles.value.forEach(proj => {
    ctx.value.beginPath();
    ctx.value.arc(proj.x, proj.y, proj.size, 0, Math.PI * 2);
    
    // Gradiente do projétil
    const gradient = ctx.value.createRadialGradient(
      proj.x, proj.y, 0,
      proj.x, proj.y, proj.size
    );
    gradient.addColorStop(0, proj.color);
    gradient.addColorStop(1, "rgba(100, 200, 255, 0.3)");
    
    ctx.value.fillStyle = gradient;
    ctx.value.fill();
    
    // Brilho interno
    ctx.value.beginPath();
    ctx.value.arc(proj.x, proj.y, proj.size * 0.5, 0, Math.PI * 2);
    ctx.value.fillStyle = "rgba(255, 255, 255, 0.7)";
    ctx.value.fill();
  });
  
  // Desenhar explosões
  explosions.value.forEach(explosion => {
    // Anel de explosão
    ctx.value.beginPath();
    ctx.value.arc(explosion.x, explosion.y, explosion.size, 0, Math.PI * 2);
    ctx.value.strokeStyle = `rgba(255, 165, 0, ${explosion.opacity})`;
    ctx.value.lineWidth = 2;
    ctx.value.stroke();
    
    // Partículas
    explosion.particles.forEach(p => {
      if (p.opacity > 0) {
        const px = explosion.x + Math.cos(p.angle) * p.distance;
        const py = explosion.y + Math.sin(p.angle) * p.distance;
        
        ctx.value.beginPath();
        ctx.value.arc(px, py, p.size, 0, Math.PI * 2);
        ctx.value.fillStyle = `rgba(255, ${100 + Math.random() * 155}, 0, ${p.opacity})`;
        ctx.value.fill();
      }
    });
  });
  
  // Desenhar nave (agora no lado esquerdo)
  drawSpaceship();
  
  // Desenhar alvo (meteoro no lado direito)
  drawTarget();
};

// Desenha a nave espacial (reposicionada para esquerda)
const drawSpaceship = () => {
  const x = canvas.value.width * (shipPosition.x / 100);
  const y = canvas.value.height * (shipPosition.y / 100);
  const size = 30 + props.power * 0.2;
  
  // Corpo da nave (triângulo apontando para direita)
  ctx.value.fillStyle = "#3b82f6";
  ctx.value.beginPath();
  ctx.value.moveTo(x, y); // Ponta da nave
  ctx.value.lineTo(x - size, y - size / 2); 
  ctx.value.lineTo(x - size, y + size / 2);
  ctx.value.closePath();
  ctx.value.fill();
  
  // Detalhes (cabine)
  ctx.value.fillStyle = "#93c5fd";
  ctx.value.beginPath();
  ctx.value.arc(x - size / 3, y, size / 4, 0, Math.PI * 2);
  ctx.value.fill();
  
  // Propulsor (agora na esquerda)
  const engineSize = 10 + props.rateOfFire * 2;
  ctx.value.fillStyle = `rgba(255, ${100 + props.rateOfFire * 10}, 0, 0.8)`;
  ctx.value.beginPath();
  ctx.value.ellipse(x - size, y, engineSize, engineSize / 2, 0, Math.PI / 2, Math.PI * 1.5);
  ctx.value.fill();
};

// Desenha o alvo (meteoro reposicionado para direita)
const drawTarget = () => {
  const x = canvas.value.width * (targetPosition.x / 100);
  const y = canvas.value.height * (targetPosition.y / 100);
  const size = 30 + props.numberOfShots * 2;
  
  // Corpo do meteoro
  ctx.value.fillStyle = "#4b5563";
  ctx.value.beginPath();
  ctx.value.arc(x, y, size, 0, Math.PI * 2);
  ctx.value.fill();
  
  // Detalhes (crateras)
  ctx.value.fillStyle = "#6b7280";
  ctx.value.beginPath();
  ctx.value.arc(x - size / 3, y - size / 4, size / 4, 0, Math.PI * 2);
  ctx.value.fill();
  
  ctx.value.beginPath();
  ctx.value.arc(x + size / 4, y + size / 5, size / 5, 0, Math.PI * 2);
  ctx.value.fill();
  
  // Crateras de impacto
  explosions.value.forEach(() => {
    ctx.value.fillStyle = "rgba(220, 38, 38, 0.5)";
    ctx.value.beginPath();
    ctx.value.arc(
      x - size / 2 + Math.random() * size,
      y - size / 2 + Math.random() * size,
      size / 8,
      0,
      Math.PI * 2
    );
    ctx.value.fill();
  });
};

// Loop principal de animação
const animate = (timestamp) => {
  // Adicionar novos projéteis baseado na taxa
  if (props.rateOfFire > 0 && timestamp - lastShotTime > 1000 / props.rateOfFire) {
    projectiles.value.push(...createProjectiles());
    lastShotTime = timestamp;
  }
  
  updateProjectiles();
  updateExplosions();
  renderScene();
  animationFrameId = requestAnimationFrame(animate);
};

// Iniciar/reiniciar animação
const startAnimation = () => {
  if (animationFrameId) {
    cancelAnimationFrame(animationFrameId);
  }
  projectiles.value = [];
  explosions.value = [];
  lastShotTime = performance.now();
  animationFrameId = requestAnimationFrame(animate);
};

// Configuração inicial
onMounted(() => {
  setupCanvas();
  startAnimation();
  
  // Redimensionar quando a janela mudar
  window.addEventListener('resize', setupCanvas);
});

// Limpeza
onUnmounted(() => {
  if (animationFrameId) {
    cancelAnimationFrame(animationFrameId);
  }
  window.removeEventListener('resize', setupCanvas);
});

// Reiniciar animação quando parâmetros mudam
watch(() => [props.numberOfShots, props.rateOfFire, props.power], () => {
  setupCanvas();
  startAnimation();
});
</script>

<style scoped>
canvas {
  display: block;
  background: linear-gradient(to bottom, #0f172a, #1e293b);
}
</style>