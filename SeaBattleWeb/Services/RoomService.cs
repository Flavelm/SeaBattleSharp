using System.Net.WebSockets;
using SeaBattleWeb.Models;

namespace SeaBattleWeb.Services;

public class RoomService(ILogger<RoomService> logger) : IRoomService, IHostedService
{
    private readonly IDictionary<ProfileModel, WebSocket> _viewers = new Dictionary<ProfileModel, WebSocket>();
    private Thread? _reader;
    private CancellationTokenSource _cancellationTokenSource = new();
    
    private void ReadWebSockets()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            List<Task> tasks = new List<Task>();
            
            lock (_viewers)
            {
                foreach (var keyPair in _viewers)
                {
                    WebSocket webSocket = keyPair.Value;
                    if (webSocket.State == WebSocketState.Closed)
                        continue;

                    tasks.Add(ProcessSocket(keyPair.Key, webSocket));
                }
            }

            foreach (var task in tasks)
                task.Wait();
        }

        foreach (var viewer in _viewers.Values)
            viewer.CloseAsync(WebSocketCloseStatus.Empty, "Reader shutdown", new CancellationToken());
    }

    private async Task ProcessSocket(ProfileModel profileModel, WebSocket webSocket)
    {
        var buffer = new byte[1024];
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
        }

        await webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }
    
    public async Task HandleViewer(ProfileModel profile, WebSocket socket)
    {
        logger.LogInformation("Connected viewer");
        lock (_viewers)
        {
            if (_viewers.Keys.FirstOrDefault(pm => pm.Id == profile.Id, null) != null)
                _viewers.Add(profile, socket);
            else
                socket.CloseAsync(WebSocketCloseStatus.EndpointUnavailable,
                    "You already connected",
                    new CancellationToken());
        }
        
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting websocket reader");
        _reader = new Thread(ReadWebSockets);
        _reader.Name = "Socket reader";
        _reader.Start();
        logger.LogInformation("Started websocket reader");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping websocket reader");
        _cancellationTokenSource.Cancel();
        if (_reader == null)
            return Task.CompletedTask;
        _reader.Join();
        logger.LogInformation("Stopped websocket reader");
        return Task.CompletedTask;
    }
}