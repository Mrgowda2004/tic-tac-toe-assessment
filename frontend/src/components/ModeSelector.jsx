import React from 'react';
import { Users, Bot } from 'lucide-react';

export default function ModeSelector({ currentMode, onSelectMode, disabled }) {
  return (
    <div className="mode-selector-wrapper">
      <div className="mode-selector-label">Game Mode:</div>
      <div className="mode-btn-group">
        <button
          type="button"
          className={`mode-btn ${currentMode === 'TwoPlayer' ? 'active' : ''}`}
          onClick={() => onSelectMode('TwoPlayer')}
          disabled={disabled}
        >
          <Users size={16} />
          <span>Two Player</span>
        </button>

        <button
          type="button"
          className={`mode-btn ${currentMode === 'Computer' ? 'active' : ''}`}
          onClick={() => onSelectMode('Computer')}
          disabled={disabled}
        >
          <Bot size={16} />
          <span>vs Computer</span>
        </button>
      </div>
    </div>
  );
}
