using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.message_filter;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;

namespace ChatBot.bot.services.level_requests;

public class LevelRequestsEvents : ServiceEvents {
    private LevelRequestsService _levelRequests = null!;
    private MessageFilterService _messageFilter = null!;
    private StreamStateCheckerService _streamState = null!;
    
    public override bool Initialized { get; protected set; }
    
    
    public override void Init(Service service) {
        _levelRequests = (LevelRequestsService)service;
        _messageFilter = (MessageFilterService)Services.Get(ServiceId.MessageFilter);
        _streamState = (StreamStateCheckerService)Services.Get(ServiceId.StreamStateChecker);
        
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        
        _messageFilter.OnMessageFiltered += _levelRequests.HandleMessage;
        _streamState.OnStreamStateChanged += _levelRequests.OnStreamStarted;
    }
    
    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        _messageFilter.OnMessageFiltered -= _levelRequests.HandleMessage;
        _streamState.OnStreamStateChanged += _levelRequests.OnStreamStarted;
    }
    
    
}