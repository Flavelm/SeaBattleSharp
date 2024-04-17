namespace SeaBattleWeb.Models;

public class Ship(IProfileModel ownedBy, Position position)
{
    public ProfileModel OwnedProfile { get; }
    public bool IsWrecked { get; private set; }
    public Position Position { get; }

    public bool IsOwnedBy(ProfileModel other)
    {
        return OwnedProfile == other;
    }
}