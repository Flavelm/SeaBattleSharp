using Newtonsoft.Json;

namespace SeaBattle.Server.PacketsUtils.Packets;

public class PingPacket : IPacket, IProcessable
{
    public int Type { get; } = 101;

    //Unix milliseconds
    public long Time;
    
    public IPacket? Apply(string json)
    {
        return JsonConvert.DeserializeObject<PingPacket>(json);
    }
    
    public string Process()
    {
        object Response = new
        {
            Type,
            Ping = (DateTimeOffset.Now.ToUnixTimeMilliseconds() - Time)
        };
        string Process = JsonConvert.SerializeObject(Response);
        return Process;
    }
}