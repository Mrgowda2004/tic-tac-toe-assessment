import React from 'react';
import { History, Clock } from 'lucide-react';

export default function MoveHistory({ moveHistory }) {
  const moves = moveHistory || [];

  return (
    <div className="card move-history-card">
      <div className="card-header">
        <div className="card-title">
          <History size={18} className="card-title-icon" />
          <span>Move History</span>
        </div>
        <span className="badge-pill">{moves.length} {moves.length === 1 ? 'Move' : 'Moves'}</span>
      </div>

      <div className="table-container">
        {moves.length === 0 ? (
          <div className="empty-history">
            <Clock size={28} className="empty-icon" />
            <p>No moves yet in this session.</p>
            <span>Click any grid square above to make the opening move!</span>
          </div>
        ) : (
          <table className="history-table">
            <thead>
              <tr>
                <th style={{ width: '15%' }}>#</th>
                <th style={{ width: '25%' }}>Player</th>
                <th style={{ width: '60%' }}>Position</th>
              </tr>
            </thead>
            <tbody>
              {moves.map((move, idx) => {
                const moveNum = move.moveNumber ?? idx + 1;
                const player = move.player;
                const row = (move.row !== undefined ? move.row : Math.floor(move.cellIndex / 3)) + 1;
                const col = (move.column !== undefined ? move.column : move.cellIndex % 3) + 1;
                const position = move.positionDisplay || `Row ${row}, Column ${col}`;

                return (
                  <tr key={idx} className={`history-row player-${player.toLowerCase()}-row`}>
                    <td className="col-num">{moveNum}</td>
                    <td className="col-player">
                      <span className={`player-badge badge-${player.toLowerCase()}`}>
                        {player}
                      </span>
                    </td>
                    <td className="col-pos">{position}</td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}

