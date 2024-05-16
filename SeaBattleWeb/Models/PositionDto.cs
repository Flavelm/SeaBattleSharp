using Newtonsoft.Json;
using SeaBattleWeb.Models.Base;
using SeaBattleWeb.Models.Play.Models.Play;

namespace SeaBattleWeb.Models;

public class PositionDto : Serializable
{
    public List<PositionModel>? YourField { get; set; }
    public List<PositionModel>? OpponentField { get; set; }
    public bool? NextShotBy { get; set; }
}