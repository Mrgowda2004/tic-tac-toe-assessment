namespace TicTacToe.Api.Models;

public class GameSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string?[] Board { get; set; } = new string?[9];
    public string CurrentPlayer { get; set; } = "X";
    public GameMode GameMode { get; set; } = GameMode.TwoPlayer;
    public GameStatus Status { get; set; } = GameStatus.InProgress;
    public string? Winner { get; set; }
    public List<int> WinningCells { get; set; } = new();
    public List<MoveRecord> MoveHistory { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public void Reset()
    {
        Board = new string?[9];
        CurrentPlayer = "X";
        Status = GameStatus.InProgress;
        Winner = null;
        WinningCells.Clear();
        MoveHistory.Clear();
        UpdatedAt = DateTime.UtcNow;
    }
}

