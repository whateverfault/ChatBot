using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;
using ChatBot.Services.message_filter;
using ChatBot.Services.Static;

namespace ChatBot.Services.level_requests;

public class LevelRequestsEvents : ServiceEvents {
    private LevelRequestsService _service = null!;
    
    
    public override void Init(Service service, Bot bot) {
        _service = (LevelRequestsService)service;
    }

    public override void Subscribe() {
        var messageFilter = (MessageFilterService)ServiceManager.GetService(ServiceName.MessageFilter);
        messageFilter.OnMessageFiltered += _service.HandleMessage;
    }
}