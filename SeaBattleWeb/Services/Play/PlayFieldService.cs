using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using SeaBattleWeb.Models;
using SeaBattleWeb.Models.Play;

namespace SeaBattleWeb.Services.Play;

public class PlayFieldService(IServiceProvider provider)
{
    private readonly List<FieldService> _fields = new();
    
    public DateTime LastActivity { get; private set; }

    public bool IsReady => _fields.All(field => field.IsReady);
    
    public async Task SetupField(WebSocket socket, IProfileModel profileModel)
    {
        FieldService service = provider.GetRequiredService<FieldService>();
        service.FieldUpdated += SyncOnFieldUpdated;
        service.SetupPlayer(socket, profileModel);
        lock (_fields)
        {
            if (_fields.Count >= 1)
                foreach (var fieldService in _fields)
                    fieldService.Socket.QuickSend(new { OpponentJoined = true });
            _fields.Add(service);
        }
    }

    private void SyncOnFieldUpdated(object? sender, FieldServiceEventArgs e)
    {
        if (e.Type == FieldServiceEventType.FieldConfigured)
            lock (_fields)
                foreach (var fieldService in _fields.Where(o => o != e.Instance))
                    fieldService.Socket.QuickSend(new { OpponentJoined = true });
        lock (_fields)
            if (IsReady)
                foreach (var field in _fields)
                    field.Socket.QuickSend(new { Ready = true });
    }
}