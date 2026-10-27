# Full-Stack Tic Tac Toe Application (React & .NET 8)

A production-grade, browser-based Tic Tac Toe application built with a **React.js** frontend and a **.NET 8 Web API** backend. The application features two-player local pass-and-play, a heuristic rule-based computer AI opponent, move history tracking, mode-aware undo functionality, session scoreboard persistence, and full unit test coverage.

---

## 🛠️ Tech Stack

| Layer | Technology | Details |
|---|---|---|
| **Frontend** | React 18.x (SPA) | Vite build tool, modern functional components, Hooks |
| **Icons & Styling** | Lucide React + CSS3 | Accessible, responsive dark-mode UI with smooth micro-interactions |
| **Backend** | .NET 8 Web API (C#) | RESTful API, Controller-based, Swagger/OpenAPI |
| **State Management** | Thread-Safe In-Memory | `ConcurrentDictionary`, thread locks, session-level scoreboard |
| **Testing** | xUnit | 22 comprehensive unit tests covering all game rules and edge cases |
| **Runtime & Tooling** | Node.js + .NET CLI | Zero external database requirement; turnkey local execution |

---

## ✨ Features Implemented

### 1. Game Board & Turns
- Standard **3 × 3 grid** with responsive layout.
- Clickable empty cells with hover ghost previews.
- Locked occupied cells displaying Player **X** or **O**.
- Clear status banner indicating whose turn it is.
- Invalid moves rejected by backend without advancing turns.

### 2. Win & Draw Detection
- Evaluates all 8 winning configurations:
  - 3 rows
  - 3 columns
  - 2 diagonals
- Visually highlights winning cells with glowing emerald pulse animations.
- Detects draw when all 9 cells are filled with no winner.
- Locks board upon game completion and updates backend scoreboard.

### 3. Move History Tracking
- Real-time tabular log displaying:
  - Move number
  - Player (`X` or `O`)
  - Grid position in human-readable coordinates (`Row X, Column Y`)
  - Chronological ordering

### 4. Mode-Aware Undo Functionality
- **Two Player Mode:** Reverts only the single most recent move and returns turn to that player.
- **Computer Mode:** Reverts both the computer's response (`O`) and the human player's preceding move (`X`) together, returning the turn to the human player.
- **Undo Policy (Clarification 2 — Option A):** Once a game concludes with a Win or Draw, Undo is disabled to maintain score finality and prevent score manipulation.

### 5. Session Scoreboard
- Tracks Player X Wins, Player O Wins, Draws, and Total Games.
- Scoreboard is managed and persisted on the backend across games.
- Resetting the board leaves the scoreboard intact.
- Includes a dedicated **Reset Scores** action.

### 6. Heuristic Computer Opponent (AI)
In "Play Against Computer" mode, human plays **X** and computer plays **O**. The computer makes automated moves following the exact prioritized strategy:
1. **Winning Move:** If `O` can win on this turn, play the winning move.
2. **Blocking Move:** If `X` can win on their next turn, block `X`.
3. **Center Cell:** Take center (`Row 2, Column 2` / index 4) if unoccupied.
4. **Corner Cell:** Take any available corner (`0, 2, 6, 8`).
5. **Remaining Cell:** Take any remaining available cell.

---

## 🚀 Quick Start Guide

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js (v18+) & npm](https://nodejs.org/)

### Option 1: One-Click Launch (Windows)
Double-click `run-all.bat` in the repository root. This opens two terminal windows:
1. .NET Backend API running at `http://localhost:5000`
2. React Frontend running at `http://localhost:3000`

### Option 2: Manual Launch

#### 1. Start Backend (.NET 8 Web API)
```bash
cd backend/TicTacToe.Api
dotnet run --launch-profile http
```
- API Endpoint: `http://localhost:5000`
- Swagger UI Documentation: `http://localhost:5000/swagger`

#### 2. Start Frontend (React + Vite)
```bash
cd frontend
npm install
npm run dev
```
- Frontend Web App: `http://localhost:3000`

---

## 🧪 Running Unit Tests

The test suite contains **22 xUnit unit tests** testing all functional requirements and edge cases.

```bash
cd backend/TicTacToe.Tests
dotnet test
```

### Test Coverage Highlights:
- **Move Validation:** Valid moves, occupied cell rejection, out-of-bounds rejection, wrong turn rejection.
- **Turn Alternation:** Two-player turn swapping, computer automatic turn triggers.
- **Win Detection:** Row 0/1/2 wins, Col 0/1/2 wins, Main diagonal win, Anti-diagonal win, winning cell indices tracking.
- **Draw Detection:** Full board with no winning combination.
- **Post-Game Restrictions:** Rejection of moves after game is won/drawn.
- **Undo Logic:**
  - Two-player single move undo and turn restore.
  - Computer mode dual move undo (clears computer move and human move together).
  - Option A: Rejection of undo once game is completed.
- **Computer AI Priority:**
  - Priority 1 (plays win for O over other moves)
  - Priority 2 (blocks X win over center/corners)
  - Priority 3 (takes center when open)
  - Priority 4 (takes corners when center occupied)
  - Priority 5 (takes remaining sides)
- **Scoreboard:** Win/draw increments, total matches calculation, reset scores.

---

## 📡 REST API Documentation

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/games` | Create a new game session (`gameMode`: `"TwoPlayer"` or `"Computer"`) |
| `GET` | `/api/games/{id}` | Get current game state, board, move history, and status |
| `POST` | `/api/games/{id}/moves` | Submit move with `player`, `row`, `column` (or `cellIndex`) |
| `POST` | `/api/games/{id}/undo` | Undo last move according to current game mode |
| `POST` | `/api/games/{id}/reset` | Reset current board and moves (keeps scoreboard intact) |
| `GET` | `/api/scoreboard` | Retrieve session scoreboard (`xWins`, `oWins`, `draws`, `totalGames`) |
| `POST` | `/api/scoreboard/reset`| Reset scoreboard counters to zero |

### Sample Response: `GET /api/games/{id}`
```json
{
  "id": "8034868c-8b13-4d98-b80f-21d2197e86a0",
  "board": ["X", null, null, null, "O", null, null, null, null],
  "currentPlayer": "X",
  "gameMode": "Computer",
  "status": "InProgress",
  "winner": null,
  "winningCells": [],
  "moveHistory": [
    {
      "moveNumber": 1,
      "player": "X",
      "row": 0,
      "column": 0,
      "cellIndex": 0,
      "positionDisplay": "Row 1, Column 1",
      "timestamp": "2026-10-27T07:15:00Z"
    },
    {
      "moveNumber": 2,
      "player": "O",
      "row": 1,
      "column": 1,
      "cellIndex": 4,
      "positionDisplay": "Row 2, Column 2",
      "timestamp": "2026-10-27T07:15:01Z"
    }
  ],
  "canUndo": true,
  "scoreboard": {
    "xWins": 0,
    "oWins": 0,
    "draws": 0,
    "totalGames": 0
  }
}
```

---

## 🧠 Architecture & Design Decisions

### 1. Backend as Single Source of Truth
- Game rules, turn validation, win checking, computer AI execution, move history, and scorekeeping all execute inside the backend service layer.
- The React frontend operates purely as a presentation and interaction layer, rendering the authoritative state returned by the REST endpoints.

### 2. Thread-Safe In-Memory Storage
- Session state is kept in a thread-safe `ConcurrentDictionary<Guid, GameSession>`.
- Session-level locks prevent race conditions during concurrent moves.
- Scoreboard counters are guarded by synchronization primitives.
- Storage is zero-config: reviewers and interview panelists can clone and run without database installation or connection string configuration.

### 3. Undo Strategy: Option A Implementation
- The requirement invited candidates to choose between **Option A (Disable Undo After Completion)** and **Option B (Allow Undo After Completion)**.
- **Option A was selected** because:
  1. It models standard competitive tournament rules where completed matches are finalized.
  2. It guarantees the scoreboard remains strictly monotonically consistent and cannot be artificially manipulated or rolled back.
  3. The `canUndo` property in the API response explicitly instructs the client UI when to disable the button.

### 4. Computer AI Implementation
- Built as an isolated `IComputerPlayerService` adhering to the Single Responsibility Principle.
- Pure heuristic decision tree fulfilling all 5 prioritized rules.
- Fully deterministic and unit tested with targeted board scenarios.

---

## 🤖 AI-Assisted Development Notes

As specified in the assessment guidelines, AI tools were leveraged to accelerate boilerplate creation, structure specifications, and generate comprehensive unit tests:

1. **Requirement Conversion:**
   - Translated assessment requirements into domain models (`GameSession`, `MoveRecord`, `Scoreboard`), state machines (`InProgress`, `Won`, `Draw`), and REST endpoint specifications.
   - Clarified technology selection: User explicitly required React.js frontend instead of Angular.

2. **Prompts & Generation:**
   - Prompted for .NET 8 controller-based architecture with clean dependency injection.
   - Prompted for 5-tier AI heuristic algorithm (`DetermineBestMove`).
   - Prompted for xUnit test matrix verifying win conditions, draw edge cases, invalid moves, and undo behaviors.

3. **Manual Changes & Code Review:**
   - Verified that Computer Mode Undo pops **both** the computer move and the human move in a single atomic transaction.
   - Adjusted CORS policies and Vite proxy configuration (`/api` proxy to port 5000) to ensure smooth local development without cross-origin friction.
   - Refined UI styling with accessible color contrast, cell hover preview ghost symbols, and winning cell highlight pulse animations.

---

## 📋 Assumptions & Limitations

- **Session Lifespan:** In-memory storage persists for the duration of the backend process. Restarting the backend resets active sessions and scores.
- **Multi-Tab Sessions:** Each new game generates a unique GUID session ID, allowing multiple simultaneous browser tabs to run independent games concurrently.

---

## 🔮 Future Improvements

1. **Persistent Database:** Swap in SQLite via Entity Framework Core for cross-process persistence.
2. **Online Multiplayer:** Add ASP.NET Core SignalR hub for real-time WebSocket communication between two remote players.
3. **Difficulty Levels:** Add configurable computer difficulties (Easy: Random, Medium: Current Heuristic, Hard: Minimax with alpha-beta pruning).

