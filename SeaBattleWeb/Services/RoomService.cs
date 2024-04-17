using System.Net.WebSockets;
using System.Text;
using SeaBattleWeb.Models;
using SeaBattleWeb.Services.Play;

namespace SeaBattleWeb.Services;

public class RoomService(ILogger<RoomService> logger, PlayFieldService playFieldService) : IRoomService
{
    public DateTime LastActivity => playFieldService.LastActivity;
    public bool IsReady => playFieldService.IsReady;

    public async Task ProcessSocket(IProfileModel profileModel, WebSocket webSocket)
    {
        await playFieldService.SetupField(webSocket, profileModel);
    }
}