namespace TicTacToe.Api.Models.Responses;

public class GameStateResponse
{
    public Guid Id { get; set; }
    public string?[] Board { get; set; } = new string?[9];
    public string CurrentPlayer { get; set; } = string.Empty;
    public string GameMode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Winner { get; set; }
    public List<int> WinningCells { get; set; } = new();
    public List<MoveRecord> MoveHistory { get; set; } = new();
    public bool CanUndo { get; set; }
    public Scoreboard Scoreboard { get; set; } = new();
}

