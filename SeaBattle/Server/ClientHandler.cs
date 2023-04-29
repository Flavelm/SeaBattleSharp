using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using SeaBattle.Logging;
using SeaBattle.Server.PacketsUtils;
using SeaBattle.Server.PacketsUtils.Packets;

namespace SeaBattle.Server;

public class ClientHandler : IDisposable
{
    private ILogger Log { get; }
    public bool Disposed { get; private set; }
    private TimeSpan Timeout { get; }
    private TcpClient Client { get; }
    private int MaxBufferSize { get; }
    
    private DateTime LastPacketTime { get; set; } = DateTime.Now;
    private NetworkStream Stream { get; set; }
    private (char, byte) Separator { get; }
    private List<byte> Buffer { get; }

    protected internal ClientHandler(TcpClient client, int timeout, string? clientId = null)
    {
        Log = LoggerFactory.GetLogger("ClientHandler");
        Log.Info("Connected " + client.Client.RemoteEndPoint);
        Client = client;
        
        Timeout = TimeSpan.FromMilliseconds(timeout);
        Client.ReceiveTimeout = timeout;

        MaxBufferSize = 5120; //TODO("Move to config");
        Client.ReceiveBufferSize = MaxBufferSize;
        Client.SendBufferSize = MaxBufferSize;

        Stream = client.GetStream();
        Separator = (';', Encoding.UTF8.GetBytes(";")[0]);
        Buffer = new();
        ThreadPool.QueueUserWorkItem(Read);   
    }
    
    private void Read(object? state)
    {
        if (!Client.Connected && Disposed)
            return;

        if (CheckTime())
        {
            Log.Info("Disconnected for timeout");
            Dispose();
            return;
        }
        
        if (Buffer.Count >= MaxBufferSize)
        {
            Log.Warn($"-------Buffer droped from len {Buffer.Count} drop len {MaxBufferSize}-------");
            Buffer.Clear();
            Stream.Flush();
            TryWriteOrClose(Stream, Errors.BufferDroppedBytes);
        }

        if (Client.Available > 0)
        {
            LastPacketTime = DateTime.Now;
            IEnumerable<byte>? NewBytes = TryReadOrClose(Stream, Client.Available);
            if (NewBytes == null)
                return;
            var NewBytesToAdd = NewBytes as byte[] ?? NewBytes.ToArray();
            if (NewBytesToAdd.Length > MaxBufferSize)
            {
                Log.Info("Disconnected for spam");
                Dispose();
                return;
            }
            Buffer.AddRange(NewBytesToAdd);
            if (Buffer.Contains(Separator.Item2))
            {
                byte[] BufferAsArray = Buffer.ToArray();
                Buffer.Clear();
                string TextFromBuffer = Encoding.UTF8.GetString(BufferAsArray);
                string[] SplitedText = TextFromBuffer.Split(Separator.Item1);
                foreach (var ToHandle in SplitedText)
                {
                    if (string.IsNullOrWhiteSpace(ToHandle))
                        continue;
                    byte[]? Bytes = Handle(ToHandle);
                    //int Len = Bytes?.Length ?? 0;
                    Log.Debug($"ToHandle {ToHandle} from {Client.Client.RemoteEndPoint} with response: {Encoding.UTF8.GetString(Bytes ?? new byte[0])}");
                    if (Bytes != null)
                    {
                        TryWriteOrClose(Stream, Bytes);
                    }
                }
            }
        }

        if (!Disposed)
            ThreadPool.QueueUserWorkItem(Read);
    }

    private bool CheckTime()
    {
        return DateTime.Now - LastPacketTime > Timeout;
    }

    //Returns response
    private byte[]? Handle(string text)
    {
        IPacket? D = PacketFactory.Construct(text);
        if (D != null)
        {
            if (D is IProcessable)
                return Encoding.UTF8.GetBytes(((IProcessable)D).Process() + ";");
            string Response = Program.Room.ProcessPacket(D.AsGetRoomPacket()) + ";";
            Log.Info($"Response: {Response}");
            return Encoding.UTF8.GetBytes(Response);
        }
        return null;
    }
    
    private IEnumerable<byte>? TryReadOrClose(NetworkStream net, int len)
    {
        try
        {
            byte[] Bytes = new byte[len];
            int Read = net.Read(Bytes, 0, len);
            return Bytes.Take(Read);
        }
        catch { Dispose(); }

        return null;
    }
    
    private void TryWriteOrClose(NetworkStream net, byte[] bytes)
    {
        try
        {
            net.Write(bytes, 0, bytes.Length);
        }
        catch { Dispose(); }
    }
    
    public void Dispose()
    {
        if (Disposed)
            return;
        EndPoint? EndPoint = Client.Client.RemoteEndPoint;
        Client.Dispose();
        Buffer.Clear();
        Disposed = true;
        Log.Info($"Disconnected {EndPoint}");
    }
}