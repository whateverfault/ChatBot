using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.message_filter;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.stream_state_checker.Data;
using TwitchAPI.api.data.requests;

namespace ChatBot.bot.services.moderation;

public class ModerationEvents : ServiceEvents {
    private ModerationService _moderationService = null!;
    private MessageFilterService _messageFilterService = null!;
    private StreamStateCheckerService _streamStateChecker = null!;
    
    public override bool Initialized { get; protected set; }
    

    public override void Init(Service service) {
        _moderationService = (ModerationService)service;
        _streamStateChecker = (StreamStateCheckerService)Services.Get(ServiceId.StreamStateChecker);
        _messageFilterService = (MessageFilterService)Services.Get(ServiceId.MessageFilter);
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        
        _messageFilterService.OnMessageFiltered += _moderationService.HandleMessage;
        _streamStateChecker.OnStreamStateChanged += RemoveWarnsWrapper;
    }

    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        _messageFilterService.OnMessageFiltered -= _moderationService.HandleMessage;
        _streamStateChecker.OnStreamStateChanged -= RemoveWarnsWrapper;
    }
    
    private void RemoveWarnsWrapper(StreamState streamStateNew, StreamState streamStateOld, StreamData? data) {
        if (!streamStateNew.Online) return;

        var modActions = _moderationService.Options.ModerationActions;
        var warnedUsers = _moderationService.Options.WarnedUsers;

        var modActionsToClear = modActions
                               .Where(modAction => modAction.ClearAfterStream)
                               .Select(modAction => modAction.PatternIndex)
                               .ToList();

        var usersToClear = warnedUsers
                          .Where(user => modActionsToClear.Contains(user.ModAction.PatternIndex))
                          .Select(user => user.UserId)
                          .ToList();
        
        _moderationService.Options.RemoveWarnedUsers(usersToClear);
    }
}