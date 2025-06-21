using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;
using ChatBot.Services.logger;
using ChatBot.Services.message_filter;
using ChatBot.Services.moderation;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.Services.level_requests;

public class LevelRequestsService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    public override string Name => ServiceName.LevelRequests;
    public override LevelRequestsOptions Options { get; } = new();


    public async void HandleMessage(ChatMessage message, FilterStatus status, int patternIndex) {
        try {
            if (Options.ServiceState == State.Disabled) return;
            if (Options.PatternIndex != patternIndex) return;
            if (status == FilterStatus.NotMatch) return;
            
            if (RestrictionHandler.Handle(Options.Restriction, message)) {
                return;
            }
        
            await Options.ModerationService.WarnUser(message, Options.PatternIndex);
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"[LevelRequests] Error while handling a message. {e.Message}");
        }
    }

    public override void Init(Bot bot) {
        Options.ModerationService = (ModerationService)ServiceManager.GetService(ServiceName.Moderation);
        base.Init(bot);
    }

    public int GetPatternIndex() {
        return Options.PatternIndex;
    }

    public bool SetPatternIndex(int index) {
        if (index < 0 || index >= Options.ModerationService.GetModActions().Count) return false;
        
        Options.SetPatternIndex(index);
        return true;
    }

    public int GetRestrictionAsInt() {
        return (int)Options.Restriction;
    }

    public void RestrictionNext() {
        Options.SetRestriction((Restriction)(((int)Options.Restriction+1)%Enum.GetValues(typeof(Restriction)).Length));
    }
}