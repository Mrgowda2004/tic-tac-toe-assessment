using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Models.Requests;
using TicTacToe.Api.Models.Responses;
using TicTacToe.Api.Services;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(GameStateResponse), StatusCodes.Status201Created)]
    public ActionResult<GameStateResponse> CreateGame([FromBody] CreateGameRequest? request)
    {
        var mode = request?.GameMode ?? Models.GameMode.TwoPlayer;
        var game = _gameService.CreateGame(mode);
        return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GameStateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<GameStateResponse> GetGame(Guid id)
    {
        var game = _gameService.GetGame(id);
        if (game == null)
        {
            return NotFound(new { message = $"Game session '{id}' not found." });
        }
        return Ok(game);
    }

    [HttpPost("{id:guid}/moves")]
    [ProducesResponseType(typeof(GameStateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<GameStateResponse> MakeMove(Guid id, [FromBody] MakeMoveRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { message = "Move request cannot be empty." });
        }

        if (string.IsNullOrWhiteSpace(request.Player))
        {
            return BadRequest(new { message = "Player identifier ('X' or 'O') is required." });
        }

        int row;
        int col;

        if (request.Row.HasValue && request.Column.HasValue)
        {
            row = request.Row.Value;
            col = request.Column.Value;
        }
        else if (request.CellIndex.HasValue)
        {
            if (request.CellIndex.Value < 0 || request.CellIndex.Value > 8)
            {
                return BadRequest(new { message = "CellIndex must be between 0 and 8." });
            }
            row = request.CellIndex.Value / 3;
            col = request.CellIndex.Value % 3;
        }
        else
        {
            return BadRequest(new { message = "Either (Row and Column) or (CellIndex) must be supplied." });
        }

        var (success, errorMessage, response) = _gameService.MakeMove(id, request.Player.Trim().ToUpperInvariant(), row, col);
        if (!success)
        {
            if (errorMessage == "Game session not found.")
            {
                return NotFound(new { message = errorMessage });
            }
            return BadRequest(new { message = errorMessage });
        }

        return Ok(response);
    }

    [HttpPost("{id:guid}/undo")]
    [ProducesResponseType(typeof(GameStateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<GameStateResponse> UndoMove(Guid id)
    {
        var (success, errorMessage, response) = _gameService.UndoMove(id);
        if (!success)
        {
            if (errorMessage == "Game session not found.")
            {
                return NotFound(new { message = errorMessage });
            }
            return BadRequest(new { message = errorMessage });
        }

        return Ok(response);
    }

    [HttpPost("{id:guid}/reset")]
    [ProducesResponseType(typeof(GameStateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<GameStateResponse> ResetGame(Guid id)
    {
        var (success, errorMessage, response) = _gameService.ResetGame(id);
        if (!success)
        {
            return NotFound(new { message = errorMessage });
        }

        return Ok(response);
    }
}

