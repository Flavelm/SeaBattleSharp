namespace SeaBattleWeb.Models.Play;

public class FieldModel
{
    private IProfileModel _ownedProfile;
    private readonly IDictionary<Position, Ship> _ships;
    private readonly List<Position> _openedPositions = new();

    public FieldModel(IProfileModel ownedProfile, IDictionary<Position, Ship> ships)
    {
        _ownedProfile = ownedProfile;
        _ships = ships;
    }

    public IProfileModel OwnedProfile => _ownedProfile;
    public IDictionary<Position, Ship> Ships => _ships;
    public IDictionary<Position, Ship> OpenedPositions
    {
        get => _ships;
        init => _ships = value.AsReadOnly();
    }

    public bool GetField(IProfileModel profileModel)
    {
        return _ownedProfile.Equals(profileModel);
    }
}