using CheckerboardGameApp.Models;

namespace CheckerboardGameApp.Dtos;

public class MakeMoveRequest
{
    public Point From { get; set; }
    public Point To { get; set; }
}
