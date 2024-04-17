namespace SeaBattleWeb.Models.Play;

public readonly struct Position(int x, int y)
{
    public int X { get; } = x;
    public int Y { get; } = y;
}