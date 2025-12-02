using ChatBot.bot.interfaces;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.logger.data;
using TwitchAPI.client;

namespace ChatBot.bot.services.logger;

public delegate void LogHandler(Log log);

public class LoggerService : Service {
    private readonly object _logLock = new object();
    
    public event LogHandler? OnLog;
    
    public override LoggerOptions Options { get; } = new LoggerOptions();
    
    
    public void Log(LogLevel logLevel, string message) {
        lock (_logLock) {
            if (Options.ServiceState == State.Disabled) return;
            if (logLevel < Options.LogLevel) return;

            var log = new Log(logLevel, DateTime.Now, message);

            Options.AddLog(log);
            OnLog?.Invoke(log);
        }
    }

    public int GetLogLevelAsInt() {
        return (int)Options.LogLevel;
    }

    public void LogLevelNext() {
        Options.SetLogLevel(((int)Options.LogLevel+1)%Enum.GetValues(typeof(LogLevel)).Length);
    }
}