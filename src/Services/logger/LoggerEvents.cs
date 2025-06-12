using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;
using TwitchLib.Client.Events;

namespace ChatBot.Services.logger;

public class LoggerEvents : ServiceEvents {
    private LoggerService _service = null!;
    private Bot _bot = null!;
    
    
    public override void Init(Service service, Bot bot) {
        _service = (LoggerService)service;
        _bot = bot;
    }

    public override void Subscribe() {
        if (subscribed) return;
        base.Subscribe();
        _bot.OnLog += _service.LogTwitchMessage;
        _bot.OnMessageReceived += LogReceivedMessage;
    }

    private void LogReceivedMessage(object? sender, OnMessageReceivedArgs args) {
        _service.Log(LogLevel.Info, $"{args.ChatMessage.Channel}\\{args.ChatMessage.Username}: {args.ChatMessage.Message}");
    }
}