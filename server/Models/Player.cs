using CheckerboardGameApp.Interfaces;

namespace CheckerboardGameApp.Models;

public class Player : IPlayer
{
    public string Name { get; init; }

    public Player(string name)
    {
        Name = name;
    }
}