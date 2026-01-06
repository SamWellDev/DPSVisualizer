<template>
  <div class="relative w-full h-full overflow-hidden bg-gray-900">
    <canvas ref="canvas" class="w-full h-full"></canvas>

    <!-- Stats Display -->
    <div class="absolute bottom-4 left-4 bg-black bg-opacity-70 rounded-lg p-3 text-sm text-white">
      <div class="font-mono flex flex-wrap gap-3">
        <div>
          <div class="text-cyan-400">ATK:</div>
          <div class="text-center">{{ heroStats.atk }}</div>
        </div>
        <div>
          <div class="text-green-400">SPD:</div>
          <div class="text-center">{{ heroStats.spd.toFixed(1) }}/s</div>
        </div>
        <div>
          <div class="text-yellow-400">CRIT%:</div>
          <div class="text-center">{{ heroStats.critChance }}%</div>
        </div>
        <div>
          <div class="text-orange-400">CRIT DMG:</div>
          <div class="text-center">{{ heroStats.critDamage }}%</div>
        </div>
      </div>
    </div>

    <!-- Monster HP Bar -->
    <div class="absolute top-4 right-4 bg-black bg-opacity-70 rounded-lg p-3 text-sm text-white min-w-[200px]">
      <div class="flex justify-between mb-1">
        <span class="text-red-400 font-bold">{{ monster.name }}</span>
        <span class="text-gray-400">Wave {{ monster.wave }}</span>
      </div>
      <div class="w-full bg-gray-700 rounded-full h-4 overflow-hidden">
        <div class="bg-gradient-to-r from-red-600 to-red-400 h-full transition-all duration-100"
          :style="{ width: (monster.currentHp / monster.maxHp * 100) + '%' }"></div>
      </div>
      <div class="text-center text-xs mt-1">
        {{ Math.max(0, Math.floor(monster.currentHp)) }} / {{ monster.maxHp }}
      </div>
    </div>

    <!-- Best Wave Display -->
    <div class="absolute top-4 left-4 bg-black bg-opacity-70 rounded-lg p-3 text-sm text-white">
      <div class="text-yellow-400 text-xs uppercase tracking-wide">🏆 Best Wave</div>
      <div class="text-2xl font-bold text-center">{{ bestWave }}</div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, onUnmounted, watch } from 'vue';

const props = defineProps({
  heroStats: {
    type: Object,
    default: () => ({ atk: 10, spd: 1.0, critChance: 5, critDamage: 150 })
  },
  bestWave: {
    type: Number,
    default: 1
  }
});

const emit = defineEmits(['monsterDefeated']);

const canvas = ref(null);
const ctx = ref(null);
const stars = ref([]);
const bullets = ref([]);
const muzzleFlashes = ref([]);
const damageNumbers = ref([]);
let animationFrameId = null;
let lastAttackTime = 0;

// ============================================
// AJUSTE AQUI A POSIÇÃO DO CANO DA ARMA
// Offset em pixels a partir do centro do sprite
// ============================================
const gunBarrelOffset = {
  x: 50,   // Positivo = direita do centro
  y: -35   // Negativo = acima do centro
};

// Estado do herói (soldado)
const hero = reactive({
  x: 20,           // Posição X em % do canvas
  y: 60,           // Posição Y em % do canvas
  size: 150,       // Tamanho do sprite
  idleOffset: 0,   // Offset de animação idle
  recoil: 0,       // Recuo ao atirar
  image: null,
  loaded: false
});

// Estado do monstro
const monster = reactive({
  name: "Slime",
  x: 78,
  y: 55,
  size: 300,
  maxHp: 140,
  currentHp: 100,
  wave: 1,
  hitFlash: 0,
  shakeOffset: 0,
  image: null,
  loaded: false
});

// Carregar sprite do soldado
const loadHeroSprite = () => {
  hero.image = new Image();
  hero.image.onload = () => {
    hero.loaded = true;
  };
  hero.image.src = '/sprites/hero_1.png';
};

// Carregar sprite do monstro
const loadMonsterSprite = () => {
  monster.image = new Image();
  monster.image.onload = () => {
    monster.loaded = true;
  };
  monster.image.src = '/sprites/slime_1.png';
};

// Configuração inicial do canvas
const setupCanvas = () => {
  ctx.value = canvas.value.getContext('2d');
  canvas.value.width = canvas.value.clientWidth;
  canvas.value.height = canvas.value.clientHeight;

  // Criar estrelas de fundo
  stars.value = [];
  for (let i = 0; i < 60; i++) {
    stars.value.push({
      x: Math.random() * canvas.value.width,
      y: Math.random() * canvas.value.height,
      size: Math.random() * 1.5,
      opacity: Math.random() * 0.4 + 0.1,
      speed: Math.random() * 0.02
    });
  }
};

