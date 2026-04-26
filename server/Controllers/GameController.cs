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
    public ActionResult<MessageResponse> GetGameStatus()
    {
        return new MessageResponse
        {
            Message = "Game is running"
        };
    }

    [HttpPost("start")]
    public ActionResult<MessageResponse> StartGame([FromBody] GameStartRequest request)
    {
        if (string.IsNullOrEmpty(request.PlayerWhiteName) || string.IsNullOrEmpty(request.PlayerBlackName))
        {
            return BadRequest(new { Message = "Nama kedua pemain harus diisi!" });
        }
        if (request.PlayerWhiteName == request.PlayerBlackName)
        {
            return BadRequest(new { Message = "Nama kedua pemain tidak boleh sama!" });
        }

        _factory.CreateGame();

        _gameService.InitializePlayers(request.PlayerWhiteName, request.PlayerBlackName);

        return new MessageResponse
        {
            Message = $"Game dimulai! Putih: {request.PlayerWhiteName}, Hitam: {request.PlayerBlackName}"
        };
    }

    [HttpGet("state")]
    public IActionResult GetGameState()
    {
        if (_gameService.WhitePlayer == null || _gameService.BlackPlayer == null)
        {
            return BadRequest(new { Message = "Permainan belum dimulai. Silakan daftarkan pemain terlebih dahulu." });
        }

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
    public ActionResult<MessageResponse> ResetGame()
    {
        try
        {
            var newGame = _factory.CreateGame();

            _gameService.LoadState(
                newGame.Board,
                Color.White,
                GameStatus.Ongoing
            );

            _gameService.ResetPlayers();

            return new MessageResponse
            {
                Message = "Game telah di-reset ke posisi awal standar."
            };
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Gagal mereset game: " + ex.Message });
        }
    }

    [HttpPost("demo")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public ActionResult<MessageResponse> LoadDemoGame()
    {

        var demoBoard = _factory.CreateDemoGame();

        _gameService.LoadState(demoBoard, Color.Black, GameStatus.Ongoing);

        return new MessageResponse
        {
            Message = "Papan demo berhasil dimuat!"
        };

    }

    [HttpPost("move")]
    public ActionResult MakeMove([FromBody] MakeMoveRequest move)
    {
        var result = _gameService.MakeMove(move.From, move.To);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
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
