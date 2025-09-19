using ChatBot.bot.chat_bot;
using ChatBot.bot.services.interfaces;

namespace ChatBot.bot.services.chat_commands;

public class ChatCommandsEvents : ServiceEvents {
    private ChatCommandsService _chatCommands = null!;

    public override bool Initialized { get; protected set; }
    

    public override void Init(Service service) {
        _chatCommands = (ChatCommandsService)service;
        
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();

        var client = TwitchChatBot.Instance.GetClient();
        if (client == null) return;

        client.OnCommandReceived += _chatCommands.HandleCommand;
    }
    
    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        var client = TwitchChatBot.Instance.GetClient();
        if (client == null) return;
        
        client.OnCommandReceived -= _chatCommands.HandleCommand;
    }
}