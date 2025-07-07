using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;
using ChatBot.shared.interfaces;
using ChatBot.utils.Twitch.Helix;

namespace ChatBot.Services.telegram;

public class TgNotificationsEvents : ServiceEvents {
    private TgNotificationsService _service = null!;
    private bot.ChatBot _bot = null!;
    
    
    public override void Init(Service service, Bot bot) {
        _service = (TgNotificationsService)service;
        _bot = (bot.ChatBot)bot;
    }

    public override void Subscribe() {
        if (subscribed) {
            return;
        }
        base.Subscribe();
        _bot.OnInitialized += () => {
                                  Task.Run(MonitorChannel);
                              };
    }

    private async Task MonitorChannel() {
        const int minute = 60;
        var lastNotificationId = _service.Options.LastMessageId;
        long lastChecked = 0;
        var isStreamRunning = _service.Options.WasStreaming;
        
        while (true) {
            if (_service.GetServiceState() == State.Disabled) {
                continue;
            }
            
            var curTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (curTime-lastChecked < minute) continue;

            lastChecked = curTime;

            var bot = Program.GetBot();
            if (bot.Options.Channel == null) {
                continue;
            }
            
            var streamResponse = await HelixUtils.GetStreams(bot.Options, bot.Options.Channel);

            if (streamResponse == null) {
                isStreamRunning = false;
                _service.Options.SetWasStreaming(isStreamRunning);
                continue;
            }
            
            if (isStreamRunning || curTime-_service.Options.LastStreamed < _service.Options.Cooldown) {
                continue;
            }
            
            if (lastNotificationId.HasValue) {
                await _service.DeleteNotification(lastNotificationId.Value);
            }
            
            lastNotificationId = await _service.SendNotification(streamResponse.Data[0]);

            if (lastNotificationId.HasValue) { 
                _service.Options.SetLastMessageId(lastNotificationId.Value);
            }
            
            isStreamRunning = true;
            _service.Options.SetWasStreaming(isStreamRunning);
            _service.Options.SetLastStreamedTime(curTime);
        }
    }
}