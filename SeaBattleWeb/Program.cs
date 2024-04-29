namespace SeaBattleWeb;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        if (builder.Environment.IsProduction())
        {
            string configPath = $"{AppDomain.CurrentDomain.BaseDirectory}/appsettings.json";
            Console.WriteLine($"Add configuration {configPath}");
            builder.Configuration.AddJsonFile(configPath);
        }
        var startup = new Startup(builder.Configuration);
        startup.ConfigureServices(builder.Services);
        var app = builder.Build();
        startup.Configure(app, builder.Environment);
        app.Run();
    }
}