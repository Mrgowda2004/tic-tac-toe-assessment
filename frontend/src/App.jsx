import React, { useState, useEffect, useCallback } from 'react';
import { api } from './services/api';
import Board from './components/Board';
import GameStatusBanner from './components/GameStatusBanner';
import Scoreboard from './components/Scoreboard';
import MoveHistory from './components/MoveHistory';
import ModeSelector from './components/ModeSelector';
import GameControls from './components/GameControls';
import { Gamepad2, AlertCircle, RefreshCw } from 'lucide-react';
import './App.css';

export default function App() {
  const [gameId, setGameId] = useState(null);
  const [board, setBoard] = useState(Array(9).fill(null));
  const [currentPlayer, setCurrentPlayer] = useState('X');
  const [gameMode, setGameMode] = useState('TwoPlayer');
  const [status, setStatus] = useState('InProgress');
  const [winner, setWinner] = useState(null);
  const [winningCells, setWinningCells] = useState([]);
  const [moveHistory, setMoveHistory] = useState([]);
  const [canUndo, setCanUndo] = useState(false);
  const [scoreboard, setScoreboard] = useState({ xWins: 0, oWins: 0, draws: 0, totalGames: 0 });

  const [isLoading, setIsLoading] = useState(false);
  const [isResettingScoreboard, setIsResettingScoreboard] = useState(false);
  const [errorMessage, setErrorMessage] = useState(null);

  const applyGameState = useCallback((data) => {
    setGameId(data.id || data.Id);
    setBoard(data.board || data.Board || Array(9).fill(null));
    setCurrentPlayer(data.currentPlayer || data.CurrentPlayer || 'X');
    setGameMode(data.gameMode || data.GameMode || 'TwoPlayer');
    setStatus(data.status || data.Status || 'InProgress');
    setWinner(data.winner || data.Winner || null);
    setWinningCells(data.winningCells || data.WinningCells || []);
    setMoveHistory(data.moveHistory || data.MoveHistory || []);
    setCanUndo(Boolean(data.canUndo ?? data.CanUndo));
    if (data.scoreboard || data.Scoreboard) {
      setScoreboard(data.scoreboard || data.Scoreboard);
    }
  }, []);

  const initGame = useCallback(async (modeToUse = gameMode) => {
    setIsLoading(true);
    setErrorMessage(null);
    try {
      const data = await api.createGame(modeToUse);
      applyGameState(data);
    } catch (err) {
      setErrorMessage(`Failed to initialize game: ${err.message}. Ensure the .NET API is running on http://localhost:5000`);
    } finally {
      setIsLoading(false);
    }
  }, [gameMode, applyGameState]);

  useEffect(() => {
    initGame('TwoPlayer');
  }, []);

  const handleCellClick = async (row, col) => {
    if (!gameId || status !== 'InProgress' || isLoading) return;

    // Human player move
    setIsLoading(true);
    setErrorMessage(null);
    try {
      const data = await api.makeMove(gameId, currentPlayer, row, col);
      applyGameState(data);
    } catch (err) {
      setErrorMessage(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  const handleUndo = async () => {
    if (!gameId || !canUndo || isLoading) return;

    setIsLoading(true);
    setErrorMessage(null);
    try {
      const data = await api.undoMove(gameId);
      applyGameState(data);
    } catch (err) {
      setErrorMessage(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  const handleResetGame = async () => {
    if (!gameId || isLoading) return;

    setIsLoading(true);
    setErrorMessage(null);
    try {
      const data = await api.resetGame(gameId);
      applyGameState(data);
    } catch (err) {
      setErrorMessage(err.message);
    } finally {
      setIsLoading(false);
    }
  };

  const handleModeChange = async (newMode) => {
    if (newMode === gameMode) return;
    setGameMode(newMode);
    await initGame(newMode);
  };

  const handleResetScoreboard = async () => {
    setIsResettingScoreboard(true);
    setErrorMessage(null);
    try {
      const data = await api.resetScoreboard();
      setScoreboard(data);
    } catch (err) {
      setErrorMessage(`Failed to reset scoreboard: ${err.message}`);
    } finally {
      setIsResettingScoreboard(false);
    }
  };

  return (
    <div className="app-container">
      <header className="app-header">
        <div className="header-brand">
          <div className="logo-box">
            <Gamepad2 size={28} />
          </div>
          <div>
            <h1 className="header-title">Tic Tac Toe</h1>
            <p className="header-subtitle">Full-Stack React + .NET 8 Web API Assessment Solution</p>
          </div>
        </div>

        <ModeSelector
          currentMode={gameMode}
          onSelectMode={handleModeChange}
          disabled={isLoading}
        />
      </header>

      {errorMessage && (
        <div className="error-alert" role="alert">
          <AlertCircle size={20} className="error-icon" />
          <span className="error-text">{errorMessage}</span>
          <button
            type="button"
            className="btn-retry"
            onClick={() => initGame(gameMode)}
            title="Retry connecting"
          >
            <RefreshCw size={14} /> Retry
          </button>
        </div>
      )}

      <main className="game-layout">
        <section className="board-section">
          <GameStatusBanner
            status={status}
            winner={winner}
            currentPlayer={currentPlayer}
            gameMode={gameMode}
            isThinking={isLoading && gameMode === 'Computer'}
          />

          <Board
            board={board}
            winningCells={winningCells}
            onCellClick={handleCellClick}
            disabled={status !== 'InProgress' || isLoading}
            currentPlayer={currentPlayer}
          />

          <GameControls
            onUndo={handleUndo}
            onResetGame={handleResetGame}
            onNewGame={() => initGame(gameMode)}
            canUndo={canUndo}
            isLoading={isLoading}
            gameMode={gameMode}
          />
        </section>

        <aside className="sidebar-section">
          <Scoreboard
            scoreboard={scoreboard}
            onResetScoreboard={handleResetScoreboard}
            isResetting={isResettingScoreboard}
          />

          <MoveHistory moveHistory={moveHistory} />
        </aside>
      </main>

      <footer className="app-footer">
        <p>Built with React.js & .NET 8 Web API &bull; REST Contract Architecture &bull; In-Memory State</p>
      </footer>
    </div>
  );
}

