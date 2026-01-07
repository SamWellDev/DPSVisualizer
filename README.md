# 🎮 Twitch Fighter

An interactive stream overlay that gamifies audience engagement through a real-time boss battle system. Viewers can boost the hero's power through subscriptions, follows, and donations.

![Vue.js](https://img.shields.io/badge/Vue.js-3.x-4FC08D?style=flat-square&logo=vue.js)
![Vite](https://img.shields.io/badge/Vite-5.x-646CFF?style=flat-square&logo=vite)
![Tailwind CSS](https://img.shields.io/badge/Tailwind-3.x-38B2AC?style=flat-square&logo=tailwind-css)
![Canvas API](https://img.shields.io/badge/Canvas-API-orange?style=flat-square)

## 🎯 Overview

This project transforms stream interactions into an engaging visual experience. A soldier character continuously attacks a monster, and the audience's actions directly affect combat stats:

| Event | Effect |
|-------|--------|
| **Follow** | +2% Critical Chance |
| **Sub Tier 1** | +0.5 Attack Speed |
| **Sub Tier 2** | +1.0 Attack Speed |
| **Sub Tier 3** | +1.5 Attack Speed + 10 ATK |
| **Donate $5** | +5 ATK |
| **Donate $10+** | +20 ATK + 15% Crit Damage |

## ✨ Features

- **Real-time Canvas Animations** - Smooth 60fps rendering with projectiles, muzzle flashes, and damage numbers
- **Progressive Difficulty** - Monster HP scales by 1.5x each wave
- **Monthly Progression** - Wave progress persists throughout the month, resetting on the 1st
- **Custom Sprites** - Support for custom hero and monster artwork
- **Overlay Mode** - Transparent background ready for OBS/StreamLabs integration

## 🛠️ Tech Stack

- **Vue 3** - Composition API with `<script setup>` syntax
- **Vite** - Lightning-fast development and build tooling
- **Tailwind CSS** - Utility-first styling with custom dark theme
- **HTML5 Canvas** - Custom rendering engine for game graphics
- **JavaScript ES6+** - Modern async patterns and reactive state

## 🚀 Getting Started

```bash
# Install dependencies
npm install

# Run development server
npm run dev

# Build for production
npm run build
```

## 🔮 Roadmap

### Phase 1: Configuration
- [ ] **Settings UI** - Configuration panel for all overlay options
- [ ] **Twitch OAuth Setup** - Connect Twitch account flow
- [ ] **Buff Customization** - Adjust ATK/SPD/Crit values per event type
- [ ] **Visual Settings** - Sprite positions, sizes, and overlay transparency

### Phase 2: Twitch Integration
- [ ] **Twitch EventSub** - Real-time follows, subs, and bits via WebSocket
- [ ] **Channel Points** - Custom rewards that trigger buffs
- [ ] **Chat Commands** - Optional commands for viewers

### Phase 3: Polish
- [ ] **Sound Effects** - Hit sounds, criticals, and wave completion
- [ ] **Multiple Monster Sprites** - Different enemies per wave range
- [ ] **Backend Persistence** - Save wave progress to database
- [ ] **Leaderboard** - Track highest waves across months

### Phase 4: Engagement
- [ ] **Shop** - In-app store for upgrades or cosmetics
- [ ] **Achievements** - Unlockable badges and milestones

## 📁 Project Structure

```
src/
├── App.vue                      # Main layout + event simulator
├── components/
│   └── VisualizationArea.vue    # Canvas game engine
├── main.js                      # Vue entry point
└── style.css                    # Global styles

public/
└── sprites/
    ├── hero_1.png               # Soldier sprite
    └── slime_1.png              # Monster sprite
```

## 🎨 Customization

### Adjust Gun Barrel Position (for custom sprites)
```javascript
// VisualizationArea.vue, lines 76-79
const gunBarrelOffset = {
  x: 50,   // Horizontal offset from sprite center
  y: -35   // Vertical offset (negative = up)
};
```

### Adjust Character Positions
```javascript
// Hero position (lines 82-84)
const hero = reactive({
  x: 20,    // % from left
  y: 60,    // % from top
  size: 150 // sprite size in pixels
});

// Monster position (lines 94-96)
const monster = reactive({
  x: 78,
  y: 55,
  size: 300
});
```

## 📄 License

MIT License - Feel free to use this project for your streams!

---

*Built with ❤️ for the streaming community*
