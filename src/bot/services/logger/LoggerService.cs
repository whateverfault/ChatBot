using ChatBot.api.client;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.shared.interfaces;

namespace ChatBot.bot.services.logger;

public delegate void LogHandler(Log log);

public class LoggerService : Service {
    public event LogHandler? OnLog;
    
    public override string Name => ServiceName.Logger;
    public override LoggerOptions Options { get; } = new LoggerOptions();
    
    
    public void Log(LogLevel level, string message) {
        if (Options.ServiceState == State.Disabled) return;
        
        var log = new Log(level, DateTime.Now, message);
        
        Options.AddLog(log);
        OnLog?.Invoke(log);
    }
}