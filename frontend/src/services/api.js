const API_BASE = import.meta.env.VITE_API_URL || '/api';

async function handleResponse(response) {
  if (!response.ok) {
    let errorDetail = 'An error occurred';
    try {
      const data = await response.json();
      errorDetail = data.message || data.title || JSON.stringify(data);
    } catch {
      errorDetail = await response.text();
    }
    throw new Error(errorDetail || `HTTP ${response.status}`);
  }
  return response.json();
}

export const api = {
  async createGame(gameMode = 'TwoPlayer') {
    const res = await fetch(`${API_BASE}/games`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ gameMode }),
    });
    return handleResponse(res);
  },

  async getGame(id) {
    const res = await fetch(`${API_BASE}/games/${id}`);
    return handleResponse(res);
  },

  async makeMove(id, player, row, col) {
    const res = await fetch(`${API_BASE}/games/${id}/moves`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ player, row, column: col }),
    });
    return handleResponse(res);
  },

  async undoMove(id) {
    const res = await fetch(`${API_BASE}/games/${id}/undo`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
    });
    return handleResponse(res);
  },

  async resetGame(id) {
    const res = await fetch(`${API_BASE}/games/${id}/reset`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
    });
    return handleResponse(res);
  },

  async getScoreboard() {
    const res = await fetch(`${API_BASE}/scoreboard`);
    return handleResponse(res);
  },

  async resetScoreboard() {
    const res = await fetch(`${API_BASE}/scoreboard/reset`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
    });
    return handleResponse(res);
  },
};
