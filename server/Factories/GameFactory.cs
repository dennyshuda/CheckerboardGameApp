using CheckerboardGameApp.Enums;
using CheckerboardGameApp.Interfaces;
using CheckerboardGameApp.Models;

namespace CheckerboardGameApp.Factories;

public class GameFactory
{
    private readonly ILogger<Services.GameService> _logger;
    public GameFactory(ILogger<Services.GameService> logger)
    {
        _logger = logger;
    }

    public IGameService CreateGame()
    {
        IBoard board = new Board();
        InitializeBoard(board);
        InitializePiece(board);

        return new CheckerboardGameApp.Services.GameService(board, Color.White, GameStatus.Ongoing, logger: _logger);
    }

    public IBoard CreateDemoGame()
    {
        IBoard board = new Board();
        InitializeBoard(board);
        InitializeDemoScenario(board);

        return board;
    }

    public void InitializeDemoScenario(IBoard board)
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                board.Squares[row, col].Piece = null;
            }
        }

        board.Squares[2, 2].Piece = new Piece(Color.White, Role.Troop);

        board.Squares[1, 1].Piece = new Piece(Color.Black, Role.Troop);
    }

    private void InitializeBoard(IBoard board)
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                board.Squares[row, col] = new Square(new Point(col, row), null);
            }
        }
    }

    private void InitializePiece(IBoard board)
    {
        for (var row = 0; row < 8; row++)
        {
            for (var col = 0; col < 8; col++)
            {
                if ((col + row) % 2 != 0)
                {
                    board.Squares[row, col].Piece = row switch
                    {
                        < 3 => new Piece(Color.Black, Role.Troop),
                        > 4 => new Piece(Color.White, Role.Troop),
                        _ => board.Squares[row, col].Piece
                    };
                }
            }
        }
    }
}