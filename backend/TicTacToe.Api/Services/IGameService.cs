using TicTacToe.Api.Models;
using TicTacToe.Api.Models.Responses;

namespace TicTacToe.Api.Services;

public interface IGameService
{
    GameStateResponse CreateGame(GameMode mode);
    GameStateResponse? GetGame(Guid id);
    (bool Success, string? ErrorMessage, GameStateResponse? Response) MakeMove(Guid id, string player, int row, int col);
    (bool Success, string? ErrorMessage, GameStateResponse? Response) UndoMove(Guid id);
    (bool Success, string? ErrorMessage, GameStateResponse? Response) ResetGame(Guid id);
}

