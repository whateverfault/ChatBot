using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.message_filter;
using ChatBot.bot.services.Static;

namespace ChatBot.bot.services.chat_logs;

public class ChatLogsEvents : ServiceEvents {
    private ChatLogsService _chatLogs = null!;
    private MessageFilterService _messageFilter = null!;
    
    public override bool Initialized { get; protected set; }
    
    
    public override void Init(Service service) {
        _chatLogs = (ChatLogsService)service;
        _messageFilter = (MessageFilterService)ServiceManager.GetService(ServiceName.MessageFilter);
        
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        
        _messageFilter.OnMessageFiltered += _chatLogs.HandleMessage;
    }
    
    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        _messageFilter.OnMessageFiltered -= _chatLogs.HandleMessage;
    }
}