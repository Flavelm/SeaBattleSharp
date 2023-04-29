using System.Net;
using System.Net.Sockets;
using SeaBattle.Config;
using SeaBattle.Logging;

namespace SeaBattle.Server;

class SeaServer
{
    private ILogger Logger { get; } = LoggerFactory.FactoryInstance.GetLoggerI("SeaServer");
    private bool StopRequested;
    private readonly Configuration Conf;
    private readonly TcpListener SocketServer;
    private readonly List<ClientHandler> Handlers = new();

    public SeaServer(Configuration configuration, bool connect = false)
    {
        Conf = configuration;
        StopRequested = false;
        SocketServer = new TcpListener(IPAddress.Parse(configuration.ListenIp), configuration.ListenPort);
        if (connect)
        {
            Start();
        }
    }

    public void Stop()
    {
        StopRequested = true;
        Logger.Info("Stop called");
        foreach (var Handler in Handlers)
        {
            Handler.Dispose();
        }
    }
    
    public void Start()
    {
        ThreadPool.QueueUserWorkItem(Check);
        Logger.Info("Starting listener...");
        SocketServer.Start(Conf.Backlog);
        SocketServer.BeginAcceptTcpClient(Accept, null);
    }

    private void Accept(IAsyncResult asyncResult)
    {
        TcpClient Client = SocketServer.EndAcceptTcpClient(asyncResult);
        Handlers.Add(new ClientHandler(Client, Conf.Timeout));
        SocketServer.BeginAcceptTcpClient(Accept, null);
    }

    private void Check(object? status)
    {
        if (StopRequested)
            return;
        Handlers.RemoveAll(item => item.Disposed);
        Thread.Sleep(5000); //TODO("Add to config");
        ThreadPool.QueueUserWorkItem(Check);
    }
}