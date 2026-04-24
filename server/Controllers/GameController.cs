using CheckerboardGameApp.Dtos;
using CheckerboardGameApp.Enums;
using CheckerboardGameApp.Factories;
using CheckerboardGameApp.Interfaces;
using CheckerboardGameApp.Models;
using CheckerboardGameApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CheckerboardGameApp.Controllers;

[Route("api/v1/game")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly GameFactory _factory;

    public GameController(IGameService gameService, GameFactory factory)
    {
        _gameService = gameService;
        _factory = factory;
    }

    [HttpGet("status")]
    public ActionResult<StatusResponseDto> GetGameStatus()
    {
        return new StatusResponseDto
        {
            Message = "Game is running"
        };
    }

    [HttpGet("state")]
    public IActionResult GetGameState()
    {
        var response = new
        {
            Status = new
            {
                CurrentPlayer = _gameService.CurrentPlayerColor.ToString(),
                GameStatus = _gameService.Status,
                _gameService.Winner,
            },
            Board = new
            {
                Rows = 8,
                Cols = 8,
                Squares = _gameService.FlattenBoard()
            }
        };
        return Ok(response);
    }

    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult ResetGame()
    {
        try
        {
            // Jika kamu mau reset ke posisi awal standar:
            var newGame = _factory.CreateGame();

            // 2. Timpa data di Singleton service kita dengan data dari factory
            _gameService.LoadState(
                newGame.Board,
                Color.White, // Reset ke player putih
                GameStatus.Ongoing
            );

            return Ok(new
            {
                Message = "Game telah di-reset ke posisi awal standar.",
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Gagal mereset game: " + ex.Message });
        }
    }

    [HttpPost("demo")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult LoadDemoGame()
    {
        try
        {
            var demoBoard = _factory.CreateDemoGame();

            _gameService.LoadState(demoBoard, Color.Black, GameStatus.Ongoing);

            return Ok(new { Message = "Papan demo berhasil dimuat!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Gagal mereset game: " + ex.Message });
        }
    }

    [HttpPost("move")]
    public IActionResult DoMove([FromBody] MoveRequest move)
    {
        try
        {
            _gameService.DoMove(move.From, move.To);
            return Ok(new
            {
                Message = "Gerakan berhasil.",
                CurrentPlayer = _gameService.CurrentPlayerColor.ToString(),
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { ex.Message });
        }
    }

    // [HttpGet("state")]
    // public IActionResult GetGameState()
    // {
    //     return Ok(new
    //     {
    //         Status = new
    //         {
    //             CurrentPlayer = _game.CurrentPlayerColor.ToString(),
    //             GameStatus = _game.Status,
    //             Winner = _game.GetWinner()?.ToString(),
    //         },
    //         Board = new
    //         {
    //             Rows = 8,
    //             Cols = 8,
    //             Squares = _game.GetBoard()
    //         }
    //     });
    // }

    // [HttpGet("valid-moves/{x}/{y}")]
    // public IActionResult GetValidMoves(int x, int y)
    // {
    //     var valid = _game.GetValidMove(new Point(x, y));

    //     var response = new
    //     {
    //         from = new
    //         {
    //             x = x,
    //             y = y
    //         },
    //         valid
    //     };
    //     return Ok(response);
    // }


    // [HttpPost("move")]
    // public IActionResult DoMove([FromBody] MoveRequest move)
    // {
    //     try
    //     {
    //         _game.DoMove(move.From, move.To);
    //         return Ok(new
    //         {
    //             Message = "Gerakan berhasil.",
    //             CurrentPlayer = _game.CurrentPlayerColor.ToString(),
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(new { ex.Message });
    //     }
    // }

    // [HttpPost("reset")]
    // public IActionResult Reset()
    // {
    //     try
    //     {
    //         _game.ResetGame();

    //         return Ok(new
    //         {
    //             Message = "Game telah di-reset ke posisi awal.",
    //             CurrentPlayer = _game.CurrentPlayerColor.ToString()
    //         });
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(new { Message = "Gagal mereset game: " + ex.Message });
    //     }
    // }

    // [HttpPost("setup-demo")]
    // public IActionResult SetupDemoScenario()
    // {
    //     try
    //     {
    //         _game.InitializeDemoScenario();

    //         return Ok(new { Message = "Scenario demo berhasil dimuat. Putih siap menang dalam 1 langkah!" });
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(new { Message = ex.Message });
    //     }
    // }
}
