namespace CheckerboardGameApp.Dtos;

public record GameResponse<T>
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }

    public static GameResponse<T> Success(T data, string message = "Success")
        => new() { IsSuccess = true, Data = data, Message = message };

    public static GameResponse<T> Failure(string message)
        => new() { IsSuccess = false, Message = message };
}