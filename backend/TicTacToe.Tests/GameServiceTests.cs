using TicTacToe.Api.Models;
using TicTacToe.Api.Services;
using Xunit;

namespace TicTacToe.Tests;

public class GameServiceTests
{
    private readonly ScoreboardService _scoreboardService;
    private readonly ComputerPlayerService _computerService;
    private readonly GameService _gameService;

    public GameServiceTests()
    {
        _scoreboardService = new ScoreboardService();
        _computerService = new ComputerPlayerService();
        _gameService = new GameService(_scoreboardService, _computerService);
    }

    [Fact]
    public void CreateGame_InitializesNewGameSession()
    {
        var game = _gameService.CreateGame(GameMode.TwoPlayer);

        Assert.NotEqual(Guid.Empty, game.Id);
        Assert.Equal("TwoPlayer", game.GameMode);
        Assert.Equal("X", game.CurrentPlayer);
        Assert.Equal("InProgress", game.Status);
        Assert.Null(game.Winner);
        Assert.Empty(game.WinningCells);
        Assert.Empty(game.MoveHistory);
        Assert.All(game.Board, cell => Assert.Null(cell));
        Assert.False(game.CanUndo);
    }

    [Fact]
    public void MakeMove_ValidMove_UpdatesBoardAndAlternatesTurn()
    {
        var game = _gameService.CreateGame(GameMode.TwoPlayer);

        var (success, error, state) = _gameService.MakeMove(game.Id, "X", 0, 0);

        Assert.True(success);
        Assert.Null(error);
        Assert.NotNull(state);
        Assert.Equal("X", state.Board[0]);
        Assert.Equal("O", state.CurrentPlayer);
        Assert.Single(state.MoveHistory);
        Assert.Equal(1, state.MoveHistory[0].MoveNumber);
        Assert.Equal("Row 1, Column 1", state.MoveHistory[0].PositionDisplay);
        Assert.True(state.CanUndo);
    }

    [Fact]
    public void MakeMove_InvalidTurn_RejectsMove()
    {
        var game = _gameService.CreateGame(GameMode.TwoPlayer);

        var (success, error, state) = _gameService.MakeMove(game.Id, "O", 0, 0);

        Assert.False(success);
        Assert.Contains("Invalid turn", error);
        Assert.Null(state);
    }

    [Fact]
    public void MakeMove_OutOfBounds_RejectsMove()
    {
        var game = _gameService.CreateGame(GameMode.TwoPlayer);

        var (success, error, state) = _gameService.MakeMove(game.Id, "X", 3, 0);

        Assert.False(success);
        Assert.Contains("out of bounds", error);
        Assert.Null(state);
    }

    [Fact]
    public void MakeMove_OccupiedCell_RejectsMove()
    {
        var game = _gameService.CreateGame(GameMode.TwoPlayer);

        _gameService.MakeMove(game.Id, "X", 0, 0);
        var (success, error, state) = _gameService.MakeMove(game.Id, "O", 0, 0);

        Assert.False(success);
        Assert.Contains("already occupied", error);
        Assert.Null(state);
    }

    [Fact]
    public void MakeMove_RowWin_DetectsWinnerAndUpdatesScoreboard()
    {
        var game = _gameService.CreateGame(GameMode.TwoPlayer);

        // Row 0 win for X:
        // X: (0,0), (0,1), (0,2)
        // O: (1,0), (1,1)
        _gameService.MakeMove(game.Id, "X", 0, 0);
        _gameService.MakeMove(game.Id, "O", 1, 0);
        _gameService.MakeMove(game.Id, "X", 0, 1);
        _gameService.MakeMove(game.Id, "O", 1, 1);
        var (success, _, state) = _gameService.MakeMove(game.Id, "X", 0, 2);

        Assert.True(success);
        Assert.NotNull(state);
        Assert.Equal("Won", state.Status);
        Assert.Equal("X", state.Winner);
        Assert.Equal(new[] { 0, 1, 2 }, state.WinningCells);
        Assert.Equal(1, state.Scoreboard.XWins);
        Assert.Equal(0, state.Scoreboard.OWins);
        Assert.False(state.CanUndo); // Option A: Undo disabled once completed
    }

    [Fact]
    public void MakeMove_ColumnWin_DetectsWinner()
    {
        var game = _gameService.CreateGame(GameMode.TwoPlayer);

        // Col 0 win for X:
        // X: (0,0), (1,0), (2,0)
        // O: (0,1), (1,1)
        _gameService.MakeMove(game.Id, "X", 0, 0);
        _gameService.MakeMove(game.Id, "O", 0, 1);
        _gameService.MakeMove(game.Id, "X", 1, 0);
        _gameService.MakeMove(game.Id, "O", 1, 1);
        var (success, _, state) = _gameService.MakeMove(game.Id, "X", 2, 0);

        Assert.True(success);
        Assert.NotNull(state);
        Assert.Equal("Won", state.Status);
        Assert.Equal("X", state.Winner);
        Assert.Equal(new[] { 0, 3, 6 }, state.WinningCells);
    }

    [Fact]
    public void MakeMove_DiagonalWin_DetectsWinner()
    {
        var game = _gameService.CreateGame(GameMode.TwoPlayer);

        // Diagonal \ win for X:
        // X: (0,0), (1,1), (2,2)
        // O: (0,1), (0,2)
        _gameService.MakeMove(game.Id, "X", 0, 0);
        _gameService.MakeMove(game.Id, "O", 0, 1);
        _gameService.MakeMove(game.Id, "X", 1, 1);
        _gameService.MakeMove(game.Id, "O", 0, 2);
        var (success, _, state) = _gameService.MakeMove(game.Id, "X", 2, 2);

        Assert.True(success);
        Assert.NotNull(state);
        Assert.Equal("Won", state.Status);
        Assert.Equal("X", state.Winner);
        Assert.Equal(new[] { 0, 4, 8 }, state.WinningCells);
    }

