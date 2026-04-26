namespace CheckerboardGameApp.Dtos;

public record MessageResponse
{
    public required string Message { get; set; }
    public string? Status { get; set; }
}
