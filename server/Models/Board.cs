
namespace CheckerboardGameApp.Models;

public interface IBoard
{
    Square[,] Squares { get; set; }
}

public class Board : IBoard
{
    public Square[,] Squares { get; set; }

    public Board()
    {
        Squares = new Square[8, 8];
    }
}