// Calcular posição do cano da arma
const getGunBarrelPosition = () => {
  const heroX = canvas.value.width * (hero.x / 100);
  const heroY = canvas.value.height * (hero.y / 100) + hero.idleOffset;

  return {
    x: heroX + gunBarrelOffset.x - hero.recoil,
    y: heroY + gunBarrelOffset.y
  };
};

// Criar projétil (bala)
const createBullet = (isCrit) => {
  const barrel = getGunBarrelPosition();
  const targetX = canvas.value.width * (monster.x / 100);
  const targetY = canvas.value.height * (monster.y / 100);

  // Pequena variação no ângulo
  const spread = (Math.random() - 0.5) * 10;

  bullets.value.push({
    x: barrel.x,
    y: barrel.y + spread,
    startX: barrel.x,
    targetX: targetX,
    targetY: targetY + spread,
    speed: 15,
    size: isCrit ? 6 : 4,
    isCrit,
    trail: []
  });

  // Efeito de flash no cano
  muzzleFlashes.value.push({
    x: barrel.x + 10,
    y: barrel.y,
    size: isCrit ? 25 : 18,
    opacity: 1
  });

  // Recuo do soldado
  hero.recoil = 8;
};

// Criar número de dano flutuante
const createDamageNumber = (damage, isCrit, x, y) => {
  damageNumbers.value.push({
    x: x - 20 + Math.random() * 40,
    y: y - monster.size / 2,
    damage: Math.floor(damage),
    isCrit,
    opacity: 1,
    velocityY: -3,
    scale: isCrit ? 1.5 : 1
  });
};

// Executar ataque (disparar)
const performAttack = () => {
  const isCrit = Math.random() * 100 < props.heroStats.critChance;
  createBullet(isCrit);
};

// Atualizar balas
const updateBullets = () => {
  bullets.value = bullets.value.filter(bullet => {
    // Adicionar ao trail
    bullet.trail.push({ x: bullet.x, y: bullet.y, opacity: 1 });
    if (bullet.trail.length > 8) bullet.trail.shift();

    // Mover bala
    bullet.x += bullet.speed;

    // Verificar colisão com monstro
    const monsterX = canvas.value.width * (monster.x / 100);

    if (bullet.x >= monsterX - monster.size / 2) {
      // Calcular dano
      let damage = props.heroStats.atk;
      if (bullet.isCrit) {
        damage = damage * (props.heroStats.critDamage / 100);
      }

      // Aplicar dano
      monster.currentHp -= damage;
      monster.hitFlash = 1;
      monster.shakeOffset = 8;

      // Número de dano
      createDamageNumber(damage, bullet.isCrit, bullet.x, bullet.y);

      // Verificar morte
      if (monster.currentHp <= 0) {
        spawnNextMonster();
      }

      return false; // Remove a bala
    }

    return bullet.x < canvas.value.width;
  });

  // Atualizar trails
  bullets.value.forEach(bullet => {
    bullet.trail.forEach(point => {
      point.opacity -= 0.15;
    });
    bullet.trail = bullet.trail.filter(p => p.opacity > 0);
  });
};

// Spawnar próximo monstro
const spawnNextMonster = () => {
  monster.wave++;
  monster.maxHp = Math.floor(100 * Math.pow(1.5, monster.wave - 1));
  monster.currentHp = monster.maxHp;
  monster.name = getMonsterName(monster.wave);
  emit('monsterDefeated', monster.wave);
};

// Nome do monstro baseado na wave
const getMonsterName = (wave) => {
  const names = ["Slime", "Goblin", "Orc", "Troll", "Demon", "Dragon", "Titan", "Ancient One"];
  return names[Math.min(wave - 1, names.length - 1)];
};

// Atualizar animações
const updateAnimations = (timestamp) => {
  // Animação de idle (leve balanço)
  hero.idleOffset = Math.sin(timestamp / 600) * 2;

  // Diminuir recuo
  if (hero.recoil > 0) {
    hero.recoil *= 0.7;
    if (hero.recoil < 0.5) hero.recoil = 0;
  }

  // Verificar se deve atirar
  const attackInterval = 1000 / props.heroStats.spd;
  if (timestamp - lastAttackTime >= attackInterval) {
    performAttack();
    lastAttackTime = timestamp;
  }

  // Atualizar balas
  updateBullets();

  // Atualizar muzzle flashes
  muzzleFlashes.value = muzzleFlashes.value.filter(flash => {
    flash.opacity -= 0.2;
    flash.size *= 0.9;
    return flash.opacity > 0;
  });

  // Atualizar números de dano
  damageNumbers.value = damageNumbers.value.filter(num => {
    num.y += num.velocityY;
    num.velocityY += 0.1; // Gravidade leve
    num.opacity -= 0.015;
    return num.opacity > 0;
  });

  // Atualizar efeitos do monstro
  if (monster.hitFlash > 0) monster.hitFlash -= 0.15;
  if (monster.shakeOffset > 0) monster.shakeOffset *= 0.85;
};

