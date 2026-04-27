# CHECKERBOARD GAME APP

CHECKERBOARD GAME APP is a modern web-based checkers game built with a Full-stack architecture using React for the user interface and .NET for the server-side game logic.

![alt text](/client/public/home.png)
![alt text](/client/public/game.png)

## 🚀 Core Technologies

### Frontend (Client)

- **React + TypeScript (Vite)**: Primary framework for a fast and type-safe UI.
- **TanStack Query (React Query)**: Server state management, caching, and API data synchronization.
- **Tailwind CSS**: Utility-first styling for a modern and responsive design.
- **Axios**: HTTP Client for backend communication.
- **Lucide React**: Lightweight and consistent icon set.

### Backend (Server)

- **.NET 10**: High-performance framework for building the Web API.
- **NSwag**: Swagger/OpenAPI integration for API documentation and testing.

## ✨ Key Features

1.  **Server Status Monitoring**: The home page automatically checks if the server is active before allowing the game to start.
2.  **Custom Player Names**: Supports name inputs for both White and Black players with form validation.
3.  **Real-time Game State**: The game board is directly synchronized with the server-side state.
4.  **Move Validation**: The backend validates every player move. Legal moves are highlighted in green on the board.
5.  **King Piece Support**: Pieces reaching the opponent's edge automatically become a 'King' (👑).
6.  **Demo Mode**: A special feature for testing that instantly sets up the board in specific scenarios.
7.  **Victory Logic**: Automatic winner detection accompanied by sound effects and an attractive victory modal.

## 🛠️ How to Run the Project

### Prerequisites

- [Node.js](https://nodejs.org/) (latest version recommended)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### 1. Running the Server (Backend)

Open a terminal in the server directory:

```bash
cd server
dotnet restore
dotnet run
```

The server will run by default on port `http://localhost:5015`. You can access the API documentation at `http://localhost:5015/swagger`.

### 2. Menjalankan Client (Frontend)

Open a terminal in the client directory:

```bash
cd client
npm install
npm run dev
```

Open browser and access `http://localhost:5173`.

## 📡 API Endpoints (v1)

| Method | Endpoint                  | Description                                          |
| :----- | :------------------------ | :--------------------------------------------------- |
| `GET`  | `/api/v1/game/status`     | Checking server health.                              |
| `GET`  | `/api/v1/game/state`      | Gets the current board state and player status.      |
| `POST` | `/api/v1/game/start`      | Initialize a new game with the player name.          |
| `POST` | `/api/v1/game/move`       | Sends a pawn move (from coordinate A to B).          |
| `POST` | `/api/v1/game/reset`      | Reset the board to its starting position.            |
| `POST` | `/api/v1/game/setup-demo` | Set up the board for demonstration/testing purposes. |
