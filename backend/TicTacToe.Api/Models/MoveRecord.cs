namespace TicTacToe.Api.Models;

public class MoveRecord
{
    public int MoveNumber { get; set; }
    public string Player { get; set; } = string.Empty;
    public int Row { get; set; }
    public int Column { get; set; }
    public int CellIndex => Row * 3 + Column;
    public string PositionDisplay => $"Row {Row + 1}, Column {Column + 1}";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

