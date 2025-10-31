using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.stream_state_checker.Data;
using TwitchAPI.helix.data.requests;

namespace ChatBot.bot.services.telegram;

public class TgNotificationsEvents : ServiceEvents {
    private static readonly object _lock = new object();
    
    private TgNotificationsService _tgNotifications = null!;
    private StreamStateCheckerService _streamStateChecker = null!;
    private bool _messageSent;

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

        lock (_lock) {
            if (_messageSent) return;
            _messageSent = true;
        }
        
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Task<int?> sendTask;
        lock (_lock) {
            if (now - streamState.LastOnline < _tgNotifications.GetCooldown()
             || now - (_tgNotifications.GetLastSentTime() ?? 0) < _tgNotifications.GetCooldown()) return;
            
            sendTask = _tgNotifications.SendNotification(data);
            _tgNotifications.Options.SetLastSentTime();
        }
        
        var messageId = await sendTask;
        lock (_lock) {
            _tgNotifications.Options.SetLastMessageId(messageId);
        }
    }

    private Task DeleteNotificationWrapper(StreamState streamState, StreamData? data) {
        lock (_lock) {
            if (streamState.WasOnline
             || streamState.OfflineTime < _tgNotifications.GetCooldown()
             || !_messageSent) return Task.CompletedTask;

            _messageSent = false;
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