import React from 'react';
import { Award, RotateCcw } from 'lucide-react';

export default function Scoreboard({ scoreboard, onResetScoreboard, isResetting }) {
  const xWins = scoreboard?.xWins ?? scoreboard?.XWins ?? 0;
  const oWins = scoreboard?.oWins ?? scoreboard?.OWins ?? 0;
  const draws = scoreboard?.draws ?? scoreboard?.Draws ?? 0;
  const total = scoreboard?.totalGames ?? scoreboard?.TotalGames ?? xWins + oWins + draws;

  return (
    <div className="card scoreboard-card">
      <div className="card-header">
        <div className="card-title">
          <Award size={18} className="card-title-icon" />
          <span>Scoreboard</span>
        </div>
        <button
          type="button"
          className="btn-link"
          onClick={onResetScoreboard}
          disabled={isResetting || total === 0}
          title="Reset session scoreboard"
        >
          <RotateCcw size={14} />
          <span>Reset Scores</span>
        </button>
      </div>

      <div className="scoreboard-grid">
        <div className="score-box score-x">
          <div className="score-symbol">X</div>
          <div className="score-meta">
            <span className="score-label">Player X</span>
            <span className="score-value">{xWins}</span>
          </div>
        </div>

        <div className="score-box score-draws">
          <div className="score-symbol">―</div>
          <div className="score-meta">
            <span className="score-label">Draws</span>
            <span className="score-value">{draws}</span>
          </div>
        </div>

        <div className="score-box score-o">
          <div className="score-symbol">O</div>
          <div className="score-meta">
            <span className="score-label">Player O</span>
            <span className="score-value">{oWins}</span>
          </div>
        </div>
      </div>

      <div className="scoreboard-footer">
        <span>Total Matches: <strong>{total}</strong></span>
        <span className="badge-pill">Backend Managed</span>
      </div>
    </div>
  );
}
