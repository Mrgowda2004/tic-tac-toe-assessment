namespace TicTacToe.Api.Services;

public class ComputerPlayerService : IComputerPlayerService
{
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

    private static readonly int[] CornerIndices = new[] { 0, 2, 6, 8 };
    private const int CenterIndex = 4;

    public int? DetermineBestMove(string?[] board)
    {
        var availableCells = GetAvailableCells(board);
        if (availableCells.Count == 0)
        {
            return null;
        }

        // Priority 1: If O can win, play the winning move
        foreach (var cell in availableCells)
        {
            if (WouldWin(board, cell, "O"))
            {
                return cell;
            }
        }

        // Priority 2: If X can win next, block X
        foreach (var cell in availableCells)
        {
            if (WouldWin(board, cell, "X"))
            {
                return cell;
            }
        }

        // Priority 3: Take center if available
        if (board[CenterIndex] == null)
        {
            return CenterIndex;
        }

        // Priority 4: Take a corner if available
        foreach (var corner in CornerIndices)
        {
            if (board[corner] == null)
            {
                return corner;
            }
        }

        // Priority 5: Take any available cell
        return availableCells[0];
    }

    private static List<int> GetAvailableCells(string?[] board)
    {
        var available = new List<int>();
        for (int i = 0; i < board.Length; i++)
        {
            if (string.IsNullOrEmpty(board[i]))
            {
                available.Add(i);
            }
        }
        return available;
    }

    private static bool WouldWin(string?[] board, int cellIndex, string player)
    {
        var testBoard = (string?[])board.Clone();
        testBoard[cellIndex] = player;

        foreach (var combo in WinningCombinations)
        {
            if (testBoard[combo[0]] == player &&
                testBoard[combo[1]] == player &&
                testBoard[combo[2]] == player)
            {
                return true;
            }
        }

        return false;
    }
}
