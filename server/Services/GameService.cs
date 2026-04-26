using CheckerboardGameApp.Dtos;
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
    public string WhitePlayerName { get; set; } = String.Empty;
    public string BlackPlayerName { get; set; } = String.Empty;

    public GameService(IBoard board, Color currentPlayerColor, GameStatus status)
    {
        Board = board;
        CurrentPlayerColor = currentPlayerColor;
        Status = status;
    }

    public void LoadState(IBoard board, Color currentPlayer, GameStatus status)
    {
        Board = board;
        CurrentPlayerColor = currentPlayer;
        Status = status;
    }

    public void InitializePlayers(string whiteName, string blackName)
    {
        WhitePlayerName = whiteName;
        BlackPlayerName = blackName;
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

    public MakeMoveResponse MakeMove(Point from, Point to)
    {
        var squareFrom = Board.Squares[from.Y, from.X];
        var squareTo = Board.Squares[to.Y, to.X];
        var piece = squareFrom.Piece;

        var validMoves = GetValidMove(from);

        if (!validMoves.Any(m => m.X == to.X && m.Y == to.Y))
        {
            return new MakeMoveResponse { IsSuccess = false, Message = "Langkah tidak valid!" };
        }

        if (Math.Abs(from.X - to.X) == 2)
        {
            var midCol = (from.X + to.X) / 2;
            var midRow = (from.Y + to.Y) / 2;
            Board.Squares[midRow, midCol].Piece = null;
        }

        squareTo.Piece = piece;
        Board.Squares[from.Y, from.X].Piece = null;


        bool isPromotion = CheckPromotion(piece, to);

        if (isPromotion)
        {
            PromoteRole(piece);
        }

        SwitchTurn();

        CheckWinner();

        return new MakeMoveResponse
        {
            IsSuccess = true,
            Message = "Gerakan berhasil",
        };
    }

    public List<Point> GetValidMove(Point from)
    {
        List<Point> validMoves = [];
        var piece = Board.Squares[from.Y, from.X].Piece;

        if (piece == null) return validMoves;

        var rowDirection = new Dictionary<string, int> { { "Up", -1 }, { "Down", 1 } };
        var colDirection = new Dictionary<string, int> { { "Left", -1 }, { "Right", 1 } };

        int directionRow = (piece.Color == Color.White) ? rowDirection["Up"] : rowDirection["Down"];

        List<int> directionCol = [colDirection["Left"], colDirection["Right"]];

        foreach (var col in directionCol)
        {
            IsValidNormalMove(validMoves, from.X + col, from.Y + directionRow);

            if (IsKing(piece))
            {
                IsValidNormalMove(validMoves, from.X + col, from.Y - directionRow);
            }

            IsValidCaptureMove(validMoves, from, col, directionRow, piece.Color);

            if (IsKing(piece))
            {
                IsValidCaptureMove(validMoves, from, col, -directionRow, piece.Color);
            }
        }

        return validMoves;
    }

    private void IsValidNormalMove(List<Point> list, int targetX, int targetY)
    {
        if (targetX >= 0 && targetX < 8 && targetY >= 0 && targetY < 8)
        {
            if (Board.Squares[targetY, targetX].Piece == null)
            {
                list.Add(new Point(targetX, targetY));
            }
        }
    }

    private void IsValidCaptureMove(List<Point> list, Point from, int dx, int dy, Color myColor)
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
    }

    private bool CheckPromotion(Piece piece, Point to)
    {

        if ((piece.Color != Color.White || to.Y != 0) &&
            (piece.Color != Color.Black || to.Y != 7))
        {
            return false;
        }

        return true;
    }

    private void PromoteRole(Piece piece)
    {
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
}
