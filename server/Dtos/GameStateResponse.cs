using CheckerboardGameApp.Models;

namespace CheckerboardGameApp.Dtos;

public record GameStateResponse
{
    public required GameStatusInfo Status { get; set; }
    public required BoardInfo Board { get; set; }
}

public record GameStatusInfo
{
    public required string CurrentPlayer { get; set; }
    public required string GameStatus { get; set; }
    public required object WhitePlayer { get; set; }
    public required object BlackPlayer { get; set; }
    public object? Winner { get; set; }
}

public record BoardInfo
{
    public required int Rows { get; set; }
    public required int Cols { get; set; }
    public required List<Square> Squares { get; set; }
}
