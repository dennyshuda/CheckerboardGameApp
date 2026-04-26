namespace CheckerboardGameApp.Dtos;

public class MakeMoveResponse
{
    public bool IsSuccess { get; set; }
    public required string Message { get; set; }
}