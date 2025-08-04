using ChatBot.bot;
using ChatBot.services.interfaces;

namespace ChatBot.services.logger;

public class LoggerEvents : ServiceEvents {
    private LoggerService _logger = null!;
    
    public override bool Initialized { get; protected set; }
    
    
    public override void Init(Service service) {
        _logger = (LoggerService)service;
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        
        TwitchChatBot.Instance.OnLog += _logger.LogTwitchMessage;
    }
    
    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        TwitchChatBot.Instance.OnLog -= _logger.LogTwitchMessage;
    }
}