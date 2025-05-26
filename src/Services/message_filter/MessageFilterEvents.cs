using ChatBot.Services.interfaces;
using ChatBot.twitchAPI.interfaces;

namespace ChatBot.Services.message_filter;

public class MessageFilterEvents : ServiceEvents {
    private MessageFilterService _service;
    private Bot _bot;
    
    
    public override void Init(Service service, Bot bot) {
        _service = (MessageFilterService)service;
        _bot = bot;
    }

    public override void Subscribe() {
        _bot.OnMessageReceived += _service.HandleMessage;
    }
}