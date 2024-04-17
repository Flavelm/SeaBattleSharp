using NuGet.Packaging;

namespace SeaBattleWeb.Models.Play;

public class FieldModel
{
    private IProfileModel _ownedProfile;
    private readonly IDictionary<Position, Ship> _ships;
    private readonly List<Position> _openedPositions = new();

    public FieldModel(IProfileModel ownedProfile, IDictionary<Position, Ship> ships)
    {
        _ownedProfile = ownedProfile;
        _ships = ships.AsReadOnly();
    }

    public IProfileModel OwnedProfile => _ownedProfile;
    public IDictionary<Position, Ship> Ships => _ships;
    public List<Position> OpenedPositions => _openedPositions;

    //Position, IsShip
    public IDictionary<Position, bool> GetField(IProfileModel profileModel)
    {
        var toReturn = new Dictionary<Position, bool>();
        if (_ownedProfile.Equals(profileModel))
        {
            toReturn.AddRange(
                _ships.Select(pair => new KeyValuePair<Position, bool>(pair.Key, true))
            );
        }
        else
        {
            toReturn.AddRange(
                _ships.Select(pair => new KeyValuePair<Position, bool>(pair.Key, pair.Value.IsBroken))
            );
        }
        
        toReturn.AddRange(_openedPositions.Select(pos => new KeyValuePair<Position, bool>(pos, false)));
        return toReturn;
    }
}