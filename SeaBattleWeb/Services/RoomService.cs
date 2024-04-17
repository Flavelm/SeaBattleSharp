using System.Net.WebSockets;
using System.Text;
using SeaBattleWeb.Models;
using SeaBattleWeb.Services.Play;

namespace SeaBattleWeb.Services;

public class RoomService(ILogger<RoomService> logger, PlayFieldService playFieldService) : IRoomService
{
    private readonly IDictionary<IProfileModel, WebSocket> _viewers = new Dictionary<IProfileModel, WebSocket>();
    public DateTime LastActivity { get; private set; } = playFieldService.LastActivity;

    public async Task ProcessSocket(IProfileModel profileModel, WebSocket webSocket)
    {
        lock (_viewers)
            _viewers.Add(profileModel, webSocket);
        logger.LogInformation("Connection process!");

        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                receiveResult.MessageType,
                receiveResult.EndOfMessage,
                CancellationToken.None);
            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
            
            LastActivity = DateTime.Now;
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
}