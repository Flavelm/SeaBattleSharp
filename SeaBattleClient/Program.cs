using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SeaBattleClient;

public class Program
{
    private SeaClient Client = new(true, -1);

    private static void Main(string[] args)
    {
        new Program().Main();
    }
    
    private void Main()
    {
        try
        {
            Client.OnReceive += OnReceive;
            Client.OnDisconnect += OnDisconnect;
            Client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5400));

            while (true)
            {
                string Send = "101:" + JsonSerializer.Serialize(new { Time = DateTimeOffset.Now.ToUnixTimeMilliseconds() }) + ";";
                Console.WriteLine("Send: " + Send);
                Client.Send = Send;
                Thread.Sleep(1000);
                if (Client.IsDisposed)
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void OnDisconnect(object? sender, DisconnectReason e)
    {
        Console.WriteLine("Disconnect " + e);
    }

    private void OnReceive(object? sender, (string, int) e)
    {
        Console.WriteLine("Receive: " + e.Item1);
    }
}