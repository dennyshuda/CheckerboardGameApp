
using CheckerboardGameApp.Interfaces;
using CheckerboardGameApp.Enums;
using CheckerboardGameApp.Models;

public class GameService_IsGameServiceShould
{
    private CheckerboardGameApp.Services.GameService _gameService;
    private IBoard _board = new Board();

    [SetUp]
    public void SetUp()
    {

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                _board.Squares[row, col] = new Square(new Point(col, row), null);
            }
        }


        _gameService = new CheckerboardGameApp.Services.GameService(_board, Color.White, GameStatus.Ongoing);
    }

    [Test]
    public void Constructor_ShouldInitializeCorrectly()
    {
        Assert.That(_gameService.CurrentPlayerColor, Is.EqualTo(Color.White));
        Assert.That(_gameService.Status, Is.EqualTo(GameStatus.Ongoing));
    }

    [Test]

    public void MakeMove_Input_ReturnTrue()
    {
        Point from = new(x: 0, y: 5);
        Point to = new(x: 1, y: 4);

        Piece whitePiece = new(Color.White, Role.Troop);

        _board.Squares[from.Y, from.X] = new Square(new Point(from.X, from.Y), whitePiece);

        var response = _gameService.MakeMove(from, to);

        Assert.That(response.IsSuccess, Is.True);
    }

    [Test]
    public void MakeMove_Input_ReturnFalse()
    {
        Point from = new(x: 0, y: 5);
        Point to = new(x: 1, y: 6);

        Piece whitePiece = new(Color.White, Role.Troop);

        _board.Squares[from.Y, from.X] = new Square(new Point(from.X, from.Y), whitePiece);

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
    public void IsKing_Input_ReturnFalse()
    {
        Piece piece = new(Color.White, Role.Troop);

        bool result = _gameService.IsKing(piece);
        Assert.That(result, Is.False);
    }
}