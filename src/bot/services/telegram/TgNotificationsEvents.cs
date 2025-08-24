using ChatBot.api.twitch.helix.data.requests;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.stream_state_checker.Data;

namespace ChatBot.bot.services.telegram;

public class TgNotificationsEvents : ServiceEvents {
    private readonly object _lock = new object();
    
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
        Task<int?> sendTask;
        lock (_lock) {
            if (now - streamState.LastOnline < _tgNotifications.GetCooldown()) return;
            sendTask = _tgNotifications.SendNotification(data);
        }
        
        var messageId = await sendTask;
        lock (_lock) {
            _tgNotifications.Options.SetLastMessageId(messageId);
        }
    }

    private Task DeleteNotificationWrapper(StreamState streamState, StreamData? data) {
        lock (_lock) {
            if (streamState.WasOnline || streamState.OfflineTime < _tgNotifications.GetCooldown()) return Task.CompletedTask;
        }

        int? lastMessageId;
        lock (_lock) {
            lastMessageId = _tgNotifications.Options.LastMessageId;
        }

        if (!lastMessageId.HasValue) return Task.CompletedTask;

        lock (_lock) {
            _ = _tgNotifications.DeleteNotification(lastMessageId.Value);
            _tgNotifications.Options.SetLastMessageId(null);
        }
        return Task.CompletedTask;
    }
}