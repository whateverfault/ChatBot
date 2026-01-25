using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;

namespace ChatBot.bot.services.bot_lifecycle;

public class BotLifecycleEvents : ServiceEvents {
    private BotLifecycleService? _lifecycle;
    private StreamStateCheckerService? _streamState;
    
    public override bool Initialized { get; protected set; }
    
    
    public override void Init(Service service) {
        if (Initialized)
            return;
        
        _lifecycle = (BotLifecycleService)service;
        _streamState = (StreamStateCheckerService)Services.Get(ServiceId.StreamStateChecker);
        
        base.Init(service);
    }
    
    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        
        if (_streamState == null || _lifecycle == null) {
            return;
        }
        
        _streamState.OnStreamStateChangedAsync += _lifecycle.BotStart;
        _streamState.OnStreamStateUpdateAsync += _lifecycle.BotStop;
    }

    public override void Kill() { }
}