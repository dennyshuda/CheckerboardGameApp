
using CheckerboardGameApp.Enums;

namespace CheckerboardGameApp.Models;

public class Piece
{
    public Color Color { get; }
    public Role Role { get; set; }

    public Piece(Color color, Role role)
    {
        Color = color;
        Role = role;
    }
}