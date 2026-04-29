using CheckerboardGameApp.Constants;
using CheckerboardGameApp.Dtos;
using CheckerboardGameApp.Enums;
using CheckerboardGameApp.Factories;
using CheckerboardGameApp.Filters;
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
    private readonly ILogger<GameController> _logger;

    public GameController(IGameService gameService, GameFactory factory, ILogger<GameController> logger)
    {
        _gameService = gameService;
        _factory = factory;
        _logger = logger;
    }

    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<MessageResponse> GetGameStatus()
    {
        return Ok(new MessageResponse
        {
            Message = GameConstants.SuccessGameRunning,
            Status = "OK"
        });
    }

    [HttpPost("start")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<GameResponse<string>> StartGame([FromBody] GameStartRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PlayerWhiteName) || string.IsNullOrWhiteSpace(request.PlayerBlackName))
        {
            _logger.LogWarning(GameConstants.LogGameStartFailed);
            return BadRequest(GameResponse<string>.Failure(GameConstants.ErrorPlayerNamesRequired));
        }

        if (request.PlayerWhiteName.Equals(request.PlayerBlackName, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(GameConstants.LogGameStartFailed);
            return BadRequest(GameResponse<string>.Failure(GameConstants.ErrorPlayerNamesDuplicate));
        }

        try
        {
            _factory.CreateGame();
            _gameService.InitializePlayers(request.PlayerWhiteName, request.PlayerBlackName);

            var message = string.Format(GameConstants.SuccessGameStarted, request.PlayerWhiteName, request.PlayerBlackName);
            _logger.LogInformation("Game dimulai dengan pemain: {0} (Putih) vs {1} (Hitam)", request.PlayerWhiteName, request.PlayerBlackName);

            return Ok(GameResponse<string>.Success(message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat memulai game");
            return BadRequest(GameResponse<string>.Failure("Gagal memulai game: " + ex.Message));
        }
    }

    [HttpGet("state")]
    [RequireGameStarted]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<GameResponse<GameStateResponse>> GetGameState()
    {
        try
        {
            var response = new GameStateResponse
            {
                Status = new GameStatusInfo
                {
                    CurrentPlayer = _gameService.CurrentPlayerColor.ToString(),
                    GameStatus = _gameService.Status.ToString(),
                    WhitePlayer = _gameService.WhitePlayer!,
                    BlackPlayer = _gameService.BlackPlayer!,
                    Winner = _gameService.Winner
                },
                Board = new BoardInfo
                {
                    Rows = GameConstants.BoardSize,
                    Cols = GameConstants.BoardSize,
                    Squares = _gameService.FlattenBoard()
                }
            };

            return Ok(GameResponse<GameStateResponse>.Success(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, GameConstants.LogGetGameStateFailed);
            return BadRequest(GameResponse<GameStateResponse>.Failure("Gagal mendapatkan status game: " + ex.Message));
        }
    }

    [HttpPost("reset")]
    [RequireGameStarted]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<GameResponse<string>> ResetGame()
    {
        try
        {
            IGameService newGame = _factory.CreateGame();
            _gameService.LoadState(newGame.Board, Color.White, GameStatus.Ongoing);
            _gameService.ResetPlayers();

            _logger.LogInformation("Game berhasil di-reset");
            return Ok(GameResponse<string>.Success(GameConstants.SuccessGameReset));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, GameConstants.ErrorResetGameFailed);
            return BadRequest(GameResponse<string>.Failure($"{GameConstants.ErrorResetGameFailed}: {ex.Message}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, GameConstants.ErrorResetGameFailed);
            return BadRequest(GameResponse<string>.Failure($"{GameConstants.ErrorResetGameFailed}: {ex.Message}"));
        }
    }

    [HttpPost("demo")]
    [RequireGameStarted]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<GameResponse<string>> LoadDemoGame()
    {
        try
        {
            IBoard demoBoard = _factory.CreateDemoGame();
            _gameService.LoadState(demoBoard, Color.Black, GameStatus.Ongoing);

            _logger.LogInformation("Papan demo berhasil dimuat");
            return Ok(GameResponse<string>.Success(GameConstants.SuccessDemoLoaded));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat memuat papan demo");
            return BadRequest(GameResponse<string>.Failure("Gagal memuat papan demo: " + ex.Message));
        }
    }

    [HttpGet("valid-moves/{x}/{y}")]
    [RequireGameStarted]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<GameResponse<ValidMoveResponse>> GetValidMoves(int x, int y)
    {
        try
        {
            _logger.LogInformation(GameConstants.LogValidMovesRequest, x, y);
            Point fromPoint = new Point(x, y);
            List<MoveOption> options = _gameService.GetValidMove(fromPoint);

            var result = new ValidMoveResponse
            {
                From = fromPoint,
                Valid = options
            };

            return Ok(GameResponse<ValidMoveResponse>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat mendapatkan valid moves untuk ({0}, {1})", x, y);
            return BadRequest(GameResponse<ValidMoveResponse>.Failure("Gagal mendapatkan valid moves: " + ex.Message));
        }
    }

    [HttpPost("move")]
    [RequireGameStarted]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<GameResponse<MoveOption>> MakeMove([FromBody] MakeMoveRequest move)
    {
        try
        {
            _logger.LogInformation(GameConstants.LogMakeMoveRequest, move.From, move.To, _gameService.CurrentPlayerColor);

            GameResponse<MoveOption> result = _gameService.MakeMove(move.From, move.To);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Move ditolak: {0}", result.Message);
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat membuat move dari {0} ke {1}", move.From, move.To);
            return BadRequest(GameResponse<MoveOption>.Failure("Gagal membuat move: " + ex.Message));
        }
    }
}
