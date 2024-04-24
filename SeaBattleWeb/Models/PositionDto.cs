using SeaBattleWeb.Models.Play;

namespace SeaBattleWeb.Models;

public class PositionDto
{
    public List<Position>? YourField { get; set; }
    public List<Position>? OpponentField { get; set; }
}