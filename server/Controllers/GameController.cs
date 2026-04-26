using CheckerboardGameApp.Dtos;
using CheckerboardGameApp.Enums;
using CheckerboardGameApp.Factories;
using CheckerboardGameApp.Interfaces;
using CheckerboardGameApp.Models;
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
            Message = "Game is running",
            Status = "OK"
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
                _gameService.WhitePlayer,
                _gameService.BlackPlayer,
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
            if (_gameService.WhitePlayer == null || _gameService.BlackPlayer == null)
            {
                return BadRequest(new { Message = "Permainan belum dimulai. Silakan daftarkan pemain terlebih dahulu." });
            }

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
        if (_gameService.WhitePlayer == null || _gameService.BlackPlayer == null)
        {
            return BadRequest(new { Message = "Permainan belum dimulai. Silakan daftarkan pemain terlebih dahulu." });
        }
        var demoBoard = _factory.CreateDemoGame();

        _gameService.LoadState(demoBoard, Color.Black, GameStatus.Ongoing);

        return new MessageResponse
        {
            Message = "Papan demo berhasil dimuat!"
        };

    }

    [HttpGet("valid-moves/{x}/{y}")]
    public ActionResult<ValidMoveResponse> GetValidMoves(int x, int y)
    {
        var fromPoint = new Point(x, y);
        var options = _gameService.GetValidMove(fromPoint);

        return new ValidMoveResponse
        {
            From = fromPoint,
            Valid = options
        };
    }

    [HttpPost("move")]
    public ActionResult<MakeMoveResponse> MakeMove([FromBody] MakeMoveRequest move)
    {
        if (_gameService.WhitePlayer == null || _gameService.BlackPlayer == null)
        {
            return BadRequest(new { Message = "Permainan belum dimulai. Silakan daftarkan pemain terlebih dahulu." });
        }

        var result = _gameService.MakeMove(move.From, move.To);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