    [Fact]
    public void MakeMove_Draw_DetectsDrawAndUpdatesScoreboard()
    {
        var game = _gameService.CreateGame(GameMode.TwoPlayer);

        // Board layout for Draw:
        // X O X
        // X O O
        // O X X
        _gameService.MakeMove(game.Id, "X", 0, 0); // 0
        _gameService.MakeMove(game.Id, "O", 0, 1); // 1
        _gameService.MakeMove(game.Id, "X", 0, 2); // 2
        _gameService.MakeMove(game.Id, "O", 1, 1); // 4
        _gameService.MakeMove(game.Id, "X", 1, 0); // 3
        _gameService.MakeMove(game.Id, "O", 1, 2); // 5
        _gameService.MakeMove(game.Id, "X", 2, 1); // 7
        _gameService.MakeMove(game.Id, "O", 2, 0); // 6
        var (success, _, state) = _gameService.MakeMove(game.Id, "X", 2, 2); // 8

        Assert.True(success);
        Assert.NotNull(state);
        Assert.Equal("Draw", state.Status);
        Assert.Null(state.Winner);
        Assert.Empty(state.WinningCells);
        Assert.Equal(1, state.Scoreboard.Draws);
    }

    [Fact]
    public void MakeMove_AfterGameCompletion_RejectsMove()
    {
        var game = _gameService.CreateGame(GameMode.TwoPlayer);

        // Win for X
        _gameService.MakeMove(game.Id, "X", 0, 0);
        _gameService.MakeMove(game.Id, "O", 1, 0);
        _gameService.MakeMove(game.Id, "X", 0, 1);
        _gameService.MakeMove(game.Id, "O", 1, 1);
        _gameService.MakeMove(game.Id, "X", 0, 2); // Won

        // Try move after game completed
        var (success, error, _) = _gameService.MakeMove(game.Id, "O", 2, 2);

        Assert.False(success);
        Assert.Contains("already completed", error);
    }

    [Fact]
    public void ResetGame_ClearsBoardAndHistory_KeepsScoreboard()
    {
        var game = _gameService.CreateGame(GameMode.TwoPlayer);

        _gameService.MakeMove(game.Id, "X", 0, 0);
        _gameService.MakeMove(game.Id, "O", 1, 0);
        _gameService.MakeMove(game.Id, "X", 0, 1);
        _gameService.MakeMove(game.Id, "O", 1, 1);
        _gameService.MakeMove(game.Id, "X", 0, 2); // Won, score X: 1

        var (success, _, resetState) = _gameService.ResetGame(game.Id);

        Assert.True(success);
        Assert.NotNull(resetState);
        Assert.Equal("InProgress", resetState.Status);
        Assert.Equal("X", resetState.CurrentPlayer);
        Assert.Null(resetState.Winner);
        Assert.Empty(resetState.WinningCells);
        Assert.Empty(resetState.MoveHistory);
        Assert.All(resetState.Board, cell => Assert.Null(cell));
        Assert.Equal(1, resetState.Scoreboard.XWins); // Scoreboard unchanged!
    }

    [Fact]
    public void UndoMove_TwoPlayerMode_RemovesOnlyMostRecentMove()
    {
        var game = _gameService.CreateGame(GameMode.TwoPlayer);

        _gameService.MakeMove(game.Id, "X", 0, 0);
        _gameService.MakeMove(game.Id, "O", 1, 1);

        var (success, _, state) = _gameService.UndoMove(game.Id);

        Assert.True(success);
        Assert.NotNull(state);
        Assert.Null(state.Board[4]); // (1,1) cleared
        Assert.Equal("X", state.Board[0]); // (0,0) intact
        Assert.Equal("O", state.CurrentPlayer); // It is O's turn again
        Assert.Single(state.MoveHistory);
    }

    [Fact]
    public void UndoMove_ComputerMode_RemovesBothComputerAndHumanMoves()
    {
        var game = _gameService.CreateGame(GameMode.Computer);

        // Human plays X at (0, 0)
        var (_, _, afterHumanAndComputer) = _gameService.MakeMove(game.Id, "X", 0, 0);

        Assert.NotNull(afterHumanAndComputer);
        Assert.Equal(2, afterHumanAndComputer.MoveHistory.Count);
        Assert.Equal("X", afterHumanAndComputer.MoveHistory[0].Player);
        Assert.Equal("O", afterHumanAndComputer.MoveHistory[1].Player);

        // User clicks Undo
        var (success, _, undoneState) = _gameService.UndoMove(game.Id);

        Assert.True(success);
        Assert.NotNull(undoneState);
        Assert.Empty(undoneState.MoveHistory);
        Assert.All(undoneState.Board, cell => Assert.Null(cell));
        Assert.Equal("X", undoneState.CurrentPlayer);
        Assert.False(undoneState.CanUndo);
    }

    [Fact]
    public void UndoMove_OptionA_DisabledAfterGameCompletion()
    {
        var game = _gameService.CreateGame(GameMode.TwoPlayer);

        _gameService.MakeMove(game.Id, "X", 0, 0);
        _gameService.MakeMove(game.Id, "O", 1, 0);
        _gameService.MakeMove(game.Id, "X", 0, 1);
        _gameService.MakeMove(game.Id, "O", 1, 1);
        _gameService.MakeMove(game.Id, "X", 0, 2); // Won

        var (success, error, _) = _gameService.UndoMove(game.Id);

        Assert.False(success);
        Assert.Contains("not permitted once the game has concluded", error);
    }
}
