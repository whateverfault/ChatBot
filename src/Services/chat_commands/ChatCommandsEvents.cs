using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;

namespace ChatBot.Services.chat_commands;

public class ChatCommandsEvents : ServiceEvents {
    private Bot _bot = null!;
    private ChatCommandsService _serviceOld = null!;


    public override void Init(Service service, Bot bot) {
        _serviceOld = (ChatCommandsService)service;
        _bot = bot;
    }

    public override void Subscribe() {
        if (subscribed) return;
        base.Subscribe();
        _bot.OnChatCommandReceived += _serviceOld.HandleCommand;
        _serviceOld.Options.OnCommandIdentifierChanged += _serviceOld.ChangeCommandIdentifier;
    }
}