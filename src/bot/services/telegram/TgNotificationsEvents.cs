using ChatBot.api.shared.requests.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.stream_state_checker.Data;

namespace ChatBot.bot.services.telegram;

public class TgNotificationsEvents : ServiceEvents {
    private TgNotificationsService _tgNotifications = null!;
    private StreamStateCheckerService _streamStateChecker = null!;

    public override bool Initialized { get; protected set; }
    

    public override void Init(Service service) {
        _tgNotifications = (TgNotificationsService)service;
        _streamStateChecker = (StreamStateCheckerService)ServiceManager.GetService(ServiceName.StreamStateChecker);
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        _streamStateChecker.OnStreamStateChangedAsync += SendNotificationWrapper;
        _streamStateChecker.OnStreamStateUpdateAsync += DeleteNotificationWrapper;
    }

    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        _streamStateChecker.OnStreamStateChangedAsync -= SendNotificationWrapper;
        _streamStateChecker.OnStreamStateUpdateAsync -= DeleteNotificationWrapper;
    }
    
    private async Task SendNotificationWrapper(StreamState streamState, StreamData? data) {
        if (streamState.WasOnline) {
            return;
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (now - streamState.LastOnline < _tgNotifications.GetCooldown()) return;
        
        var messageId = await _tgNotifications.SendNotification(data);
        _tgNotifications.Options.SetLastMessageId(messageId);
    }

    private async Task DeleteNotificationWrapper(StreamState streamState, StreamData? data) {
        if (streamState.WasOnline || streamState.OfflineTime < _tgNotifications.GetCooldown()) return;
        
        var lastMessageId = _tgNotifications.Options.LastMessageId;
        if (lastMessageId.HasValue) {
            await _tgNotifications.DeleteNotification(lastMessageId.Value);
            _tgNotifications.Options.SetLastMessageId(null);
        }
    }
}