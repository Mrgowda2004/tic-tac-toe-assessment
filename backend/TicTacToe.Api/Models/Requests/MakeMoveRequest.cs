namespace TicTacToe.Api.Models.Requests;

public class MakeMoveRequest
{
    public string Player { get; set; } = string.Empty;
    public int? Row { get; set; }
    public int? Column { get; set; }
    public int? CellIndex { get; set; }
}

