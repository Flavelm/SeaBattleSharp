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
    
    [Key] public Guid FieldId { get; init; }
    public ProfileModel OwnedProfile { get; init; }
    public ReadOnlyCollection<ShipModel> Ships { get; init; }
    public List<PositionModel> OpenedPositions { get; init; }

    //Position, IsShip
    public List<PositionModel> GetField(ProfileModel profileModel)
    {
        var toReturn = new List<PositionModel>();
        if (OpenedPositions.Equals(profileModel))
        {
            toReturn.AddRange(Ships);
        }
        else
        {
            toReturn.AddRange(Ships.Where(ship => ship.IsBroken));
        }
        
        toReturn.AddRange(OpenedPositions);
        return toReturn;
    }
}