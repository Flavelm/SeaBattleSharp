using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SeaBattleClient;

public class SeaClient : IDisposable
{
    public EventHandler<DisconnectReason>? OnDisconnect { get; set; }
    public EventHandler<(string, int)>? OnReceive { get; set; }
    public string Send
    {
        set => TryWrite(value);
    }
    public NetworkStream Stream { get; private set; }
    private TcpClient Client { get; }
    private TimeSpan? Timeout { get; }
    private bool AutoFlush { get; }
    public bool IsDisposed { get; private set; }

    public SeaClient(bool autoFlush = false, int timeout = -1)
    {
        Client = new TcpClient();
        Timeout = timeout < 0 ? null : TimeSpan.FromMilliseconds(timeout);
        AutoFlush = autoFlush;
    }

    public void Connect(IPEndPoint endPoint, bool activateRider = true)
    {
        Client.Connect(endPoint);
        Stream = Client.GetStream();
        IsDisposed = false;
        if (activateRider)
            Read(null);
    }

    public void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        Client.Dispose();
    }
    
    public void Flush()
    {
        CheckDisposeAndThrow();
        Stream.Flush();
    }
    
    public void Disconnect(DisconnectReason reason = DisconnectReason.BY_USER)
    {
        Dispose();
        OnDisconnect?.Invoke(this, reason);
    }

    private DateTime LastReceive = DateTime.Now;

    private void Read(object? state)
    {
        if (IsDisposed)
            return;
        
        DateTime Time = DateTime.Now;
        
        if (!Client.Connected)
        {
            return;
        }

        if (Client.Available > 0)
        {
            LastReceive = DateTime.Now;
            (byte[], int) Received = TryReadAvailable();
            if (Received.Item1.Length == 0 || Received.Item2 == 0)
                Disconnect(DisconnectReason.READ_ERROR);
            (string, int) ReceivedAsStr = (Encoding.UTF8.GetString(Received.Item1), Received.Item2);
            if (OnReceive != null)
                OnReceive.Invoke(this, ReceivedAsStr);
            FlushIfEnabled();
        }

        if (Timeout != null && Time - LastReceive >= Timeout)
        {
            Disconnect(DisconnectReason.READ_TIMEOUT);
        }

        if (!IsDisposed)
        {
            ThreadPool.QueueUserWorkItem(Read);
        }
    }
    
    private void TryWrite(object? s)
    {
        CheckDisposeAndThrow();
        if (s == null)
            return;
        string? StrObj = s.ToString();
        if (StrObj == null)
            return;
        byte[] AsByte = Encoding.UTF8.GetBytes(StrObj);
        try
        {
            lock (Stream)
            {
                FlushIfEnabled();
                Stream.Write(AsByte);
            }
        }
        catch
        {
            Disconnect(DisconnectReason.WRITE_ERROR);
        }
    }

    private (byte[], int) TryReadAvailable()
    {
        try
        {
            lock (Stream)
            {
                byte[] Bytes = new byte[Client.Available];
                int Count = Stream.Read(Bytes);
                return (Bytes, Count);
            }
        }
        catch
        {
            return (new byte[0], 0);
        }
    }

    private void FlushIfEnabled()
    {
        if (AutoFlush)
            Stream.Flush();
    }
   
    private void CheckDisposeAndThrow()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(SeaClient));
    }
}

public enum DisconnectReason
{
    READ_TIMEOUT, READ_ERROR, WRITE_ERROR, BY_USER
}
