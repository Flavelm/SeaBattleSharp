using Microsoft.EntityFrameworkCore;

namespace SeaBattleWeb.Models.Play.Models.Play;

[Keyless]
public class PositionModel
{
    public PositionModel(int x, int y)
    {
        X = x;
        Y = y;
    }
    
    public PositionModel()
    {}
    
    public int X { get; init; }
    public int Y { get; init; }
}