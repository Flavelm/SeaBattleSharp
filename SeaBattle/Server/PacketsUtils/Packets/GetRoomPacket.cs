using Newtonsoft.Json;

namespace SeaBattle.Server.PacketsUtils.Packets;

public class GetRoomPacket : IPacket
{
    public int Type { get; } = 100;
    
    public int Player;

    public IPacket? Apply(string json)
    {
        return JsonConvert.DeserializeObject<GetRoomPacket>(json);
    }
}