using System.Net.WebSockets;
using System.Text;
using SeaBattleWeb.Models;
using SeaBattleWeb.Services.Play;

namespace SeaBattleWeb.Services;

public class RoomService(ILogger<RoomService> logger, PlayFieldService playFieldService) : IRoomService
{
    private int _connectionsCount = 0;
    
    public DateTime LastActivity => playFieldService.LastActivity;
    public bool IsReady => playFieldService.IsReady;

    public async Task Authorize(WebSocket socket, IProfileModel profileModel)
    {
        if (_connectionsCount <= 1) ;
    }

    public async Task ProcessSocket(IProfileModel profileModel, WebSocket webSocket)
    {
        await playFieldService.ProcessSocket(webSocket, profileModel);
    }
}