using System.Collections.ObjectModel;
using SeaBattleWeb.Models;
using SeaBattleWeb.Models.Play.Models.Play;
using SeaBattleWeb.Services.Play;

namespace SeaBattleWeb.Services;

public interface IRoomService
{
    DateTime LastActivity { get; }
    RoomState RoomState { get; }
    Guid RoomId { get; }
    public ReadOnlyCollection<FieldService> FieldServices { get; }
    
    event EventHandler<FieldServiceEventArgs>? FieldUpdated;

    FieldService GetByOwner(ProfileModel profileModel);
    FieldService GetByOther(ProfileModel profileModel);
    
    bool CanShot(ProfileModel profileModel);
    
    void Join(FieldModel fieldModel);
}