using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.message_filter;
using ChatBot.bot.services.Static;

namespace ChatBot.bot.services.level_requests;

public class LevelRequestsEvents : ServiceEvents {
    private LevelRequestsService _levelRequests = null!;
    private MessageFilterService _messageFilter = null!;
    
    public override bool Initialized { get; protected set; }
    
    
    public override void Init(Service service) {
        _levelRequests = (LevelRequestsService)service;
        _messageFilter = (MessageFilterService)Services.Get(ServiceId.MessageFilter);
        
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        
        _messageFilter.OnMessageFiltered += _levelRequests.HandleMessage;
    }
    
    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        _messageFilter.OnMessageFiltered -= _levelRequests.HandleMessage;
    }
}