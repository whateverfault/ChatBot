using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.message_filter;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.stream_state_checker.Data;
using TwitchAPI.api.data.requests;

namespace ChatBot.bot.services.chat_logs;

public class ChatLogsEvents : ServiceEvents {
    private ChatLogsService _chatLogs = null!;
    private MessageFilterService _messageFilter = null!;
    private StreamStateCheckerService _streamStateChecker = null!;

    public override bool Initialized { get; protected set; }
    
    
    public override void Init(Service service) {
        _chatLogs = (ChatLogsService)service;
        _messageFilter = (MessageFilterService)Services.Get(ServiceId.MessageFilter);
        _streamStateChecker = (StreamStateCheckerService)Services.Get(ServiceId.StreamStateChecker);
        
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        
        _messageFilter.OnMessageFiltered += _chatLogs.HandleMessage;
        _streamStateChecker.OnStreamStateChanged += SaveLogsWrapper;
    }
    
    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        _messageFilter.OnMessageFiltered -= _chatLogs.HandleMessage;
        _streamStateChecker.OnStreamStateChanged -= SaveLogsWrapper;
    }

    private void SaveLogsWrapper(StreamState streamStateNew, StreamState streamStateOld, StreamData? data) {
        if (streamStateNew.Online) {
            return;
        }
        
        _chatLogs.Options.Save();
    }
}