using ChatBot.services.interfaces;
using ChatBot.services.message_filter;
using ChatBot.services.Static;

namespace ChatBot.services.moderation;

public class ModerationEvents : ServiceEvents {
    private ModerationService _moderationService = null!;
    private MessageFilterService _messageFilterService = null!;
    
    public override bool Initialized { get; protected set; }
    
    
    public override void Init(Service service) {
        _moderationService = (ModerationService)service;
        _messageFilterService = (MessageFilterService)ServiceManager.GetService(ServiceName.MessageFilter);
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        
        _messageFilterService.OnMessageFiltered += _moderationService.HandleMessage;
    }

    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        _messageFilterService.OnMessageFiltered -= _moderationService.HandleMessage;
    }
}