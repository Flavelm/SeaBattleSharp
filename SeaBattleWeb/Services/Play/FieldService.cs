using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using SeaBattleWeb.Models;
using SeaBattleWeb.Models.Play;

namespace SeaBattleWeb.Services.Play;

public class FieldService(PlayFieldService playFieldService)
{
    private readonly PlayFieldService PlayFieldService = playFieldService;

    private WebSocket? _socket;
    private FieldModel _field;
    
    public bool IsReady => _field != null;
    public WebSocket Socket => _socket;
    
    public event EventHandler<FieldServiceEventArgs> FieldUpdated;

    public async Task SetupPlayer(WebSocket socket, IProfileModel profileModel)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await socket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        IDictionary<Position, Ship>? ships = null;
        try
        {
            string json = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            ships = JsonConvert.DeserializeObject<Dictionary<Position, Ship>>(json);
        }
        finally { }
        
        if (ships == null)
        {
            socket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, null, CancellationToken.None);
            return;
        }
        
        _field = new FieldModel(profileModel, ships); //Todo validate
        FieldUpdated.Invoke(this, new FieldServiceEventArgs()
        {
            Instance = this,
            Type = FieldServiceEventType.FieldConfigured
        });
        
        Read();
    }

    private async Task Read()
    {
        if (_socket == null)
            throw new ArgumentException("Socket didn't open!");
        
        while (!_socket.CloseStatus.HasValue)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await _socket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
            
            dynamic? ships = null;
            try
            {
                string json = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                ships = JsonConvert.DeserializeObject<dynamic>(json);
            }
            finally { } //Todo command handle
            
            FieldUpdated.Invoke(this, new FieldServiceEventArgs()
            {
                Instance = this,
                Type = FieldServiceEventType.ShipBroken
            });
        }

        await _socket.CloseAsync(
            _socket.CloseStatus.Value,
            _socket.CloseStatusDescription,
            CancellationToken.None);
    } 
}