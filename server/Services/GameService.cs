using CheckerboardGameApp.Enums;
using CheckerboardGameApp.Interfaces;
using CheckerboardGameApp.Models;

namespace CheckerboardGameApp.Services;

public class GameService : IGameService
{
    public IBoard Board { get; private set; }
    public Color CurrentPlayerColor { get; set; }
    public GameStatus Status { get; set; }
    public Color? Winner { get; set; }

    public GameService(IBoard board, Color currentPlayerColor, GameStatus status)
    {
        Board = board;
        CurrentPlayerColor = currentPlayerColor;
        Status = status;
    }

    public List<Square> FlattenBoard()
    {
        var list = new List<Square>();
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                var square = Board.Squares[y, x];

                square.Point = new Point { X = x, Y = y };
                list.Add(square);
            }
        }
        return list;
    }


    public void InitializeDemoScenario()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                Board.Squares[y, x].Piece = null;
            }
        }

        Board.Squares[2, 2].Piece = new Piece(Color.White, Role.Troop);

        Board.Squares[1, 1].Piece = new Piece(Color.Black, Role.Troop);
    }

    public void DoMove(Point from, Point to)
    {
        var squareFrom = Board.Squares[from.Y, from.X];
        var squareTo = Board.Squares[to.Y, to.X];
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
            Board.Squares[midRow, midCol].Piece = null;
        }

        squareTo.Piece = piece;
        Board.Squares[from.Y, from.X].Piece = null;

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

        CheckWinner();
    }

    public List<Point> GetValidMove(Point from)
    {
        var validMoves = new List<Point>();
        var piece = Board.Squares[from.Y, from.X].Piece;

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

    // singlre resopen
    private void CheckAndAddMove(List<Point> list, int targetX, int targetY)
    {
        if (targetX >= 0 && targetX < 8 && targetY >= 0 && targetY < 8)
        {
            if (Board.Squares[targetY, targetX].Piece == null)
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
            var enemyPiece = Board.Squares[enemyY, enemyX].Piece;
            var targetSquare = Board.Squares[targetY, targetX];

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
        Board.Squares[point.Y, point.X].Piece = null;
        // tru false
    }

    private void CheckPromotion(Piece piece, Point to)
    {

        if (piece?.Role != Role.Troop) return;
        if ((piece.Color != Color.White || to.Y != 0) &&
            (piece.Color != Color.Black || to.Y != 7)) return;
        piece.Role = Role.King;
    }


    private bool IsKing(Piece piece)
    {
        return piece.Role == Role.King;
    }

    public void CheckWinner()
    {
        bool whiteCanMove = CanPlayerMove(Color.White);
        bool blackCanMove = CanPlayerMove(Color.Black);

        if (!whiteCanMove)
        {
            Winner = Color.Black;
            Status = GameStatus.GameOver;
        }

        if (!blackCanMove)
        {
            Winner = Color.White;
            Status = GameStatus.GameOver;
        }
    }

    private bool CanPlayerMove(Color playerColor)
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var square = Board.Squares[row, col];
                if (square.Piece != null && square.Piece.Color == playerColor)
                {
                    var moves = GetValidMove(new Point(col, row));
                    if (moves.Count > 0) return true;
                }
            }
        }
        return false;
    }

    public void LoadState(IBoard board, Color currentPlayer, GameStatus status)
    {
        Board = board;
        CurrentPlayerColor = currentPlayer;
        Status = status;
    }
}
