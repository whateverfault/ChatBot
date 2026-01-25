using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.message_filter;
using ChatBot.bot.services.Static;
using TwitchAPI.client.commands.data;

namespace ChatBot.bot.services.chat_commands;

public class ChatCommandsEvents : ServiceEvents {
    private ChatCommandsService _chatCommands = null!;
    private MessageFilterService _messageFilter = null!;

    public override bool Initialized { get; protected set; }
    

    public override void Init(Service service) {
        _chatCommands = (ChatCommandsService)service;
        _messageFilter = (MessageFilterService)Services.Get(ServiceId.MessageFilter);
        
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();

        _messageFilter.OnCommandFiltered += HandleCommandWrapper;
    }
    
    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        _messageFilter.OnCommandFiltered -= HandleCommandWrapper;
    }

    private void HandleCommandWrapper(Command cmd, FilterStatus status, int filterIndex) {
        if (status != FilterStatus.Ok)
            return;

        _chatCommands.HandleCommand(cmd);
    }
}