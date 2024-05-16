using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeaBattleWeb.Models.Play.Models.Play;

public class PositionModel
{
    [ForeignKey("Id")]  public int Id { get; init; }
    
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