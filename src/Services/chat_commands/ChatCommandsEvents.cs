using ChatBot.Services.interfaces;
using ChatBot.twitchAPI.interfaces;

namespace ChatBot.Services.chat_commands;

public class ChatCommandsEvents : ServiceEvents {
    private ChatCommandsService _service;
    private Bot _bot;
    
    
    public override void Init(Service service, Bot bot) {
        _service = (ChatCommandsService)service;
        _bot = bot;
    }

    public override void Subscribe() {
        _bot.OnChatCommandReceived += _service.HandleMessage;
        _service.Options.OnCommandIdentifierChanged += _service.ChangeCommandIdentifier;
    }
}