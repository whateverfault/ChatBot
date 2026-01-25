using ChatBot.bot.chat_bot;
using ChatBot.bot.services.interfaces;

namespace ChatBot.bot.services.message_filter;

public class MessageFilterEvents : ServiceEvents {
    private MessageFilterService _messageFilter = null!;
    
    public override bool Initialized { get; protected set; }
    
    
    public override void Init(Service service) {
        _messageFilter = (MessageFilterService)service;
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        
        TwitchChatBot.Instance.OnMessageReceived += _messageFilter.HandleMessage;
        TwitchChatBot.Instance.OnCommandReceived += _messageFilter.HandleCommand;
    }

    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        TwitchChatBot.Instance.OnMessageReceived -= _messageFilter.HandleMessage;
        TwitchChatBot.Instance.OnCommandReceived -= _messageFilter.HandleCommand;
    }
}