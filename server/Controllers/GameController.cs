using CheckerboardGameApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CheckerboardGameApp.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGame _game;

    public GameController(IGame game)
    {
        _game = game;
    }

    [HttpGet]
    public IActionResult GetBoard()
    {
        var board = _game.GetBoard();
        return Ok(board);
    }

    [HttpGet("state")]
    public IActionResult GetGameState()
    {
        return Ok(new
        {
            Status = new
            {
                CurrentPlayer = _game.CurrentPlayerColor.ToString(),
            },
            Board = new
            {
                Rows = 8,
                Cols = 8,
                Squares = _game.GetBoard()
            }
        });
    }

    [HttpGet("valid-moves/{x}/{y}")]
    public IActionResult GetValidMoves(int x, int y)
    {
        return Ok(new { x, y });
    }

    [HttpPost("move")]
    public IActionResult PostGame([FromBody] MoveRequest move)
    {
        return Ok(move);
    }

}
