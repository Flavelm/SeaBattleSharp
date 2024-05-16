using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeaBattleWeb.Models.Play.Models.Play;

public class ShipModel(int x, int y) : PositionModel(x, y)
{
    [ForeignKey("Id")] public int Id { get; init; }
    
    public event EventHandler Updated; 
    public bool IsBroken { get; set; }

    public void Break()
    {
        IsBroken = true;
        Updated.Invoke(this, EventArgs.Empty);
    }
}