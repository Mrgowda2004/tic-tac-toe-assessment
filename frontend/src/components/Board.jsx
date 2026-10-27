import React from 'react';
import Cell from './Cell';

export default function Board({ board, winningCells, onCellClick, disabled, currentPlayer }) {
  return (
    <div className="board-wrapper">
      <div className="board-grid">
        {board.map((value, index) => {
          const isWinning = winningCells.includes(index);
          const row = Math.floor(index / 3);
          const col = index % 3;

          return (
            <Cell
              key={index}
              value={value}
              isWinning={isWinning}
              disabled={disabled}
              previewSymbol={currentPlayer}
              onClick={() => onCellClick(row, col, index)}
            />
          );
        })}
      </div>
    </div>
  );
}

