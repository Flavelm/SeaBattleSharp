using System.Net.WebSockets;
using System.Text;
using SeaBattleWeb.Models;
using SeaBattleWeb.Services.Play;

namespace SeaBattleWeb.Services;

public class RoomService(ILogger<RoomService> logger, PlayFieldService playFieldService) : IRoomService
{
    public bool CanConnect { get; private set; }
    public int ConnectedWebsocketCount { get; private set; }
    public DateTime LastActivity => playFieldService.LastActivity;
    public bool IsReady => playFieldService.IsReady;

    public async Task ProcessSocket(IProfileModel profileModel, WebSocket webSocket)
    {
        ConnectedWebsocketCount++;
        CanConnect = ConnectedWebsocketCount >= 2;
        await playFieldService.SetupField(webSocket, profileModel);
    }
}