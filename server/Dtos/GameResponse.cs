namespace CheckerboardGameApp.Dtos;

public record GameResponse<T>
{
    public bool Status { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }

    public static GameResponse<T> Success(T data, string message = "Success")
        => new() { Status = true, Data = data, Message = message };

    public static GameResponse<T> Failure(string message)
        => new() { Status = false, Message = message };
}