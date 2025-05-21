using ChatBot.Services.interfaces;
using ChatBot.twitchAPI.interfaces;

namespace ChatBot.Services.message_randomizer;

public class MessageRandomizerEvents : ServiceEvents {
    private MessageRandomizerService _service;
    private Bot _bot;
    

    public override void Init(Service service, Bot bot) {
        _service = (MessageRandomizerService)service;
        _bot = bot;
    }

    public override void Subscribe() {
        _bot.OnMessageReceived += _service.HandleMessage;
    }
}