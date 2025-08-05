using ChatBot.bot;
using ChatBot.services.interfaces;
using ChatBot.services.logger;
using ChatBot.services.Static;

namespace ChatBot.services.stream_state_checker;

public class StreamStateCheckerEvents : ServiceEvents {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);

    private StreamStateCheckerService _checkerService = null!;
    private bool _killSignal;
    
    public override bool Initialized { get; protected set; }
    

    public override void Init(Service service) {
        _killSignal = false;
        
        _checkerService = (StreamStateCheckerService)service;
        base.Init(service);
    }

    public override void Kill() {
        _killSignal = true;
        
        base.Kill();
    }
    
    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        
        TwitchChatBot.Instance.OnInitialized += CheckStreamStateRoutine;
    }

    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        TwitchChatBot.Instance.OnInitialized -= CheckStreamStateRoutine;
    }
    
    private async void CheckStreamStateRoutine() {
        try {
            while (true) {
                if (_killSignal) {
                    return;
                }
                
                var cooldown = _checkerService.Options.GetCheckCooldown();
                var lastChecked = _checkerService.Options.GetLastCheckTime();
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                
                if (now-lastChecked < cooldown) continue;

                await _checkerService.CheckState();
                _checkerService.Options.SetLastCheckedTime();
            }
        }
        catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Exception while checking stream state. {e}");
        }
    }
}