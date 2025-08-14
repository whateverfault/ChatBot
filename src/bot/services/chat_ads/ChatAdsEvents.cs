using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;

namespace ChatBot.bot.services.chat_ads;

public class ChatAdsEvents : ServiceEvents {
    private ChatAdsService _chatAds = null!;
    private StreamStateCheckerService _streamStateChecker = null!;
    
    public override bool Initialized { get; protected set; }
    
    
    public override void Init(Service service) {
        _chatAds = (ChatAdsService)service;
        _streamStateChecker = (StreamStateCheckerService)ServiceManager.GetService(ServiceName.StreamStateChecker);
        
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        
        _streamStateChecker.OnStreamStateUpdate += _chatAds.HandleChatAdsSending;
    }
    
    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        _streamStateChecker.OnStreamStateUpdate -= _chatAds.HandleChatAdsSending;
    }
}