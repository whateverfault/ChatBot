using ChatBot.Services.interfaces;
using ChatBot.Services.message_filter;
using ChatBot.Services.Static;
using ChatBot.twitchAPI.interfaces;

namespace ChatBot.Services.message_randomizer;

public class MessageRandomizerEvents : ServiceEvents {
    private MessageRandomizerService _service;


    public override void Init(Service service, Bot bot) {
        _service = (MessageRandomizerService)service;
    }

    public override void Subscribe() {
        var regexService = (MessageFilterService)ServiceManager.GetService(ServiceName.MessageFilter);
        regexService.OnMessageFiltered += _service.HandleMessage;
    }
}