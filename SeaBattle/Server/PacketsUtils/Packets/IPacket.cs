namespace SeaBattle.Server.PacketsUtils.Packets;

public interface IPacket
{
    int Type { get; }
    
    static readonly IDictionary<PacketsTypes, IPacket> Packets = new Dictionary<PacketsTypes, IPacket>
    {
        { PacketsTypes.GET_ROOM, new GetRoomPacket() },
        { PacketsTypes.PING, new PingPacket() },
    };
    
    static IPacket? Parse(int id, string json)
    {
        return Packets[(PacketsTypes)id].Apply(json);
    }

    IPacket? Apply(string json);
}