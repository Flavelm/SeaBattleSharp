namespace SeaBattleWeb.Models.Play;

public class Ship(int x, int y) : Position(x, y)
{
    public event EventHandler Updated; 
    public bool IsBroken { get; set; }

    public void Break()
    {
        IsBroken = true;
        Updated.Invoke(this, EventArgs.Empty);
    }
}