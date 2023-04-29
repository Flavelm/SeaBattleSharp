using System.Net.Mime;
using System.Runtime.ExceptionServices;
using SeaBattle.Config;
using SeaBattle.Logging;
using SeaBattle.Rooms;
using SeaBattle.Server;

namespace SeaBattle;

public class Program
{
    public static Room Room { get; } = new();

    public static void Main(string[] args)
    {
        Thread.CurrentThread.Name = "Starter";
        new Program().Start(args);
    }

    private ILogger Logger = LoggerFactory.GetLogger("StartThread");
    private SeaServer? Server { get; set; }

    private void Start(string[] args)
    {
        Logger.Info("Starting...");
        Logger.Info("Config loading...");
        Configuration.SaveDefault("config.json", false);
        Configuration? Conf = Configuration.FromFile("config.json");
        
        if (Conf == null)
        {
            Logger.Error("Config not found!");
            return;
        }

        LoggerFactory.FactoryInstance.DebugEnable = Conf.Debug; 
        
        ThreadPool.SetMaxThreads(Conf.Backlog, 0);
        
        Logger.Info("Setup exception handler...");
        AppDomain.CurrentDomain.FirstChanceException += ExceptionHandler;
        
        Logger.Info($"Bind interface: {Conf.ListenIp}:{Conf.ListenPort}");
        Server = new SeaServer(Conf);
        Server.Start();

        Console.CancelKeyPress += ConsoleOnCancelKeyPress;

        while (true)
        {
            Thread.Sleep(10000);
            Logger.Debug($"Thread count: {ThreadPool.ThreadCount}");
            Logger.Debug($"Pending threads count: {ThreadPool.PendingWorkItemCount}");
            Logger.Debug($"Completed threads count: {ThreadPool.CompletedWorkItemCount}");
        }
    }

    private void ConsoleOnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        if (Server != null)
            Server.Stop();
    }

    private void ExceptionHandler(object? sender, FirstChanceExceptionEventArgs e)
    {
        Logger.Error("Error: ");
        Logger.Error("   " + e.Exception.Message);
        if (e.Exception.StackTrace != null) Logger.Error(e.Exception.StackTrace);
    }
}