using CheckerboardGameApp.Models;

namespace CheckerboardGameApp.Interfaces;

public interface IBoard
{
    Square[,] Squares { get; set; }
}