// Renderizar cena
const renderScene = () => {
  const c = ctx.value;
  const w = canvas.value.width;
  const h = canvas.value.height;

  // Limpar canvas
  c.clearRect(0, 0, w, h);

  // Fundo gradiente
  const gradient = c.createLinearGradient(0, 0, 0, h);
  gradient.addColorStop(0, "#0a0a15");
  gradient.addColorStop(0.7, "#1a1a2e");
  gradient.addColorStop(1, "#2d2d44");
  c.fillStyle = gradient;
  c.fillRect(0, 0, w, h);

  // Desenhar estrelas
  stars.value.forEach(star => {
    star.x -= star.speed;
    if (star.x < 0) star.x = w;

    c.beginPath();
    c.arc(star.x, star.y, star.size, 0, Math.PI * 2);
    c.fillStyle = `rgba(255, 255, 255, ${star.opacity})`;
    c.fill();
  });

  // Desenhar chão
  const groundGradient = c.createLinearGradient(0, h * 0.7, 0, h);
  groundGradient.addColorStop(0, "#2d3748");
  groundGradient.addColorStop(1, "#1a202c");
  c.fillStyle = groundGradient;
  c.fillRect(0, h * 0.7, w, h * 0.3);

  // Desenhar monstro
  drawMonster();

  // Desenhar trails das balas
  drawBulletTrails();

  // Desenhar balas
  drawBullets();

  // Desenhar muzzle flashes
  drawMuzzleFlashes();

  // Desenhar herói
  drawHero();

  // Desenhar números de dano
  drawDamageNumbers();
};

// Desenhar herói (soldado)
const drawHero = () => {
  const c = ctx.value;
  const x = canvas.value.width * (hero.x / 100) - hero.recoil;
  const y = canvas.value.height * (hero.y / 100) + hero.idleOffset;
  const size = hero.size;

  c.save();
  c.translate(x, y);

  // Sombra do herói
  c.fillStyle = "rgba(0, 0, 0, 0.3)";
  c.beginPath();
  c.ellipse(-30, size * 0.5, size * 0.35, size * 0.1, 0, 0, Math.PI * 2);
  c.fill();

  if (hero.loaded && hero.image) {
    c.drawImage(hero.image, -size / 2, -size / 2, size, size);
  } else {
    // Placeholder
    c.fillStyle = "#4a9eff";
    c.fillRect(-40, -60, 80, 120);
    c.fillStyle = "#fff";
    c.font = "12px Arial";
    c.textAlign = "center";
    c.fillText("Loading...", 0, 0);
  }

  c.restore();
};

// Desenhar monstro
const drawMonster = () => {
  const c = ctx.value;
  const shake = (Math.random() - 0.5) * monster.shakeOffset;
  const x = canvas.value.width * (monster.x / 100) + shake;
  const y = canvas.value.height * (monster.y / 100);
  const size = monster.size;

  c.save();
  c.translate(x, y);

  // Sombra
  c.fillStyle = "rgba(0, 0, 0, 0.3)";
  c.beginPath();
  c.ellipse(0, size * 0.4, size * 0.5, size * 0.15, 0, 0, Math.PI * 2);
  c.fill();

  if (monster.loaded && monster.image) {
    // Usar sprite
    c.drawImage(monster.image, -size / 2, -size / 2, size, size);
  } else {
    // Fallback: desenhar slime programaticamente
    const bodyColor = monster.hitFlash > 0 ?
      `rgb(${Math.min(255, 100 + 155 * monster.hitFlash)}, ${74 - 74 * monster.hitFlash}, ${120 - 70 * monster.hitFlash})` :
      "#4a7c59";

    c.fillStyle = bodyColor;
    c.beginPath();
    c.ellipse(0, 0, size * 0.8, size * 0.6, 0, 0, Math.PI * 2);
    c.fill();

    // Olhos
    c.fillStyle = "#1a1a1a";
    c.beginPath();
    c.ellipse(-size * 0.25, -size * 0.1, size * 0.1, size * 0.15, 0, 0, Math.PI * 2);
    c.fill();
    c.beginPath();
    c.ellipse(size * 0.25, -size * 0.1, size * 0.1, size * 0.15, 0, 0, Math.PI * 2);
    c.fill();
  }

  c.restore();
};

