using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;
using ChatBot.Services.message_filter;
using ChatBot.Services.Static;

namespace ChatBot.Services.moderation;

public class ModerationEvents : ServiceEvents {
    private ModerationService _service;
    
    
    public override void Init(Service service, Bot bot) {
        _service = (ModerationService)service;
    }

    public override void Subscribe() {
        var regexService = (MessageFilterService)ServiceManager.GetService(ServiceName.MessageFilter);
        regexService.OnMessageFiltered += _service.HandleMessage;
    }
}