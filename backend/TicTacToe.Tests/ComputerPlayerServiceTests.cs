using TicTacToe.Api.Services;
using Xunit;

namespace TicTacToe.Tests;

public class ComputerPlayerServiceTests
{
    private readonly ComputerPlayerService _service = new();

    [Fact]
    public void DetermineBestMove_ReturnsNull_WhenBoardIsFull()
    {
        var board = new[] { "X", "O", "X", "X", "O", "O", "O", "X", "X" };
        var move = _service.DetermineBestMove(board);
        Assert.Null(move);
    }

    [Fact]
    public void DetermineBestMove_Priority1_PlaysWinningMoveForO()
    {
        // O has cells 0 and 1, cell 2 is empty -> O should play 2 to win!
        // Even though X could also win or center is open.
        var board = new string?[]
        {
            "O", "O", null,
            "X", "X", null,
            null, null, null
        };

        var move = _service.DetermineBestMove(board);
        Assert.Equal(2, move);
    }

    [Fact]
    public void DetermineBestMove_Priority2_BlocksWinningMoveForX()
    {
        // X has cells 3 and 4, cell 5 is empty -> O must block at 5!
        var board = new string?[]
        {
            "O", null, null,
            "X", "X", null,
            null, null, null
        };

        var move = _service.DetermineBestMove(board);
        Assert.Equal(5, move);
    }

    [Fact]
    public void DetermineBestMove_Priority3_TakesCenter_WhenAvailable()
    {
        // No immediate win or block, center (4) is empty
        var board = new string?[]
        {
            "X", null, null,
            null, null, null,
            null, null, null
        };

        var move = _service.DetermineBestMove(board);
        Assert.Equal(4, move);
    }

    [Fact]
    public void DetermineBestMove_Priority4_TakesCorner_WhenCenterOccupied()
    {
        // Center (4) occupied by X. Corner 0, 2, 6, 8 available.
        var board = new string?[]
        {
            null, null, null,
            null, "X", null,
            null, null, null
        };

        var move = _service.DetermineBestMove(board);
        Assert.Contains(move!.Value, new[] { 0, 2, 6, 8 });
    }

    [Fact]
    public void DetermineBestMove_Priority5_TakesAnyAvailableCell_WhenCenterAndCornersOccupied()
    {
        // Corners and center are all occupied
        var board = new string?[]
        {
            "X", null, "O",
            null, "X", null,
            "O", null, "X"
        };

        var move = _service.DetermineBestMove(board);
        Assert.Contains(move!.Value, new[] { 1, 3, 5, 7 });
    }
}

