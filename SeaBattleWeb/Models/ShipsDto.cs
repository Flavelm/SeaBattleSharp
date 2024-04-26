using SeaBattleWeb.Models.Base;
using SeaBattleWeb.Models.Play;

namespace SeaBattleWeb.Models;

public class ShipsDto : Serializable
{
    public List<Ship>? YourField { get; set; }
    public List<Ship>? OpponentField { get; set; }
}