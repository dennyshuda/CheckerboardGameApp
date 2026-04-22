namespace CheckerboardGameApp.Models;

public class Square
{
    public Point Point { get; set; }
    public Piece? Piece { get; set; }

    public Square(Point point, Piece? piece)
    {
        Point = point;
        Piece = piece;
    }
}