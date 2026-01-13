# 🎮 Twitch Fighter

An interactive stream overlay that gamifies audience engagement through a real-time boss battle system. Viewers can boost the hero's power through subscriptions, follows, and donations.

![Vue.js](https://img.shields.io/badge/Vue.js-3.x-4FC08D?style=flat-square&logo=vue.js)
![Vite](https://img.shields.io/badge/Vite-5.x-646CFF?style=flat-square&logo=vite)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.x-512BD4?style=flat-square&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?style=flat-square&logo=postgresql)
![Tailwind CSS](https://img.shields.io/badge/Tailwind-3.x-38B2AC?style=flat-square&logo=tailwind-css)

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
| **Bits** | +1% Crit Damage per bit |

## ✨ Features

### Frontend (Vue 3)
- **Real-time Canvas Animations** - 60fps with projectiles and damage numbers
- **Global Rankings** - Leaderboard with Global/Friends toggle
- **Achievements System** - Unlockable badges
- **Live/Offline Toggle** - Manual stream control

### Backend (ASP.NET Core)
- **Twitch OAuth** - Login with Twitch
- **REST API** - User, Progress, Stats, Rankings endpoints
- **SignalR Hub** - Real-time event streaming
- **PostgreSQL** - Persistent data storage

## 🛠️ Tech Stack

| Layer | Technology |
|-------|------------|
| Frontend | Vue 3, Vite, Tailwind CSS, Canvas API |
| Backend | ASP.NET Core 9, SignalR, TwitchLib |
| Database | PostgreSQL with EF Core |

## 🚀 Getting Started

### Frontend
```bash
npm install
npm run dev
```

### Backend
```bash
cd TwitchFighter.API
dotnet ef database update
dotnet run
```
> API available at `http://localhost:5041`

## 📁 Project Structure

```
DPSVisualizer/
├── src/                          # Vue frontend
│   ├── views/                    # Pages
│   └── components/               # UI components
├── TwitchFighter.API/            # C# Backend
│   ├── Controllers/              # API endpoints
│   ├── Hubs/                     # SignalR
│   ├── Models/                   # Database entities
│   └── Data/                     # EF Core context
├── public/sprites/               # Game assets
└── IMPLEMENTATION_PLAN.md        # Backend roadmap
```

## 🔌 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/auth/login` | Twitch OAuth redirect |
| GET | `/api/auth/callback` | OAuth callback |
| GET | `/api/user/{id}` | Get user profile |
| GET | `/api/progress/{userId}` | Get wave progress |
| POST | `/api/progress/{userId}/defeat` | Record kill + achievements |
| POST | `/api/stats/{userId}/buff` | Apply event buff |
| GET | `/api/rankings/global` | Monthly leaderboard |

## 🎮 User Journey

### Complete Flow

```
  ┌──────────────────────────────────────────────────────────────────────────┐
  │                                                                          │
  │   1. LOGIN          2. DASHBOARD           3. LIVE              4. OBS   │
  │                                                                          │
  │   ┌─────────┐       ┌─────────────────┐    ┌────────────┐    ┌────────┐  │
  │   │ Login   │       │ 🔴 OFFLINE      │    │ 🟢 LIVE    │    │ Copy   │  │
  │   │  with   │──────▶│                 │───▶│            │───▶│  URL   │  │
  │   │ Twitch  │       │ [Test Follow]   │    │ Real       │    │  for   │  │
  │   └─────────┘       │ [Test Sub]      │    │ Twitch     │    │  OBS   │  │
  │                     │ [Test Bits]     │    │ Events     │    └────────┘  │
  │                     └─────────────────┘    └────────────┘                │
  │                                                                          │
  └──────────────────────────────────────────────────────────────────────────┘
```

### Step by Step

| Step | What Happens | Details |
|------|--------------|----------|
| **1. Login** | User clicks "Login with Twitch" | OAuth authenticates and saves user to database |
| **2. Dashboard** | Panel shows stream status | 🔴 OFFLINE or 🟢 LIVE |
| **3. OFFLINE** | Test mode | Grayscale visualization, test buttons work |
| **4. LIVE** | Live mode | Real Twitch events apply buffs |
| **5. Test** | Buttons simulate events | Follow, Sub, Bits → see buffs working |
| **6. Export** | Copy overlay URL | Add as Browser Source in OBS |

### Application Routes

| Route | Description |
|-------|-----------|
| `/` | Landing page with "Login with Twitch" button |
| `/dashboard` | Control panel (after login) |
| `/overlay?id=xxx` | Pure visualization for OBS (transparent, no UI) |

### How Does it Detect if Stream is LIVE?

The backend queries the Twitch API periodically:

```javascript
// GET https://api.twitch.tv/helix/streams?user_id=BROADCASTER_ID
// If data.length > 0 → LIVE
// If data.length == 0 → OFFLINE
```

### How Does it Work in OBS?

1. In Dashboard, click **"Copy Overlay URL"**
2. In OBS, add a **Browser Source**
3. Paste the URL: `http://localhost:3000/overlay?id=YOUR_ID`
4. Configure size (recommended: 1920x1080)
5. Enable **"Shutdown source when not visible"**

The overlay page has a transparent background, so only the hero, monster and effects are visible!

---

## 🔮 Roadmap

### ✅ Completed
- [x] Canvas battle system
- [x] Dashboard with Config/Test tabs
- [x] Global Rankings & Achievements
- [x] C# Backend with PostgreSQL
- [x] REST API (Auth, User, Progress, Stats, Rankings)
- [x] SignalR Hub for real-time events
- [x] Twitch OAuth flow (login + callback)
- [x] TwitchEventSubService with WebSocket connection
- [x] SignalR frontend integration
- [x] `gameconfig.json` - editable file for balancing
- [x] Dashboard loads config from backend
- [x] Isolated Test mode with immortal "Dummy" monster
- [x] `/overlay?id=xxx` page for OBS
- [x] Overlay sync with backend (polling every 10s)
- [x] Monster state persistence (wave + HP saved between sessions)
- [x] Auto-detect LIVE stream status via Twitch API
- [x] Reset Progress button in dashboard
- [x] HP base configurable (not hardcoded)

### 🏗️ Architecture (v1.0)

| Component | Role |
|-----------|------|
| **Overlay** | Real game, calls `recordDefeat`, saves to backend |
| **Dashboard Config** | Status panel, auto-sync every 10s, no game |
| **Dashboard Test** | Sandbox with immortal Dummy monster |

### 📋 Next Steps (Visual Improvements)
- [ ] Remove blob fallback and require actual sprites
- [ ] Generate new monster sprites (better quality)
- [ ] Add more hero skins
- [ ] Update monster HP values for balanced progression
- [ ] Add death/spawn animations
- [ ] Particle effects for critical hits
- [ ] Sound effects integration

### 🎨 Art & Assets
- [ ] Generate multiple monster sprites (Slime, Goblin, Orc, etc.)
- [ ] Create hero variations
- [ ] Add background variations
- [ ] Victory/defeat animations

## 📄 License

MIT License

---
*Built with ❤️ for the streaming community*
