namespace TicTacToe.Api.Models.Requests;

public class CreateGameRequest
{
    public GameMode GameMode { get; set; } = GameMode.TwoPlayer;
}

