using System.Net.WebSockets;
using SeaBattleWeb.Models;

namespace SeaBattleWeb.Services.Play;

public class PlayFieldService(IServiceProvider provider)
{
    private readonly List<FieldService> _fields = new();
    
    public DateTime LastActivity { get; private set; }

    public bool IsReady
    {
        get
        {
            lock (_fields)
            {
                if (_fields.Count == 0)
                    return false;
                return _fields.All(field => field.IsReady);
            }
        }
    }

    public Task ProcessSocket(IProfileModel profile, WebSocket socket)
    {
        return SetupField(socket, profile);
    }
    
    public async Task SetupField(WebSocket socket, IProfileModel profileModel)
    {
        FieldService service = provider.GetRequiredService<FieldService>();
        service.FieldUpdated += SyncOnFieldUpdated;
        lock (_fields)
        {
            if (_fields.Count >= 1)
                foreach (var fieldService in _fields)
                    fieldService.Socket.QuickSend(new { OpponentJoined = true });
            _fields.Add(service);
        }

        LastActivity = DateTime.Now;
        await service.SetupPlayer(socket, profileModel);
    }

    private void SyncOnFieldUpdated(object? sender, FieldServiceEventArgs e)
    {
        if (e.Type == FieldServiceEventType.FieldConfigured)
        {
            lock (_fields)
            {
                foreach (var fieldService in _fields.Where(o => o != e.Instance))
                    fieldService.Socket.QuickSend(new { OpponentJoined = true });
                if (IsReady)
                    foreach (var field in _fields)
                        field.Socket.QuickSend(new { Ready = true });
            }
        }
        else if (e.Type is FieldServiceEventType.ShipBroken)
        {
            lock (_fields)
            {
                for (int i = 0; i < _fields.Count; i++)
                {
                    _fields[i.Reverse()].Sync(_fields[i].Field.OwnedProfile);
                    _fields[i].Sync();
                }
            }
        }
        LastActivity = DateTime.Now;
    }
}