// Desenhar trails das balas
const drawBulletTrails = () => {
  const c = ctx.value;

  bullets.value.forEach(bullet => {
    bullet.trail.forEach((point, i) => {
      if (point.opacity <= 0) return;

      c.beginPath();
      c.arc(point.x, point.y, bullet.size * 0.5, 0, Math.PI * 2);

      const color = bullet.isCrit ?
        `rgba(255, 200, 50, ${point.opacity * 0.6})` :
        `rgba(255, 150, 50, ${point.opacity * 0.5})`;

      c.fillStyle = color;
      c.fill();
    });
  });
};

// Desenhar balas
const drawBullets = () => {
  const c = ctx.value;

  bullets.value.forEach(bullet => {
    c.save();
    c.translate(bullet.x, bullet.y);

    // Glow
    const glowSize = bullet.size * 2;
    const glow = c.createRadialGradient(0, 0, 0, 0, 0, glowSize);
    if (bullet.isCrit) {
      glow.addColorStop(0, "rgba(255, 220, 100, 0.8)");
      glow.addColorStop(0.5, "rgba(255, 150, 50, 0.3)");
      glow.addColorStop(1, "rgba(255, 100, 0, 0)");
    } else {
      glow.addColorStop(0, "rgba(255, 200, 100, 0.7)");
      glow.addColorStop(0.5, "rgba(255, 100, 50, 0.2)");
      glow.addColorStop(1, "rgba(255, 50, 0, 0)");
    }

    c.fillStyle = glow;
    c.beginPath();
    c.arc(0, 0, glowSize, 0, Math.PI * 2);
    c.fill();

    // Core da bala
    c.fillStyle = bullet.isCrit ? "#fff5cc" : "#ffeecc";
    c.beginPath();
    c.arc(0, 0, bullet.size * 0.6, 0, Math.PI * 2);
    c.fill();

    c.restore();
  });
};

// Desenhar muzzle flashes
const drawMuzzleFlashes = () => {
  const c = ctx.value;

  muzzleFlashes.value.forEach(flash => {
    c.save();
    c.translate(flash.x, flash.y);

    // Flash de luz
    const gradient = c.createRadialGradient(0, 0, 0, 0, 0, flash.size);
    gradient.addColorStop(0, `rgba(255, 255, 200, ${flash.opacity})`);
    gradient.addColorStop(0.3, `rgba(255, 200, 100, ${flash.opacity * 0.7})`);
    gradient.addColorStop(1, `rgba(255, 100, 0, 0)`);

    c.fillStyle = gradient;
    c.beginPath();
    c.arc(0, 0, flash.size, 0, Math.PI * 2);
    c.fill();

    c.restore();
  });
};

// Desenhar números de dano
const drawDamageNumbers = () => {
  const c = ctx.value;

  damageNumbers.value.forEach(num => {
    c.save();

    const fontSize = num.isCrit ? 26 : 18;
    c.font = `bold ${fontSize * num.scale}px Arial`;
    c.textAlign = 'center';

    // Outline
    c.strokeStyle = 'rgba(0, 0, 0, 0.8)';
    c.lineWidth = 3;

    // Cor do texto
    c.fillStyle = num.isCrit ?
      `rgba(255, 220, 50, ${num.opacity})` :
      `rgba(255, 255, 255, ${num.opacity})`;

    const text = num.isCrit ? `${num.damage}!` : `${num.damage}`;

    c.strokeText(text, num.x, num.y);
    c.fillText(text, num.x, num.y);

    c.restore();
  });
};

// Loop de animação
const animate = (timestamp) => {
  updateAnimations(timestamp);
  renderScene();
  animationFrameId = requestAnimationFrame(animate);
};

// Lifecycle
onMounted(() => {
  loadHeroSprite();
  loadMonsterSprite();
  setupCanvas();
  animationFrameId = requestAnimationFrame(animate);

  window.addEventListener('resize', setupCanvas);
});

onUnmounted(() => {
  if (animationFrameId) {
    cancelAnimationFrame(animationFrameId);
  }
  window.removeEventListener('resize', setupCanvas);
});

// Watch para mudanças
watch(() => props.heroStats, () => {
  // Podemos adicionar efeitos quando stats mudam
}, { deep: true });
</script>

<style scoped>
canvas {
  display: block;
}
</style>