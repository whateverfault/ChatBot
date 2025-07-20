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
        const int checkCooldown = 120;
        long lastChecked = 0;
        var lastNotificationId = _service.Options.LastMessageId;
        var isStreamRunning = _service.Options.WasStreaming;

        while (true) {
            if (_service.GetServiceState() == State.Disabled) {
                continue;
            }

            var curTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (curTime-lastChecked < checkCooldown) continue;

            lastChecked = curTime;

            var bot = Program.GetBot();
            if (bot.Options.Channel == null) {
                continue;
            }

            var streamResponse = await HelixUtils.GetStreams(bot.Options, bot.Options.Channel);

            if (streamResponse == null) {
                if (curTime-_service.Options.LastStreamed < _service.Options.Cooldown) {
                    continue;
                }

                isStreamRunning = false;
                _service.Options.SetWasStreaming(isStreamRunning);
                continue;
            }

            if (!isStreamRunning && curTime-_service.Options.LastStreamed >= _service.Options.Cooldown) {
                if (lastNotificationId.HasValue) {
                    await _service.DeleteNotification(lastNotificationId.Value);
                }

                lastNotificationId = await _service.SendNotification(streamResponse.Data[0]);
                _service.Options.SetLastMessageId(lastNotificationId);
                isStreamRunning = true;
            }

            _service.Options.SetWasStreaming(isStreamRunning);
            _service.Options.SetLastStreamedTime(curTime);
        }
    }
}