using CheckerboardGameApp.Enums;
using CheckerboardGameApp.Models;

namespace CheckerboardGameApp.Services;

public interface IGame
{
    IBoard Board { get; set; }
    Color CurrentPlayerColor { get; }

    List<Square> GetBoard();
}

public class Game : IGame
{
    private IBoard _board;
    private Color _currentPlayerColor;

    IBoard IGame.Board
    {
        get => _board;
        set => _board = value;
    }

    Color IGame.CurrentPlayerColor => _currentPlayerColor;

    public List<Square> GetBoard()
    {
        var list = new List<Square>();
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                var square = _board.Squares[y, x];

                square.Point = new Point { X = x, Y = y };
                list.Add(square);
            }
        }
        return list;
    }


    public Game()
    {
        _board = new Board();
        _currentPlayerColor = Color.White;

        InitializeBoard();
        InitializePiece();
    }

    private void InitializeBoard()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                _board.Squares[y, x] = new Square(new Point(x, y), null);
            }
        }
    }

    private void InitializePiece()
    {
        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 8; x++)
            {
                if ((x + y) % 2 != 0)
                {
                    _board.Squares[y, x].Piece = y switch
                    {
                        < 3 => new Piece(Color.Black, Role.Troop),
                        > 4 => new Piece(Color.White, Role.Troop),
                        _ => _board.Squares[y, x].Piece
                    };
                }
            }
        }
    }

}
