using SeaBattleWeb.Models.Play.Models.Play;
using SeaBattleWeb.Services.Play;

namespace SeaBattleWeb.Services;

public interface IRoomService
{
    DateTime LastActivity { get; }
    RoomState RoomState { get; }
    
    event EventHandler<FieldServiceEventArgs>? FieldUpdated;

    void Join(FieldModel fieldModel);
}