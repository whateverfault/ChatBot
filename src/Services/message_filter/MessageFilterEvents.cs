using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;

namespace ChatBot.Services.message_filter;

public class MessageFilterEvents : ServiceEvents {
    private MessageFilterService _service = null!;
    private Bot _bot = null!;
    
    
    public override void Init(Service service, Bot bot) {
        _service = (MessageFilterService)service;
        _bot = bot;
    }

    public override void Subscribe() {
        if (subscribed) return;
        base.Subscribe();
        _bot.OnMessageReceived += _service.HandleMessage;
    }
}