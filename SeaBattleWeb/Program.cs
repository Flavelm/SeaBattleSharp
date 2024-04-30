namespace SeaBattleWeb;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        if (builder.Environment.IsProduction())
        {
            string? domain = Environment.GetEnvironmentVariable("ORIGINS");
            if (domain == null)
            {
                Console.Error.WriteLine("Origins not installed");
                Environment.Exit(1);
            }
            
            builder.Environment.ContentRootPath = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine($"Set environment ContentRootPath (to {builder.Environment.ContentRootPath}) for production");
            builder.Configuration.AddJsonFile(builder.Environment.ContentRootPath + "appsettings.json");
            Console.WriteLine($"Set configuration (to {builder.Environment.ContentRootPath + "appsettings.json"}) for production");
        }
        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);
        var app = builder.Build();
        startup.Configure(app, builder.Environment);
        app.Run();
    }
}