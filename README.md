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

## 🔮 Roadmap

### ✅ Completed
- [x] Canvas battle system
- [x] Dashboard with Config/Test
- [x] Global Rankings & Achievements
- [x] Live/Offline toggle
- [x] C# Backend with PostgreSQL
- [x] REST API (Auth, User, Progress, Stats, Rankings)
- [x] SignalR Hub

### 🚧 Next Steps
- [ ] Twitch EventSub integration
- [ ] Connect Vue to SignalR
- [ ] Sound effects
- [ ] Multiple monster sprites

## 📄 License

MIT License

---
*Built with ❤️ for the streaming community*
