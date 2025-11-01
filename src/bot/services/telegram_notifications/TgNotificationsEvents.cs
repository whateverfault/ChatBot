﻿using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.stream_state_checker.Data;
using TwitchAPI.helix.data.requests;

namespace ChatBot.bot.services.telegram_notifications;

#pragma warning disable InconsistentlySynchronizedField
public class TgNotificationsEvents : ServiceEvents {
    private static readonly object _lock = new object();
    
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

        lock (_lock) {
            if (_tgNotifications.GetIsSent()) return;
            _tgNotifications.Options.SetIsSent(true);
        }
        
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (now - streamState.LastOnline < _tgNotifications.GetCooldown()
         || now - (_tgNotifications.GetLastSentTime() ?? 0) < _tgNotifications.GetCooldown()) return;
            
        var messageId = await _tgNotifications.SendNotification(data); 
        
        _tgNotifications.Options.SetLastSentTime();
        _tgNotifications.Options.SetLastMessageId(messageId ?? -1);
    }

    private async Task DeleteNotificationWrapper(StreamState streamState, StreamData? data) {
        lock (_lock) {
            if (streamState.WasOnline
             || streamState.OfflineTime < _tgNotifications.GetCooldown()
             || !_tgNotifications.GetIsSent()) return;
            
            _tgNotifications.Options.SetIsSent(false);
        }

        var lastMessageId = _tgNotifications.Options.LastMessageId;
        if (lastMessageId < 0) return;
        
        var result = await _tgNotifications.DeleteNotification(lastMessageId);
        if (result) _tgNotifications.Options.SetLastMessageId(-1);
    }
#pragma warning restore InconsistentlySynchronizedField
}