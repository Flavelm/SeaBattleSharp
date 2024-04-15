namespace SeaBattleWeb.Models;

public class Ship
{
    public ProfileModel OwnedProfile;
    public ShipState State { get; private set; }
    public Position Pos { get; }
}