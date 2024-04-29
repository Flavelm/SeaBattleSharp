namespace SeaBattleWeb;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        if (builder.Environment.IsProduction())
        {
            builder.Environment.ContentRootPath = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine($"Set environment ContentRootPath (to {builder.Environment.ContentRootPath}) for production");
        }
        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);
        var app = builder.Build();
        startup.Configure(app, builder.Environment);
        app.Run();
    }
}