using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services;

public interface IScoreboardService
{
    Scoreboard GetScoreboard();
    void RecordWin(string winner);
    void RecordDraw();
    void Reset();
}
