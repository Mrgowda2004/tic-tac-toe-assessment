using TicTacToe.Api.Services;
using Xunit;

namespace TicTacToe.Tests;

public class ScoreboardServiceTests
{
    [Fact]
    public void Scoreboard_TracksWinsAndDrawsCorrectly()
    {
        var scoreboardService = new ScoreboardService();

        scoreboardService.RecordWin("X");
        scoreboardService.RecordWin("X");
        scoreboardService.RecordWin("O");
        scoreboardService.RecordDraw();

        var scoreboard = scoreboardService.GetScoreboard();
        Assert.Equal(2, scoreboard.XWins);
        Assert.Equal(1, scoreboard.OWins);
        Assert.Equal(1, scoreboard.Draws);
        Assert.Equal(4, scoreboard.TotalGames);
    }

    [Fact]
    public void Reset_ClearsAllScores()
    {
        var scoreboardService = new ScoreboardService();
        scoreboardService.RecordWin("X");
        scoreboardService.RecordWin("O");
        scoreboardService.RecordDraw();

        scoreboardService.Reset();

        var scoreboard = scoreboardService.GetScoreboard();
        Assert.Equal(0, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
        Assert.Equal(0, scoreboard.TotalGames);
    }
}
