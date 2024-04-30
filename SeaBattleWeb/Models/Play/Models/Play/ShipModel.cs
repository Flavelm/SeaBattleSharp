using Microsoft.EntityFrameworkCore;

namespace SeaBattleWeb.Models.Play.Models.Play;

public class ShipModel(int x, int y) : PositionModel(x, y)
{
    private int Id { get; init; }
    
    public event EventHandler Updated; 
    public bool IsBroken { get; set; }

    public void Break()
    {
        IsBroken = true;
        Updated.Invoke(this, EventArgs.Empty);
    }
}