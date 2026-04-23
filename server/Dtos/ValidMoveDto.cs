using CheckerboardGameApp.Models;

public record ValidMoveDto
{
    public Point FromPoint { get; set; }
    public Point ToPoint { get; set; }
}