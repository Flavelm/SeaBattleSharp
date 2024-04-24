using System.Collections.ObjectModel;
using NuGet.Packaging;

namespace SeaBattleWeb.Models.Play;

public class FieldModel
{
    private IProfileModel _ownedProfile;
    private readonly ReadOnlyCollection<Ship> _ships;
    private readonly List<Position> _openedPositions = new();

    public FieldModel(IProfileModel ownedProfile, List<Ship> ships)
    {
        _ownedProfile = ownedProfile;
        _ships = ships.AsReadOnly();
    }

    public IProfileModel OwnedProfile => _ownedProfile;
    public ReadOnlyCollection<Ship> Ships => _ships;
    public List<Position> OpenedPositions => _openedPositions;

    //Position, IsShip
    public List<Position> GetField(IProfileModel profileModel)
    {
        var toReturn = new List<Position>();
        if (_ownedProfile.Equals(profileModel))
        {
            toReturn.AddRange(_ships);
        }
        else
        {
            toReturn.AddRange(_ships.Where(ship => ship.IsBroken));
        }
        
        toReturn.AddRange(_openedPositions);
        return toReturn;
    }
}