
using CheckerboardGameApp.Interfaces;

namespace CheckerboardGameApp.Models;


public class Board : IBoard
{
    public Square[,] Squares { get; set; }

    public Board()
    {
        Squares = new Square[8, 8];
    }
}