using SeaBattleWeb.Models.Base;
using SeaBattleWeb.Models.Play;
using SeaBattleWeb.Models.Play.Models.Play;

namespace SeaBattleWeb.Models;

public class ShipsDto : Serializable
{
    public List<ShipModel>? YourField { get; set; }
    public List<ShipModel>? OpponentField { get; set; }
}