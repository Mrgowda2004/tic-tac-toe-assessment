using TicTacToe.Api.Models;

namespace TicTacToe.Api.Services;

public class ScoreboardService : IScoreboardService
{
    private readonly object _lock = new();
    private int _xWins;
    private int _oWins;
    private int _draws;

    public Scoreboard GetScoreboard()
    {
        lock (_lock)
        {
            return new Scoreboard
            {
                XWins = _xWins,
                OWins = _oWins,
                Draws = _draws
            };
        }
    }

    public void RecordWin(string winner)
    {
        lock (_lock)
        {
            if (string.Equals(winner, "X", StringComparison.OrdinalIgnoreCase))
            {
                _xWins++;
            }
            else if (string.Equals(winner, "O", StringComparison.OrdinalIgnoreCase))
            {
                _oWins++;
            }
        }
    }

    public void RecordDraw()
    {
        lock (_lock)
        {
            _draws++;
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            _xWins = 0;
            _oWins = 0;
            _draws = 0;
        }
    }
}

