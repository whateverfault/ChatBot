using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;

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
    }
}