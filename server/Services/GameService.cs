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
    public IPlayer? WhitePlayer { get; private set; }
    public IPlayer? BlackPlayer { get; private set; }

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
        WhitePlayer = new Player(whiteName);
        BlackPlayer = new Player(blackName);
    }

    public void ResetPlayers()
    {
        WhitePlayer = null;
        BlackPlayer = null;
        Winner = null;
    }

    public List<Square> FlattenBoard()
    {
        List<Square> list = [];
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                var square = Board.Squares[row, col];

                square.Point = new Point { X = col, Y = row };
                list.Add(square);
            }
        }
        return list;
    }

    public MakeMoveResponse MakeMove(Point from, Point to)
    {
        Square squareFrom = Board.Squares[from.Y, from.X];
        Square squareTo = Board.Squares[to.Y, to.X];
        Piece? piece = squareFrom.Piece;

        List<MoveOption> validMoves = GetValidMove(from);

        if (!validMoves.Any(move => move.To.X == to.X && move.To.Y == to.Y))
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

        if (piece == null)
        {
            return new MakeMoveResponse { IsSuccess = false, Message = "Tidak ada bidak di posisi awal!" };
        }

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

    public List<MoveOption> GetValidMove(Point from)
    {
        List<MoveOption> validMoves = [];
        Piece? piece = Board.Squares[from.Y, from.X].Piece;

        if (piece == null || piece.Color != CurrentPlayerColor) return validMoves;

        Dictionary<string, int> rowMovement = new()
        {
            { "Up", -1 },
            { "Down", 1 }
        };
        Dictionary<string, int> colMovement = new()
        {
            { "Left", -1 },
            { "Right", 1 }
        };

        int rows = (piece.Color == Color.White) ? rowMovement["Up"] : rowMovement["Down"];
        List<int> cols = [colMovement["Left"], colMovement["Right"]];

        foreach (int col in cols)
        {
            IsValidNormalMove(validMoves, from.X + col, from.Y + rows);

            if (IsKing(piece))
            {
                IsValidNormalMove(validMoves, from.X + col, from.Y - rows);
            }

            IsValidCaptureMove(validMoves, from, col, rows, piece.Color);

            if (IsKing(piece))
            {
                IsValidCaptureMove(validMoves, from, col, -rows, piece.Color);
            }
        }

        return validMoves;
    }

    private void IsValidNormalMove(List<MoveOption> list, int targetX, int targetY)
    {
        if (IsInsideBoard(targetX, targetY))
        {
            if (Board.Squares[targetY, targetX].Piece == null)
            {
                list.Add(new MoveOption { To = new Point(targetX, targetY), EnemyCaptured = null });
            }
        }
    }

    private void IsValidCaptureMove(List<MoveOption> list, Point from, int colDirection, int rowDirection, Color color)
    {
        int enemyX = from.X + colDirection;
        int enemyY = from.Y + rowDirection;
        int targetX = from.X + (colDirection * 2);
        int targetY = from.Y + (rowDirection * 2);

        if (IsInsideBoard(targetX, targetY))
        {
            var enemyPiece = Board.Squares[enemyY, enemyX].Piece;
            var targetSquare = Board.Squares[targetY, targetX].Piece;

            if (enemyPiece != null && enemyPiece.Color != color && targetSquare == null)
            {
                list.Add(new MoveOption
                {
                    To = new Point(targetX, targetY),
                    EnemyCaptured = new Point(enemyX, enemyY)
                });
            }
        }
    }

    private bool IsInsideBoard(int col, int row) => col >= 0 && col < 8 && row >= 0 && row < 8;

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
        bool canCurrentPlayerMove = CanPlayerMove(CurrentPlayerColor);

        Console.WriteLine($"Current Player ({CurrentPlayerColor}) can move: {canCurrentPlayerMove}");

        if (!canCurrentPlayerMove)
        {
            Winner = (CurrentPlayerColor == Color.White) ? Color.Black : Color.White;
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
                    if (moves.Count > 0)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
