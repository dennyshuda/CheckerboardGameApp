
namespace CheckerboardGameApp.Models;

public class Board
{
    public Square[,] Squares { get; set; }

    public Board(Square[,] squares)
    {
        Squares = squares;
    }
}