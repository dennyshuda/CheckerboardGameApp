using System.Text.Json.Serialization;

namespace CheckerboardGameApp.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameStatus
{
    Draw,
    Ongoing,
    GameOver
}