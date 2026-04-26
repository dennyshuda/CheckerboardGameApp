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
    List<Square> FlattenBoard();
    MakeMoveResponse MakeMove(Point from, Point to);
    List<Point> GetValidMove(Point from);
    void CheckWinner();
    void LoadState(IBoard board, Color currentPlayer, GameStatus status);
}