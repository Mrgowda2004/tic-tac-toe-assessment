namespace TicTacToe.Api.Services;

public interface IComputerPlayerService
{
    int? DetermineBestMove(string?[] board);
}
