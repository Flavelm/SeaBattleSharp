using System.ComponentModel.DataAnnotations;

namespace SeaBattleWeb.Models.Play.Models.Play;

public class PositionModel
{
    [Key]  public int Id { get; init; }
    
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