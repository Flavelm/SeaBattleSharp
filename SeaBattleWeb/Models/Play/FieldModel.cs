using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;

namespace SeaBattleWeb.Models.Play;

public class FieldModel
{
    private IProfileModel _ownedProfile;
    private readonly IDictionary<Position, Ship> _ships;

    public IProfileModel OwnedProfile
    {
        get => _ownedProfile;
        init => _ownedProfile = value is { Connection: not null }
            ? value 
            : throw new ArgumentException("Only connected clients");
    }
    public IDictionary<Position, Ship> Ships
    {
        get => _ships;
        init => _ships = value.AsReadOnly();
    }

    public void RefreshField() => RefreshField(this);

    public void RefreshField(FieldModel model)
    {
        string message;
        if (this == model)
            message = JsonConvert.SerializeObject(
                new { YourFieldUpdate = Ships });
        else
            message = JsonConvert.SerializeObject(
                new { OppenentFieldUpdate = model.Ships.Where(pair => pair.Value.IsBroken) });
        ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        OwnedProfile.Connection!.SendAsync(
            buffer,
            WebSocketMessageType.Text,
            WebSocketMessageFlags.EndOfMessage,
            CancellationToken.None);
    }
}