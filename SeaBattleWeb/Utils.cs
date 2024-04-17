using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;

namespace SeaBattleWeb;

public static class Utils
{
    public static async Task QuickSend(this WebSocket webSocket, object obj)
    {
        string? json = obj is not string
            ? JsonConvert.SerializeObject(obj)
            : obj as string;
        if (json == null)
            throw new Exception("What?");
        byte[] buffer = Encoding.UTF8.GetBytes(json);
        await webSocket.SendAsync(
            new ArraySegment<byte>(buffer),
            WebSocketMessageType.Text,
            WebSocketMessageFlags.EndOfMessage,
            CancellationToken.None);
    }
}