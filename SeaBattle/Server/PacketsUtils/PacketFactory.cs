using SeaBattle.Server.PacketsUtils.Packets;

namespace SeaBattle.Server.PacketsUtils;

public static class PacketFactory
{
    public static IPacket? Construct(string text)
    {
        List<string> D = text.Split(':').ToList();
        if (D.Count >= 2 && D[1].StartsWith("{") && D[^1].EndsWith("}"))
        {
            int PacketId = int.Parse(D[0]);
            D.Remove(D[0]);
            string ToParse = string.Join(":", D);
            return IPacket.Parse(PacketId, ToParse);
        }

        return null;
    }

    public static GetRoomPacket AsGetRoomPacket(this IPacket packet)
    {
        return (GetRoomPacket)packet;
    }
}