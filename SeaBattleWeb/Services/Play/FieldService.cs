using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using SeaBattleWeb.Models;
using SeaBattleWeb.Models.Play;

namespace SeaBattleWeb.Services.Play;

public class FieldService(PlayFieldService playFieldService, ILogger<FieldService> logger)
{
    private readonly PlayFieldService PlayFieldService = playFieldService;

    private WebSocket? _socket;
    private FieldModel _field;
    
    public bool IsReady => _field != null;
    public WebSocket Socket => _socket;
    public FieldModel Field => _field;
    
    public event EventHandler<FieldServiceEventArgs> FieldUpdated;

    public async Task Sync(IProfileModel profileModel)
    {
        Socket.QuickSend(new { OpponentField = Field.GetField(profileModel) });
    }
    
    public async Task Sync()
    {
        Socket.QuickSend(new { YourField = Field.GetField(Field.OwnedProfile) });
    }

    public async Task SetupPlayer(WebSocket socket, IProfileModel profileModel)
    {
        logger.LogDebug("Setup player {} ready", _field.OwnedProfile.IdUsername);
        var buffer = new byte[1024 * 4];
        var receiveResult = await socket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        KeyValuePair<string, List<Ship>>? ships;
        try
        {
            string json = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            ships = JsonConvert.DeserializeObject<KeyValuePair<string, List<Ship>>>(json);
        }
        finally { }
        
        if (!ships.HasValue || ships.Value.Key != "YourField")
        {
            socket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, null, CancellationToken.None);
            return;
        }


        _field = new FieldModel(profileModel, ships.Value.Value); //Todo validate
        
        FieldUpdated.Invoke(this, new FieldServiceEventArgs()
        {
            Instance = this,
            Type = FieldServiceEventType.FieldConfigured
        });
        
        logger.LogDebug("Field {} ready", _field.OwnedProfile.IdUsername);

        await Read(socket, profileModel);
    }

    private async Task Read(WebSocket socket, IProfileModel profileModel)
    {
        await SetupPlayer(socket, profileModel);
        
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