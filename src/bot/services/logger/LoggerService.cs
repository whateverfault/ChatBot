using ChatBot.bot.interfaces;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using TwitchAPI.client;

namespace ChatBot.bot.services.logger;

public delegate void LogHandler(Log log);

public class LoggerService : Service {
    public event LogHandler? OnLog;
    
    public override string Name => ServiceName.Logger;
    public override LoggerOptions Options { get; } = new LoggerOptions();
    
    
    public void Log(LogLevel logLevel, string message) {
        if (Options.ServiceState == State.Disabled) return;
        if (logLevel < Options.LogLevel) return;
        
        var log = new Log(logLevel, DateTime.Now, message);
        
        Options.AddLog(log);
        OnLog?.Invoke(log);
    }

    public int GetLogLevelAsInt() {
        return (int)Options.LogLevel;
    }

    public void LogLevelNext() {
        Options.SetLogLevel(((int)Options.LogLevel+1)%Enum.GetValues(typeof(LogLevel)).Length);
    }
}