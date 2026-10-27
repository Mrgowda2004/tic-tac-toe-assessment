import React from 'react';
import { Undo2, RotateCcw, PlusCircle } from 'lucide-react';

export default function GameControls({
  onUndo,
  onResetGame,
  onNewGame,
  canUndo,
  isLoading,
  gameMode,
}) {
  const undoTitle =
    gameMode === 'Computer'
      ? 'Undo removes both the computer move and your preceding move'
      : 'Undo removes the most recent move';

  return (
    <div className="game-controls">
      <button
        type="button"
        className="btn btn-secondary btn-undo"
        onClick={onUndo}
        disabled={!canUndo || isLoading}
        title={undoTitle}
      >
        <Undo2 size={18} />
        <span>Undo Last Move</span>
      </button>

      <button
        type="button"
        className="btn btn-secondary btn-reset"
        onClick={onResetGame}
        disabled={isLoading}
        title="Clear current board and moves (keeps scoreboard)"
      >
        <RotateCcw size={18} />
        <span>Reset Game</span>
      </button>

      <button
        type="button"
        className="btn btn-primary btn-new"
        onClick={onNewGame}
        disabled={isLoading}
        title="Start a fresh game session"
      >
        <PlusCircle size={18} />
        <span>New Session</span>
      </button>
    </div>
  );
}
