# Twitch Fighter Backend - Implementation Plan

Backend using C# ASP.NET Core 9 with PostgreSQL, SignalR, and Twitch integration.

## Architecture

```
┌─────────────────┐     SignalR       ┌─────────────────┐     EventSub     ┌──────────┐
│   Vue Frontend  │◄─────────────────►│  C# Backend     │◄───────────────►│  Twitch  │
│   (Port 5173)   │                   │  (Port 5000)    │                  │   API    │
└─────────────────┘                   └────────┬────────┘                  └──────────┘
                                               │
                                               ▼
                                      ┌─────────────────┐
                                      │   PostgreSQL    │
                                      └─────────────────┘
```

---

## Progress Checklist

### ✅ Phase 1: Project Setup
- [x] Create ASP.NET Core project
- [x] Add NuGet packages (EF Core, TwitchLib, SignalR)
- [x] Configure PostgreSQL connection

### ✅ Phase 2: Database
- [x] User, Progress, Stats, Achievement models
- [x] AppDbContext with relationships
- [x] Initial migration applied
- [x] Achievement seeds

### ✅ Phase 3: SignalR
- [x] GameHub implementation
- [x] JoinGame/LeaveGame methods
- [x] CORS configured for Vue

### ✅ Phase 4: API Controllers
- [x] AuthController (OAuth login/callback)
- [x] UserController (profile endpoints)
- [x] ProgressController (wave, achievements)
- [x] StatsController (buff application)
- [x] RankingsController (leaderboards)

### 🚧 Phase 5: Twitch EventSub
- [ ] TwitchService (IHostedService)
- [ ] Subscribe to follow events
- [ ] Subscribe to subscription events
- [ ] Subscribe to bits events

### 🚧 Phase 6: Frontend Integration
- [ ] Install @microsoft/signalr
- [ ] Connect to GameHub
- [ ] Handle TwitchEvent messages
- [ ] Call API on monster defeat

---

## API Reference

### Authentication
| Endpoint | Description |
|----------|-------------|
| `GET /api/auth/login` | Redirect to Twitch |
| `GET /api/auth/callback` | Handle OAuth |
| `GET /api/auth/validate` | Validate token |

### User & Progress
| Endpoint | Description |
|----------|-------------|
| `GET /api/user/{id}` | User profile |
| `GET /api/progress/{userId}` | Wave progress |
| `POST /api/progress/{userId}/wave` | Update wave |
| `POST /api/progress/{userId}/defeat` | Record kill |

### Stats & Rankings
| Endpoint | Description |
|----------|-------------|
| `GET /api/stats/{userId}` | Current stats |
| `POST /api/stats/{userId}/buff` | Apply buff |
| `GET /api/rankings/global` | Leaderboard |
| `GET /api/rankings/rank/{userId}` | User rank |

---

## Running the Backend

```bash
cd TwitchFighter.API

# First time: apply migrations
dotnet ef database update

# Run the server
dotnet run
```

API available at: `http://localhost:5000`
SignalR Hub: `http://localhost:5000/gamehub`

---

## Configuration

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=twitchfighter;Username=postgres;Password=YOUR_PASSWORD"
  },
  "Twitch": {
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_SECRET",
    "RedirectUri": "http://localhost:5173/auth/callback"
  }
}
```

Get Twitch credentials at: https://dev.twitch.tv/console/apps
