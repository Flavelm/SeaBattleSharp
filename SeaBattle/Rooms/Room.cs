using Newtonsoft.Json;
using SeaBattle.Server.PacketsUtils.Packets;

namespace SeaBattle.Rooms;

public class Room
{
    private Fields Fields = new();

    public string ProcessPacket(GetRoomPacket packet)
    {
        switch (packet.Player)
        {
            case 0: return ToStringPlayerOne(packet.Type);
            case 1: return ToStringPlayerTwo(packet.Type);
        }

        return "";
    }

    public string ToStringPlayerOne(int packetId)
    {
        object Field = new
        {
            Type = packetId,
            FieldOne = Fields.FirstField,
            FieldTwo = Fields.GetSecondWithoutShips()
        };
        return JsonConvert.SerializeObject(Field);
    }

    public string ToStringPlayerTwo(int packetId)
    {
        object Field = new
        {
            Type = packetId,
            FieldOne = Fields.GetFirstWithoutShips(),
            FieldTwo = Fields.SecondField
        };
        return JsonConvert.SerializeObject(Field);
    }
}