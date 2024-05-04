using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace SeaBattleWeb.Models.Play.Models.Play;

public class FieldModel
{
    public FieldModel(ProfileModel ownedProfile, List<ShipModel> ships)
    {
        OwnedProfile = ownedProfile;
        Ships = ships.AsReadOnly();
        OpenedPositions = new();
    }
    
    public FieldModel() { }
    
    [Key] public Guid FieldId { get; init; }
    public required ProfileModel OwnedProfile { get; init; }
    public required ReadOnlyCollection<ShipModel> Ships { get; init; }
    public required ConcurrentBag<PositionModel> OpenedPositions { get; init; }

    public List<PositionModel> FieldForOwner
    {
        get
        {
            var toReturn = new List<PositionModel>();
            toReturn.AddRange(Ships);
            toReturn.AddRange(OpenedPositions);
            return toReturn;
        }
    }

    public List<PositionModel> FieldForOther
    {
        get
        {
            var toReturn = new List<PositionModel>();
            toReturn.AddRange(Ships.Where(ship => ship.IsBroken));
            toReturn.AddRange(OpenedPositions);
            return toReturn;
        }
    }
}