using SeaBattleWeb.Models;

namespace SeaBattleWeb.Hubs;

public interface IClientRoomHub
{
    public void HandleError(string error);
    public void HandleError(PositionDto newField);
}