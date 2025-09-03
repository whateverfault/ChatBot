using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.message_filter;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.stream_state_checker.Data;
using TwitchAPI.helix.data.requests;

namespace ChatBot.bot.services.chat_logs;

public class ChatLogsEvents : ServiceEvents {
    private ChatLogsService _chatLogs = null!;
    private MessageFilterService _messageFilter = null!;
    private StreamStateCheckerService _streamStateChecker = null!;

    public override bool Initialized { get; protected set; }
    
    
    public override void Init(Service service) {
        _chatLogs = (ChatLogsService)service;
        _messageFilter = (MessageFilterService)ServiceManager.GetService(ServiceName.MessageFilter);
        _streamStateChecker = (StreamStateCheckerService)ServiceManager.GetService(ServiceName.StreamStateChecker);
        
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

    private void SaveLogsWrapper(StreamState streamState, StreamData? data) {
        if (streamState.WasOnline) {
            return;
        }
        
        _chatLogs.Options.Save();
    }
}