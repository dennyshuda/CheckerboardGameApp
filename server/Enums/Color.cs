namespace CheckerboardGameApp.Enums;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Color
{
    White,
    Black
}