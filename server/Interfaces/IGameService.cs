using CheckerboardGameApp.Dtos;
using CheckerboardGameApp.Enums;
using CheckerboardGameApp.Models;

namespace CheckerboardGameApp.Interfaces;

public interface IGameService
{
    IBoard Board { get; }
    Color CurrentPlayerColor { get; }
    GameStatus Status { get; }
    Color? Winner { get; }

    IPlayer? WhitePlayer { get; }
    IPlayer? BlackPlayer { get; }
    List<Square> FlattenBoard();
    MakeMoveResponse MakeMove(Point from, Point to);
    List<MoveOption> GetValidMove(Point from);
    void CheckWinner();
    void LoadState(IBoard board, Color currentPlayer, GameStatus status);
    void InitializePlayers(string whiteName, string blackName);
    void ResetPlayers();
    bool IsKing(Piece piece);
    bool IsInsideBoard(int col, int row);
    bool CheckPromotion(Piece piece, Point to);
    bool CanPlayerMove(Color playerColor);
}