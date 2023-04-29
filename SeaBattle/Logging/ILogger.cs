namespace SeaBattle.Logging;

public interface ILogger
{
    public bool DebugEnabled { get; }
    
    void Debug(string text);
    void Info(string text);
    void Warn(string text);
    void Error(string text);
    void Msg(LogLevel level, string msg);
}