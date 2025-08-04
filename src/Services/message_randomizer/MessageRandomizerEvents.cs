using ChatBot.services.chat_logs;
using ChatBot.services.interfaces;
using ChatBot.services.Static;

namespace ChatBot.services.message_randomizer;

public class MessageRandomizerEvents : ServiceEvents {
    private MessageRandomizerService _messageRandomizer = null!;
    private ChatLogsService _chatLogs = null!;

    public override bool Initialized { get; protected set; }
    
    
    public override void Init(Service service) {
        _messageRandomizer = (MessageRandomizerService)service;
        _chatLogs = (ChatLogsService)ServiceManager.GetService(ServiceName.ChatLogs);
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        
        _chatLogs.OnLogsAppended += _messageRandomizer.HandleChatLog;
    }

    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        _chatLogs.OnLogsAppended -= _messageRandomizer.HandleChatLog;
    }
}