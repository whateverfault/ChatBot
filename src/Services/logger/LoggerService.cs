using ChatBot.Services.interfaces;
using ChatBot.Services.Static;
using ChatBot.shared.interfaces;
using TwitchLib.Client.Events;

namespace ChatBot.Services.logger;

public delegate void LogHandler(Log log);

public class LoggerService : Service {
    public event LogHandler? OnLog;
    public event LogHandler? OnTwitchLog;
    public override string Name => ServiceName.Logger;
    public override LoggerOptions Options { get; } = new();
    
    
    public void Log(LogLevel level, string message) {
        if (Options.ServiceState == State.Disabled) return;
        
        var log = new Log(level, DateTime.Now, message);

        Options.AddLog(log);
        OnLog?.Invoke(log);
    }
    
    public void LogTwitchMessage(object? sender, OnLogArgs args) {
        if (Options.ServiceState == State.Disabled) return;
        
        var log = new Log(LogLevel.Info, DateTime.Now, args.Data);

        Options.AddTwitchLog(log);
        OnTwitchLog?.Invoke(log);
    }
}