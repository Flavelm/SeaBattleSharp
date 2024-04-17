namespace SeaBattleWeb.Models.Play;

public class Ship
{
    public event EventHandler Updated; 
    public bool IsBroken { get; set; }

    public void Break()
    {
        IsBroken = true;
        Updated.Invoke(this, EventArgs.Empty);
    }
}