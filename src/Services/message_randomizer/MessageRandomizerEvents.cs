using ChatBot.Services.interfaces;
using ChatBot.twitchAPI.interfaces;

namespace ChatBot.Services.message_randomizer;

public class MessageRandomizerEvents : ServiceEvents {
    private Bot _bot;
    private MessageRandomizerService _service;


    public override void Init(Service service, Bot bot) {
        _service = (MessageRandomizerService)service;
        _bot = bot;
    }

    public override void Subscribe() {
        _bot.OnMessageReceived += _service.HandleMessage;
    }
}