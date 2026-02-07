using ChatBot.bot.services.interfaces;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;

namespace ChatBot.bot.services.stream_state_checker;

public class StreamStateCheckerEvents : ServiceEvents {
    private StreamStateCheckerService _checkerService = null!;
    private CancellationTokenSource _cts = null!;
    
    public override bool Initialized { get; protected set; }
    

    public override void Init(Service service) {
        if (Initialized) {
            return;
        }
        base.Init(service);

        _cts = new CancellationTokenSource();
        _checkerService = (StreamStateCheckerService)service;
        
        CheckStreamStateRoutine(_cts.Token);
    }

    public override void Kill() { }
    
    private async void CheckStreamStateRoutine(CancellationToken cancellationToken = default) {
        try {
            while (true) {
                if (cancellationToken.IsCancellationRequested) {
                    return;
                }

                var cooldown = _checkerService.Options.GetCheckCooldown();
                var lastChecked = _checkerService.Options.GetLastCheckTime();
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (now - lastChecked < cooldown) {
                    var sleep = cooldown - now + lastChecked;
                    if (sleep < 0) {
                        sleep = 0;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(Math.Min(30, sleep)), CancellationToken.None);
                    continue;
                }

                await _checkerService.CheckState();
                _checkerService.Options.SetLastCheckedTime();
            }
        }
        catch (TaskCanceledException) { }
        catch (Exception e) {
            ErrorHandler.LogMessage(LogLevel.Error, $"Exception while checking stream state: {e.Message}");
        }
    }
}