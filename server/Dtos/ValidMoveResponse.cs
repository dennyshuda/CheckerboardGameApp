using CheckerboardGameApp.Models;

namespace CheckerboardGameApp.Dtos;

public record ValidMoveResponse
{
    public Point From { get; set; }

    public List<MoveOption> Valid { get; set; } = [];
}

public record MoveOption
{
    public Point To { get; set; }
    public Point? EnemyCaptured { get; set; }
}