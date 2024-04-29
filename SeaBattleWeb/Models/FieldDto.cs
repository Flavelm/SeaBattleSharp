using System.Collections.ObjectModel;
using SeaBattleWeb.Models.Play.Models.Play;

namespace SeaBattleWeb.Models;

public class FieldDto
{
    public List<ShipModel> Ships { get; init; }
}