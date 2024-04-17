namespace SeaBattleWeb.Models.Play;

public class FieldModel
{
    private readonly IDictionary<Position, Ship?> _ships;
    
    public ProfileModel OwnedProfile { get; init; }

    public IDictionary<Position, Ship?> Ships
    {
        get => _ships;
        init => _ships = value.AsReadOnly();
    }
}