using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Models.Responses;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoreboardController : ControllerBase
{
    private readonly IScoreboardService _scoreboardService;

    public ScoreboardController(IScoreboardService scoreboardService)
    {
        _scoreboardService = scoreboardService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ScoreboardResponse), StatusCodes.Status200OK)]
    public ActionResult<ScoreboardResponse> GetScoreboard()
    {
        var scoreboard = _scoreboardService.GetScoreboard();
        return Ok(new ScoreboardResponse
        {
            XWins = scoreboard.XWins,
            OWins = scoreboard.OWins,
            Draws = scoreboard.Draws,
            TotalGames = scoreboard.TotalGames
        });
    }

    [HttpPost("reset")]
    [ProducesResponseType(typeof(ScoreboardResponse), StatusCodes.Status200OK)]
    public ActionResult<ScoreboardResponse> ResetScoreboard()
    {
        _scoreboardService.Reset();
        var scoreboard = _scoreboardService.GetScoreboard();
        return Ok(new ScoreboardResponse
        {
            XWins = scoreboard.XWins,
            OWins = scoreboard.OWins,
            Draws = scoreboard.Draws,
            TotalGames = scoreboard.TotalGames
        });
    }
}
