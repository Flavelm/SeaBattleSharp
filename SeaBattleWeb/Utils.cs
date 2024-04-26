using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using SeaBattleWeb.Models;
using SeaBattleWeb.Models.Base;

namespace SeaBattleWeb;

public static class Utils
{
    public static async Task QuickSendAsync(this WebSocket webSocket, object obj)
    {
        await webSocket.QuickSendAsync(obj, CancellationToken.None);
    }
    
    public static async Task QuickSendAsync(this WebSocket webSocket, object obj, CancellationToken cancellationToken)
    {
        if (obj is NotificationType notificationType)
            obj = new NotificationDto(notificationType);
        
        string? json = obj is Serializable dto ? dto.Serialize() : obj.ToString();
        
        if (json == null)
            throw new Exception("What?");
        
        byte[] buffer = Encoding.UTF8.GetBytes(json);
        await webSocket.SendAsync(
            new ArraySegment<byte>(buffer),
            WebSocketMessageType.Text,
            WebSocketMessageFlags.EndOfMessage,
            cancellationToken);
    }
    
    public static async Task QuickSend(this WebSocket webSocket, object obj)
    {
        await webSocket.QuickSendAsync(obj, CancellationToken.None);
    }
    
    public static async Task<byte[]> QuickReceiveAsync(this WebSocket webSocket)
    {
        return await webSocket.QuickReceiveAsync(CancellationToken.None);
    }
    
    public static async Task<byte[]> QuickReceiveAsync(this WebSocket webSocket, CancellationToken cancellationToken)
    {
        List<byte> buffer = new List<byte>();

        WebSocketReceiveResult currentReceiveResult;

        do
        {
            var crr = await webSocket.QuickReceivePartAsync(cancellationToken);
            currentReceiveResult = crr.Item1;
            var currentBuffer = crr.Item2;

            ArraySegment<byte> array = new ArraySegment<byte>(currentBuffer, 0, currentReceiveResult.Count);
            
            buffer.AddRange(array);
        } while (!currentReceiveResult.EndOfMessage);

        return buffer.ToArray();
    }
    
    private static async Task<(WebSocketReceiveResult, byte[])> QuickReceivePartAsync(this WebSocket webSocket, CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[1024];

        WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), cancellationToken);
        
        return (receiveResult, buffer);
    }
    
    public static async Task QuickCloseAsync(this WebSocket webSocket, object obj)
    { 
        await webSocket.QuickCloseAsync(obj, CancellationToken.None);
    }
    
    public static async Task QuickCloseAsync(this WebSocket webSocket, object obj, CancellationToken cancellationToken)
    {
        string? json = obj is not string
            ? JsonConvert.SerializeObject(obj)
            : obj as string;
        await webSocket.CloseAsync(
            WebSocketCloseStatus.EndpointUnavailable,
            json,
            cancellationToken);
    }

    public static int Reverse(this int num) => num == 0 ? 1 : 0;
}