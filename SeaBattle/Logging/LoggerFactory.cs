namespace SeaBattle.Logging;

public class LoggerFactory
{
    internal static readonly object MessageLock = new();
    public static LoggerFactory FactoryInstance { get; } = new();
    
    private ILogger SelfLogger { get; } = new Logger("LoggerFactory");

    private bool DebugEnabledI;

    public bool DebugEnable
    {
        get => DebugEnabledI;
        set => SelfLogger.Warn($"DebugEnabled value changed to {DebugEnabledI = value}");
    }

    public LoggerFactory(bool debugEnabled = false)
    {
        DebugEnabledI = debugEnabled;
    }

    public ILogger GetLoggerI(string byName)
    {
        return new Logger(byName ?? throw new NullReferenceException("Logger not null"));
    }
    
    public static ILogger GetLogger(string byName)
    {
        return FactoryInstance.GetLoggerI(byName);
    }

    public static ConsoleColor GetColorByLevel(LogLevel level)
    {
        switch (level)
        {
            case LogLevel.DEBUG:
                return ConsoleColor.Cyan;
            case LogLevel.INFO:
                return ConsoleColor.Green;
            case LogLevel.WARNING:
                return ConsoleColor.Yellow;
            case LogLevel.ERROR:
                return ConsoleColor.Red;
            default:
                return ConsoleColor.White;
        }
    }
    
    private class Logger : ILogger
    {
        private string Prefix { get; }
        bool ILogger.DebugEnabled => FactoryInstance.DebugEnable;

        public Logger(string prefix)
        {
            Prefix = prefix;
        }

        public void Debug(string text)
        {
            if (FactoryInstance.DebugEnable)
                Msg(LogLevel.DEBUG, text);
        }

        public void Info(string text)
        {
            Msg(LogLevel.INFO, text);
        }

        public void Warn(string text)
        {
            Msg(LogLevel.WARNING, text);
        }

        public void Error(string text)
        {
            Msg(LogLevel.ERROR, text);
        }

        public void Msg(LogLevel level, string text)
        {
            lock (LoggerFactory.MessageLock)
            {
                Console.ForegroundColor = LoggerFactory.GetColorByLevel(level);
                Console.Write($"[{level}] ");
                Console.ResetColor();
                Console.Write($"[{Thread.CurrentThread.Name}] ");
                Console.Write($"[{Prefix}] ");
                Console.Write(text);
                Console.WriteLine();
            }
        }
    }
}
