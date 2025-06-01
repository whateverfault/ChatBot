using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;

namespace ChatBot.Services.chat_commands;

public class ChatCommandsEvents : ServiceEvents {
    private Bot _bot = null!;
    private ChatCommandsService _service = null!;


    public override void Init(Service service, Bot bot) {
        _service = (ChatCommandsService)service;
        _bot = bot;
    }

    public override void Subscribe() {
        _bot.OnChatCommandReceived += _service.HandleMessage;
        _service.Options.OnCommandIdentifierChanged += _service.ChangeCommandIdentifier;
    }
}