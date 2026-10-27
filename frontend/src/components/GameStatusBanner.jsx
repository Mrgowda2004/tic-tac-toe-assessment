import React from 'react';
import { Trophy, Handshake, PlayCircle, Bot, Users } from 'lucide-react';

export default function GameStatusBanner({ status, winner, currentPlayer, gameMode, isThinking }) {
  const isComputer = gameMode === 'Computer';

  let bannerContent = null;
  let bannerClass = 'status-in-progress';

  if (status === 'Won') {
    bannerClass = 'status-won';
    const winnerLabel = isComputer
      ? winner === 'X'
        ? 'Human Player (X)'
        : 'Computer (O)'
      : `Player ${winner}`;

    bannerContent = (
      <div className="status-message">
        <Trophy className="status-icon win-icon" size={26} />
        <div>
          <span className="status-title">Winner Declared!</span>
          <span className="status-subtitle">🎉 Congratulations {winnerLabel} won the match!</span>
        </div>
      </div>
    );
  } else if (status === 'Draw') {
    bannerClass = 'status-draw';
    bannerContent = (
      <div className="status-message">
        <Handshake className="status-icon draw-icon" size={26} />
        <div>
          <span className="status-title">Game Drawn!</span>
          <span className="status-subtitle">All 9 squares filled with no winner. Well played!</span>
        </div>
      </div>
    );
  } else {
    // InProgress
    if (isThinking) {
      bannerContent = (
        <div className="status-message">
          <Bot className="status-icon thinking-icon spin-pulse" size={26} />
          <div>
            <span className="status-title">Computer Thinking...</span>
            <span className="status-subtitle">Evaluating winning combinations and strategic blocks</span>
          </div>
        </div>
      );
    } else {
      const turnLabel = isComputer
        ? currentPlayer === 'X'
          ? 'Your Turn (X)'
          : 'Computer Turn (O)'
        : `Player ${currentPlayer}'s Turn`;

      bannerContent = (
        <div className="status-message">
          <PlayCircle className="status-icon turn-icon" size={26} />
          <div>
            <span className="status-title">{turnLabel}</span>
            <span className="status-subtitle">Click an empty cell on the board to make your move</span>
          </div>
        </div>
      );
    }
  }

  return (
    <div className={`status-banner ${bannerClass}`}>
      <div className="banner-top">
        <div className="mode-badge">
          {isComputer ? (
            <>
              <Bot size={15} />
              <span>Computer Mode (Human: X vs AI: O)</span>
            </>
          ) : (
            <>
              <Users size={15} />
              <span>Two Player Mode (Local Pass & Play)</span>
            </>
          )}
        </div>
        <div className={`turn-indicator player-${currentPlayer.toLowerCase()}`}>
          Turn: <strong>{currentPlayer}</strong>
        </div>
      </div>
      {bannerContent}
    </div>
  );
}
