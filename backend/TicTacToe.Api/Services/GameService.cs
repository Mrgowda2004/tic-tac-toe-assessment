using System.Collections.Concurrent;
using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Responses;

namespace TicTacToe.Api.Services;

public class GameService : IGameService
{
    private readonly ConcurrentDictionary<Guid, GameSession> _sessions = new();
    private readonly IScoreboardService _scoreboardService;
    private readonly IComputerPlayerService _computerPlayerService;

    private static readonly int[][] WinningCombinations = new int[][]
    {
        new[] { 0, 1, 2 }, // Row 1
        new[] { 3, 4, 5 }, // Row 2
        new[] { 6, 7, 8 }, // Row 3
        new[] { 0, 3, 6 }, // Col 1
        new[] { 1, 4, 7 }, // Col 2
        new[] { 2, 5, 8 }, // Col 3
        new[] { 0, 4, 8 }, // Diagonal \
        new[] { 2, 4, 6 }  // Diagonal /
    };

    public GameService(IScoreboardService scoreboardService, IComputerPlayerService computerPlayerService)
    {
        _scoreboardService = scoreboardService;
        _computerPlayerService = computerPlayerService;
    }

    public GameStateResponse CreateGame(GameMode mode)
    {
        var session = new GameSession
        {
            Id = Guid.NewGuid(),
            GameMode = mode,
            CurrentPlayer = "X",
            Status = GameStatus.InProgress
        };

        _sessions[session.Id] = session;
        return MapToResponse(session);
    }

    public GameStateResponse? GetGame(Guid id)
    {
        if (_sessions.TryGetValue(id, out var session))
        {
            lock (session)
            {
                return MapToResponse(session);
            }
        }
        return null;
    }

    public (bool Success, string? ErrorMessage, GameStateResponse? Response) MakeMove(Guid id, string player, int row, int col)
    {
        if (!_sessions.TryGetValue(id, out var session))
        {
            return (false, "Game session not found.", null);
        }

        lock (session)
        {
            if (session.Status != GameStatus.InProgress)
            {
                return (false, "Game is already completed. Reset the game to start over.", null);
            }

            if (!string.Equals(player, session.CurrentPlayer, StringComparison.OrdinalIgnoreCase))
            {
                return (false, $"Invalid turn. It is player {session.CurrentPlayer}'s turn.", null);
            }

            if (row < 0 || row > 2 || col < 0 || col > 2)
            {
                return (false, "Move position out of bounds (rows and columns must be between 0 and 2).", null);
            }

            int cellIndex = row * 3 + col;
            if (!string.IsNullOrEmpty(session.Board[cellIndex]))
            {
                return (false, $"Cell at row {row + 1}, column {col + 1} is already occupied.", null);
            }

            // Apply player move
            ApplyMove(session, player, row, col);

            // Check game outcome after player move
            var outcome = EvaluateBoard(session.Board);
            if (outcome.Winner != null)
            {
                session.Status = GameStatus.Won;
                session.Winner = outcome.Winner;
                session.WinningCells = outcome.WinningCells;
                _scoreboardService.RecordWin(outcome.Winner);
                return (true, null, MapToResponse(session));
            }
            else if (outcome.IsDraw)
            {
                session.Status = GameStatus.Draw;
                session.Winner = null;
                session.WinningCells.Clear();
                _scoreboardService.RecordDraw();
                return (true, null, MapToResponse(session));
            }

            // Game is still InProgress
            if (session.GameMode == GameMode.TwoPlayer)
            {
                session.CurrentPlayer = (session.CurrentPlayer == "X") ? "O" : "X";
            }
            else if (session.GameMode == GameMode.Computer)
            {
                // Computer plays automatically as 'O'
                int? computerMoveIndex = _computerPlayerService.DetermineBestMove(session.Board);
                if (computerMoveIndex.HasValue)
                {
                    int compRow = computerMoveIndex.Value / 3;
                    int compCol = computerMoveIndex.Value % 3;
                    ApplyMove(session, "O", compRow, compCol);

                    var compOutcome = EvaluateBoard(session.Board);
                    if (compOutcome.Winner != null)
                    {
                        session.Status = GameStatus.Won;
                        session.Winner = compOutcome.Winner;
                        session.WinningCells = compOutcome.WinningCells;
                        _scoreboardService.RecordWin(compOutcome.Winner);
                    }
                    else if (compOutcome.IsDraw)
                    {
                        session.Status = GameStatus.Draw;
                        session.Winner = null;
                        session.WinningCells.Clear();
                        _scoreboardService.RecordDraw();
                    }
                    else
                    {
                        // Turn returns to human player X
                        session.CurrentPlayer = "X";
                    }
                }
            }

            return (true, null, MapToResponse(session));
        }
    }

