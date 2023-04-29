using System.Dynamic;
using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SeaBattle.Logging;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SeaBattle.Config;

public class Configuration
{
    public static Configuration Default { get; } = new()
    {
        ListenIp = "127.0.0.1",
        ListenPort = 5400,
        Backlog = 50,
        Timeout = 1000,
        Debug = false
    };

    public static ILogger? Logger
    {
        get => Logger;
        set => Logger = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string ListenIp;
    public int ListenPort;
    public int Backlog;
    public int Timeout;
    public bool Debug;

    public static Configuration? FromFile(string path)
    {
        string Text = File.ReadAllText(path);
        Configuration? Conf = JsonConvert.DeserializeObject<Configuration>(Text);
        return Conf;
    }

    public static void SaveDefault(string path, bool rewrite)
    {
        bool Exists = File.Exists(path);
        if (rewrite)
        {
            File.WriteAllText(path, JsonSerializer.Serialize<dynamic>(Default));
            return;
        }
        if (!Exists)
        {
            File.WriteAllText(path, JsonSerializer.Serialize<dynamic>(Default));
        }
    }

    private static void Log(LogLevel level, string text)
    {
        Logger?.Info(text);
    }
}