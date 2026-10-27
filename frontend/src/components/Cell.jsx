import React from 'react';

export default function Cell({ value, isWinning, onClick, disabled, previewSymbol }) {
  const isOccupied = Boolean(value);

  return (
    <button
      type="button"
      className={`board-cell ${isOccupied ? `occupied occupied-${value.toLowerCase()}` : 'empty'} ${
        isWinning ? 'winning' : ''
      }`}
      onClick={onClick}
      disabled={disabled || isOccupied}
      data-preview={!isOccupied && !disabled ? previewSymbol : undefined}
      aria-label={value ? `Cell with ${value}` : 'Empty cell'}
    >
      <span className="cell-content">{value}</span>
    </button>
  );
}
