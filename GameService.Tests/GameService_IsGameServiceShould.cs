
using CheckerboardGameApp.Interfaces;
using CheckerboardGameApp.Enums;
using CheckerboardGameApp.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameService.Tests;

public class GameService_IsGameServiceShould
{
    private Mock<ILogger<CheckerboardGameApp.Services.GameService>> _loggerMock;
    private CheckerboardGameApp.Services.GameService _gameService;
    private IBoard _board = new Board();

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<CheckerboardGameApp.Services.GameService>>();

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                _board.Squares[row, col] = new Square(new Point(col, row), null);
            }
        }

        _gameService = new CheckerboardGameApp.Services.GameService(_board, Color.White, GameStatus.Ongoing, logger: _loggerMock.Object);
    }

    [Test]
    public void Constructor_ShouldInitializeCorrectly()
    {
        Assert.That(_gameService.CurrentPlayerColor, Is.EqualTo(Color.White));
        Assert.That(_gameService.Status, Is.EqualTo(GameStatus.Ongoing));
    }

    [Test]
    public void MakeMove_NormalMove_ReturnTrue()
    {
        Point from = new(x: 0, y: 5);
        Point to = new(x: 1, y: 4);

        Piece whitePiece = new(Color.White, Role.Troop);

        _board.Squares[from.Y, from.X] = new Square(new Point(from.X, from.Y), whitePiece);

        var response = _gameService.MakeMove(from, to);

        Assert.That(response.IsSuccess, Is.True);
    }

    [Test]
    public void MakeMove_KingNormalMoveBackward_ReturnTrue()
    {
        Point from = new(x: 0, y: 5);
        Point to = new(x: 1, y: 6);

        Piece whitePiece = new(Color.White, Role.King);

        _board.Squares[from.Y, from.X] = new Square(new Point(from.X, from.Y), whitePiece);

        var response = _gameService.MakeMove(from, to);

        Assert.That(response.IsSuccess, Is.True);
    }

    [Test]
    public void MakeMove_NormalMoveInvalidDestination_ReturnFalse()
    {
        Point from = new(x: 0, y: 5);
        Point to = new(x: 3, y: 4);

        Piece whitePiece = new(Color.White, Role.Troop);

        _board.Squares[from.Y, from.X] = new Square(new Point(from.X, from.Y), whitePiece);

        var response = _gameService.MakeMove(from, to);

        Assert.That(response.IsSuccess, Is.False);
    }

    [Test]
    public void MakeMove_CaptureMove_ReturnTrue()
    {
        Point from = new(x: 0, y: 5);
        Point to = new(x: 2, y: 3);
        Point blackPiecePosition = new(x: 1, y: 4);

        Piece whitePiece = new(Color.White, Role.Troop);
        Piece blackPiece = new(Color.Black, Role.Troop);

        _board.Squares[from.Y, from.X] = new Square(new Point(from.X, from.Y), whitePiece);
        _board.Squares[blackPiecePosition.Y, blackPiecePosition.X] = new Square(new Point(blackPiecePosition.X, blackPiecePosition.Y), blackPiece);

        var response = _gameService.MakeMove(from, to);

        Assert.That(response.IsSuccess, Is.True);
    }

    [Test]
    public void MakeMove_KingCaptureMoveBackward_ReturnTrue()
    {
        Point from = new(x: 0, y: 5);
        Point to = new(x: 1, y: 6);
        Point blackPiecePosition = new(x: 2, y: 7);

        Piece whitePiece = new(Color.White, Role.King);
        Piece blackPiece = new(Color.Black, Role.Troop);

        _board.Squares[from.Y, from.X] = new Square(new Point(from.X, from.Y), whitePiece);
        _board.Squares[blackPiecePosition.Y, blackPiecePosition.X] = new Square(new Point(blackPiecePosition.X, blackPiecePosition.Y), blackPiece);

        var response = _gameService.MakeMove(from, to);

        Assert.That(response.IsSuccess, Is.True);
    }

    [Test]
    public void MakeMove_CaptureMoveHasNoEnemy_ReturnFalse()
    {
        Point from = new(x: 0, y: 5);
        Point to = new(x: 2, y: 3);
        Point blackPiecePosition = new(x: 3, y: 2);

        Piece whitePiece = new(Color.White, Role.Troop);
        Piece blackPiece = new(Color.Black, Role.Troop);

        _board.Squares[from.Y, from.X] = new Square(new Point(from.X, from.Y), whitePiece);
        _board.Squares[blackPiecePosition.Y, blackPiecePosition.X] = new Square(new Point(blackPiecePosition.X, blackPiecePosition.Y), blackPiece);

        var response = _gameService.MakeMove(from, to);

        Assert.That(response.IsSuccess, Is.False);
    }

    [Test]
    public void MakeMove_Input_ReturnFalseNoPiece()
    {
        Point from = new(x: 0, y: 5);
        Point to = new(x: 1, y: 4);

        var response = _gameService.MakeMove(from, to);

        Assert.That(response.IsSuccess, Is.False);
    }

    [Test]
    public void MakeMove_WhitePiecePromoteToKing_ReturnTrue()
    {
        Point from = new(x: 2, y: 1);
        Point to = new(x: 1, y: 0);
        Piece whitePiece = new(Color.White, Role.Troop);
        _gameService.CurrentPlayerColor = Color.White;

        _board.Squares[from.Y, from.X] = new Square(new Point(from.X, from.Y), whitePiece);

        var response = _gameService.MakeMove(from, to);
        Assert.That(response.IsSuccess, Is.True);

        Piece? pieceAtDestination = _board.Squares[to.Y, to.X].Piece;
        Assert.That(pieceAtDestination, Is.Not.Null);
        Assert.That(pieceAtDestination.Role, Is.EqualTo(Role.King), "Bidak seharusnya sudah dipromosikan jadi King");
    }

    [Test]
    public void MakeMove_BlackPiecePromoteToKing_ReturnTrue()
    {
        Point from = new(x: 5, y: 6);
        Point to = new(x: 6, y: 7);
        Piece blackPiece = new(Color.Black, Role.Troop);
        _gameService.CurrentPlayerColor = Color.Black;

        _board.Squares[from.Y, from.X] = new Square(new Point(from.X, from.Y), blackPiece);

        var response = _gameService.MakeMove(from, to);
        Assert.That(response.IsSuccess, Is.True);

        Piece? pieceAtDestination = _board.Squares[to.Y, to.X].Piece;
        Assert.That(pieceAtDestination, Is.Not.Null);
        Assert.That(pieceAtDestination.Role, Is.EqualTo(Role.King), "Bidak seharusnya sudah dipromosikan jadi King");
    }

    [Test]
    public void CheckPromotion_Input_ReturnTrue()
    {
        Point to = new(x: 0, y: 0);
        Piece whitePiece = new(Color.White, Role.Troop);

        bool result = _gameService.CheckPromotion(whitePiece, to);
        Assert.That(result, Is.True);
    }

    [Test]
    public void CheckPromotion_Input_ReturnFalse()
    {
        Point to = new(x: 0, y: 3);
        Piece whitePiece = new(Color.White, Role.Troop);

        bool result = _gameService.CheckPromotion(whitePiece, to);
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsInsideBoard_Input_ReturnTrue()
    {
        bool result = _gameService.IsInsideBoard(3, 4);
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsInsideBoard_Input_ReturnFalse()
    {
        bool result = _gameService.IsInsideBoard(-1, 4);
        Assert.That(result, Is.False);

        result = _gameService.IsInsideBoard(3, -1);
        Assert.That(result, Is.False);

        result = _gameService.IsInsideBoard(8, 4);
        Assert.That(result, Is.False);

        result = _gameService.IsInsideBoard(3, 8);
        Assert.That(result, Is.False);
    }

    [Test]
    public void ResetPlayers_Input_ReturnNull()
    {
        _gameService.InitializePlayers("Alice", "Bob");
        _gameService.ResetPlayers();

        Assert.That(_gameService.BlackPlayer, Is.Null);
        Assert.That(_gameService.Winner, Is.Null);
        Assert.That(_gameService.WhitePlayer, Is.Null);
    }


    [Test]
    public void IsKing_Input_ReturnFalse()
    {
        Piece piece = new(Color.White, Role.Troop);

        bool result = _gameService.IsKing(piece);
        Assert.That(result, Is.False);
    }
}