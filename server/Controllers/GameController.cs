using CheckerboardGameApp.Models;
using CheckerboardGameApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CheckerboardGameApp.Controllers;

[Route("api/v1/game")]
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
        return Ok(new { Message = "Welcome to CheckerboardGameApp" });
    }

    [HttpGet("state")]
    public IActionResult GetGameState()
    {
        return Ok(new
        {
            Status = new
            {
                CurrentPlayer = _game.CurrentPlayerColor.ToString(),
                GameStatus = _game.Status,
                Winner = _game.GetWinner()?.ToString(),
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
        var valid = _game.GetValidMove(new Point(x, y));

        var response = new
        {
            from = new
            {
                x = x,
                y = y
            },
            valid
        };
        return Ok(response);
    }


    [HttpPost("move")]
    public IActionResult DoMove([FromBody] MoveRequest move)
    {
        try
        {
            _game.DoMove(move.From, move.To);
            return Ok(new
            {
                Message = "Gerakan berhasil.",
                CurrentPlayer = _game.CurrentPlayerColor.ToString(),
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { ex.Message });
        }
    }

    [HttpPost("reset")]
    public IActionResult Reset()
    {
        try
        {
            _game.ResetGame();

            return Ok(new
            {
                Message = "Game telah di-reset ke posisi awal.",
                CurrentPlayer = _game.CurrentPlayerColor.ToString()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Gagal mereset game: " + ex.Message });
        }
    }
}