    public (bool Success, string? ErrorMessage, GameStateResponse? Response) UndoMove(Guid id)
    {
        if (!_sessions.TryGetValue(id, out var session))
        {
            return (false, "Game session not found.", null);
        }

        lock (session)
        {
            // Option A: Undo is disabled once the game is won or drawn
            if (session.Status != GameStatus.InProgress)
            {
                return (false, "Undo is not permitted once the game has concluded.", null);
            }

            if (session.MoveHistory.Count == 0)
            {
                return (false, "No moves available to undo.", null);
            }

            if (session.GameMode == GameMode.TwoPlayer)
            {
                var lastMove = session.MoveHistory[^1];
                session.MoveHistory.RemoveAt(session.MoveHistory.Count - 1);
                session.Board[lastMove.CellIndex] = null;
                session.CurrentPlayer = lastMove.Player;
            }
            else if (session.GameMode == GameMode.Computer)
            {
                // In computer mode, undo removes computer's last move and player's preceding move together
                if (session.MoveHistory.Count >= 2 && session.MoveHistory[^1].Player == "O")
                {
                    var compMove = session.MoveHistory[^1];
                    session.MoveHistory.RemoveAt(session.MoveHistory.Count - 1);
                    session.Board[compMove.CellIndex] = null;

                    var humanMove = session.MoveHistory[^1];
                    session.MoveHistory.RemoveAt(session.MoveHistory.Count - 1);
                    session.Board[humanMove.CellIndex] = null;

                    session.CurrentPlayer = "X";
                }
                else if (session.MoveHistory.Count >= 1)
                {
                    var singleMove = session.MoveHistory[^1];
                    session.MoveHistory.RemoveAt(session.MoveHistory.Count - 1);
                    session.Board[singleMove.CellIndex] = null;
                    session.CurrentPlayer = "X";
                }
            }

            // Game remains InProgress
            session.Status = GameStatus.InProgress;
            session.Winner = null;
            session.WinningCells.Clear();
            session.UpdatedAt = DateTime.UtcNow;

            return (true, null, MapToResponse(session));
        }
    }

    public (bool Success, string? ErrorMessage, GameStateResponse? Response) ResetGame(Guid id)
    {
        if (!_sessions.TryGetValue(id, out var session))
        {
            return (false, "Game session not found.", null);
        }

        lock (session)
        {
            session.Reset();
            return (true, null, MapToResponse(session));
        }
    }

    private static void ApplyMove(GameSession session, string player, int row, int col)
    {
        int cellIndex = row * 3 + col;
        session.Board[cellIndex] = player;
        session.MoveHistory.Add(new MoveRecord
        {
            MoveNumber = session.MoveHistory.Count + 1,
            Player = player,
            Row = row,
            Column = col,
            Timestamp = DateTime.UtcNow
        });
        session.UpdatedAt = DateTime.UtcNow;
    }

    private static (string? Winner, List<int> WinningCells, bool IsDraw) EvaluateBoard(string?[] board)
    {
        foreach (var combo in WinningCombinations)
        {
            var cell0 = board[combo[0]];
            var cell1 = board[combo[1]];
            var cell2 = board[combo[2]];

            if (!string.IsNullOrEmpty(cell0) && cell0 == cell1 && cell1 == cell2)
            {
                return (cell0, combo.ToList(), false);
            }
        }

        bool hasEmptyCell = board.Any(string.IsNullOrEmpty);
        return (null, new List<int>(), !hasEmptyCell);
    }

    private GameStateResponse MapToResponse(GameSession session)
    {
        return new GameStateResponse
        {
            Id = session.Id,
            Board = (string?[])session.Board.Clone(),
            CurrentPlayer = session.CurrentPlayer,
            GameMode = session.GameMode.ToString(),
            Status = session.Status.ToString(),
            Winner = session.Winner,
            WinningCells = new List<int>(session.WinningCells),
            MoveHistory = new List<MoveRecord>(session.MoveHistory),
            CanUndo = session.Status == GameStatus.InProgress && session.MoveHistory.Count > 0,
            Scoreboard = _scoreboardService.GetScoreboard()
        };
    }
}
