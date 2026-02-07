using ChatBot.bot.interfaces;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.stream_state_checker.Data;
using TwitchAPI.api.data.requests;

namespace ChatBot.bot.services.telegram_notifications;

#pragma warning disable InconsistentlySynchronizedField
public class TgNotificationsEvents : ServiceEvents {
    private static readonly object _sync = new object();
    
    private TgNotificationsService _tgNotifications = null!;
    private StreamStateCheckerService _streamStateChecker = null!;

    public override bool Initialized { get; protected set; }
    

    public override void Init(Service service) {
        if (Initialized)
            return;
        base.Init(service);
        
        _tgNotifications = (TgNotificationsService)service;
        _streamStateChecker = (StreamStateCheckerService)Services.Get(ServiceId.StreamStateChecker);
        
        _streamStateChecker.OnStreamStateChangedAsync += OnStreamStarted;
        _streamStateChecker.OnStreamStateUpdateAsync += SendNotificationWrapper;
        _streamStateChecker.OnStreamStateUpdateAsync += DeleteNotificationWrapper;
    }
    
    private Task OnStreamStarted(StreamState streamStateNew, StreamState streamStateOld, StreamData? data) {
        lock (_sync) {
            if (!streamStateNew.Online
             || _tgNotifications.Options.ServiceState == State.Disabled) 
                return Task.CompletedTask;
            
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (now - streamStateOld.LastOnline < _tgNotifications.GetCooldown()
             || now - (_tgNotifications.GetLastSentTime() ?? 0) < _tgNotifications.GetCooldown()) 
                return Task.CompletedTask;
            
            _tgNotifications.Options.SetIsSent(false);
        }
        
        return Task.CompletedTask;
    }
    
    private async Task SendNotificationWrapper(StreamState streamState, StreamData? data) {
        lock (_sync) {
            if (!streamState.Online 
             || _tgNotifications.GetIsSent()
             || _tgNotifications.Options.ServiceState == State.Disabled) 
                return;
            
            _tgNotifications.Options.SetIsSent(true);
        }
        
        var messageId = await _tgNotifications.SendNotification(data);

        lock (_sync) {
            _tgNotifications.Options.SetLastSentTime();
            if (messageId != null)
                _tgNotifications.Options.SetLastMessageId(messageId.Value);
        }
    }

    private async Task DeleteNotificationWrapper(StreamState streamState, StreamData? data) {
        if (streamState.Online
         || streamState.OfflineTime < _tgNotifications.GetCooldown())
            return;
        
        lock (_sync) {
            if (!_tgNotifications.GetIsSent()) 
                return;
            
            _tgNotifications.Options.SetIsSent(false);
        }
        
        var lastMessageId = _tgNotifications.Options.LastMessageId;
        if (lastMessageId < 0) 
            return;
        
        var result = await _tgNotifications.DeleteNotification(lastMessageId);

        lock (_sync) {
            if (result) 
                _tgNotifications.Options.SetLastMessageId(-1);
        }
    }
#pragma warning restore InconsistentlySynchronizedField
}