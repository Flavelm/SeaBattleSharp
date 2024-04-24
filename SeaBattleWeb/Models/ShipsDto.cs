using SeaBattleWeb.Models.Play;

namespace SeaBattleWeb.Models;

public class ShipsDto
{
    public List<Ship>? YourField { get; set; }
    public List<Ship>? OpponentField { get; set; }
}