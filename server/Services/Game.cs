using CheckerboardGameApp.Enums;
using CheckerboardGameApp.Models;

namespace CheckerboardGameApp.Services;

public interface IGame
{
    Color CurrentPlayerColor { get; set; }
    GameStatus Status { get; set; }
    List<Square> GetBoard();
    void DoMove(Point from, Point to);
    void ResetGame();
    List<Point> GetValidMove(Point from);
    Color? GetWinner();
    void InitializeDemoScenario();
}

public class Game : IGame
{
    private IBoard _board { get; set; }
    private Color _currentPlayerColor;
    public GameStatus Status { get; set; }

    public Color CurrentPlayerColor
    {
        get => _currentPlayerColor;
        set => _currentPlayerColor = value;
    }

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
        Status = GameStatus.Ongoing;
        InitializeBoard();
        InitializePiece();
    }

    private void InitializeBoard()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                _board.Squares[row, col] = new Square(new Point(col, row), null);
            }
        }
    }

    public void InitializePiece()
    {
        for (var row = 0; row < 8; row++)
        {
            for (var col = 0; col < 8; col++)
            {
                if ((col + row) % 2 != 0)
                {
                    _board.Squares[row, col].Piece = row switch
                    {
                        < 3 => new Piece(Color.Black, Role.Troop),
                        > 4 => new Piece(Color.White, Role.Troop),
                        _ => _board.Squares[row, col].Piece
                    };
                }
            }
        }
    }

    public void InitializeDemoScenario()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                _board.Squares[y, x].Piece = null;
            }
        }

        _board.Squares[2, 2].Piece = new Piece(Color.White, Role.Troop);

        _board.Squares[1, 1].Piece = new Piece(Color.Black, Role.Troop);
    }

    public void DoMove(Point from, Point to)
    {
        var squareFrom = _board.Squares[from.Y, from.X];
        var squareTo = _board.Squares[to.Y, to.X];
        var piece = squareFrom.Piece;

        var validMoves = GetValidMove(from);

        if (!validMoves.Any(m => m.X == to.X && m.Y == to.Y))
        {
            throw new Exception("Langkah tidak valid atau dilarang!");
        }

        if (Math.Abs(from.X - to.X) == 2)
        {
            var midCol = (from.X + to.X) / 2;
            var midRow = (from.Y + to.Y) / 2;
            _board.Squares[midRow, midCol].Piece = null;
        }

        squareTo.Piece = piece;
        _board.Squares[from.Y, from.X].Piece = null;

        if (piece != null) CheckPromotion(piece, to);

        if (piece?.Color == Color.White && to.Y == 0)
        {
            piece.Role = Role.King;
        }
        else if (piece?.Color == Color.Black && to.Y == 7)
        {
            piece.Role = Role.King;
        }

        SwitchTurn();

        var winner = GetWinner();
        if (winner != null)
        {
            Status = GameStatus.GameOver;
            Console.WriteLine($"Permainan Selesai! Pemenangnya adalah {winner}");
        }
    }

    public List<Point> GetValidMove(Point from)
    {
        var validMoves = new List<Point>();
        var piece = _board.Squares[from.Y, from.X].Piece;

        if (piece == null) return validMoves;

        int directionRow = (piece.Color == Color.White) ? -1 : 1;

        int[] directionCol = [-1, 1];

        foreach (var col in directionCol)
        {
            CheckAndAddMove(validMoves, from.X + col, from.Y + directionRow);

            if (IsKing(piece)) CheckAndAddMove(validMoves, from.X + col, from.Y - directionRow);

            CheckAndAddJump(validMoves, from, col, directionRow, piece.Color);

            if (IsKing(piece)) CheckAndAddJump(validMoves, from, col, -directionRow, piece.Color);
        }

        return validMoves;
    }

    private void CheckAndAddMove(List<Point> list, int targetX, int targetY)
    {
        if (targetX >= 0 && targetX < 8 && targetY >= 0 && targetY < 8)
        {
            if (_board.Squares[targetY, targetX].Piece == null)
            {
                list.Add(new Point(targetX, targetY));
            }
        }
    }

    private void CheckAndAddJump(List<Point> list, Point from, int dx, int dy, Color myColor)
    {
        int enemyX = from.X + dx;
        int enemyY = from.Y + dy;
        int targetX = from.X + (dx * 2);
        int targetY = from.Y + (dy * 2);

        if (targetX >= 0 && targetX < 8 && targetY >= 0 && targetY < 8)
        {
            var enemyPiece = _board.Squares[enemyY, enemyX].Piece;
            var targetSquare = _board.Squares[targetY, targetX];

            if (enemyPiece != null && enemyPiece.Color != myColor && targetSquare.Piece == null)
            {
                list.Add(new Point(targetX, targetY));
            }
        }
    }

    private void SwitchTurn()
    {
        CurrentPlayerColor = (CurrentPlayerColor == Color.White) ? Color.Black : Color.White;
    }


    public void RemovePiece(Point point)
    {
        _board.Squares[point.Y, point.X].Piece = null;
    }

    private void CheckPromotion(Piece piece, Point to)
    {
        if (piece?.Role != Role.Troop) return;
        if ((piece.Color != Color.White || to.Y != 0) &&
            (piece.Color != Color.Black || to.Y != 7)) return;
        piece.Role = Role.King;
    }

    private bool IsInsideBoard(Point point) => point.X is >= 0 and < 8 && point.Y is >= 0 and < 8;

    private bool IsKing(Piece piece)
    {
        return piece.Role == Role.King;
    }

    public void ResetGame()
    {
        _board = new Board();
        _currentPlayerColor = Color.White;
        Status = GameStatus.Ongoing;

        InitializeBoard();
        InitializePiece();
    }

    public Color? GetWinner()
    {
        bool whiteCanMove = CanPlayerMove(Color.White);
        bool blackCanMove = CanPlayerMove(Color.Black);

        if (!whiteCanMove) return Color.Black;

        if (!blackCanMove) return Color.White;

        // Belum ada pemenang
        return null;
    }

    private bool CanPlayerMove(Color playerColor)
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var square = _board.Squares[row, col];
                if (square.Piece != null && square.Piece.Color == playerColor)
                {
                    var moves = GetValidMove(new Point(col, row));
                    if (moves.Count > 0) return true;
                }
            }
        }
        return false;
    }


}
