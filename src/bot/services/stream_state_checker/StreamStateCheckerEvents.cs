using ChatBot.api.twitch.client;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.Static;

namespace ChatBot.bot.services.stream_state_checker;

public class StreamStateCheckerEvents : ServiceEvents {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);

    private StreamStateCheckerService _checkerService = null!;
    private readonly object _killLock = new object();
    private bool _killSignal;
    
    public override bool Initialized { get; protected set; }
    

    public override void Init(Service service) {
        lock (_killLock) {
            _killSignal = false;
        }

        _checkerService = (StreamStateCheckerService)service;
        base.Init(service);
    }

    public override void Kill() {
        base.Kill();
        
        lock (_killLock) {
            _killSignal = true;
        }
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
                lock (_killLock) {
                    if (_killSignal) return;
                }
                
                var cooldown = _checkerService.Options.GetCheckCooldown();
                var lastChecked = _checkerService.Options.GetLastCheckTime();
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (now - lastChecked < cooldown) {
                    var sleep = cooldown - now + lastChecked;
                    Thread.Sleep(TimeSpan.FromSeconds(sleep));
                }

                await _checkerService.CheckState();
                _checkerService.Options.SetLastCheckedTime();
            }
        }
        catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Exception while checking stream state. {e}");
        }
    }
}