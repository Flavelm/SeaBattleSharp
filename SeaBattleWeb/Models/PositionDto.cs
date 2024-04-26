using SeaBattleWeb.Models.Base;
using SeaBattleWeb.Models.Play;

namespace SeaBattleWeb.Models;

public class PositionDto : Serializable
{
    public List<Position>? YourField { get; set; }
    public List<Position>? OpponentField { get; set; }
}