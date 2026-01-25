using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_logs;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.stream_state_checker.Data;
using TwitchAPI.client.data;
using TwitchAPI.api.data.requests;

namespace ChatBot.bot.services.chat_ads;

public class ChatAdsEvents : ServiceEvents {
    private ChatAdsService _chatAds = null!;
    private StreamStateCheckerService _streamStateChecker = null!;
    private ChatLogsService _chatLogs = null!;
    
    public override bool Initialized { get; protected set; }
    
    
    public override void Init(Service service) {
        _chatAds = (ChatAdsService)service;
        _streamStateChecker = (StreamStateCheckerService)Services.Get(ServiceId.StreamStateChecker);
        _chatLogs = (ChatLogsService)Services.Get(ServiceId.ChatLogs);
        
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        
        _streamStateChecker.OnStreamStateUpdate += _chatAds.SendChatAds;
        _streamStateChecker.OnStreamStateChanged += ZeroChatAdsCounter;
        _chatLogs.OnLogsAppended += IncrementChatAdsCounterWrapper;
    }
    
    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        _streamStateChecker.OnStreamStateUpdate -= _chatAds.SendChatAds;
        _streamStateChecker.OnStreamStateChanged -= ZeroChatAdsCounter;
        _chatLogs.OnLogsAppended -= IncrementChatAdsCounterWrapper;
    }

    private void IncrementChatAdsCounterWrapper(ChatMessage chatMessage) {
        var ads = _chatAds.GetChatAds();

        foreach (var ad in ads) {
            if (ad.GetState() == State.Disabled) continue;
            ad.IncrementCounter();
        }
    }
    
    private void ZeroChatAdsCounter(StreamState state, StreamData? data) {
        if (state.Online) return;
        
        var ads = _chatAds.GetChatAds();
        foreach (var ad in ads) {
            ad.ZeroCounter();
        }
    }
}