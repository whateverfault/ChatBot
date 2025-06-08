using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;
using ChatBot.Services.message_filter;
using ChatBot.Services.moderation;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.Services.level_requests;

public class LevelRequestsService : Service {
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
            throw new Exception(e.ToString());
        }
    }

    public override void Init(Bot bot) {
        Options.ModerationService = (ModerationService)ServiceManager.GetService(ServiceName.Moderation);
        base.Init(bot);
    }

    public int GetPatternIndex() {
        return Options.PatternIndex;
    }

    public void SetPatternIndex(int index) {
        Options.SetPatternIndex(index);
    }

    public int GetRestrictionAsInt() {
        return (int)Options.Restriction;
    }

    public void RestrictionNext() {
        Options.SetRestriction((Restriction)(((int)Options.Restriction+1)%Enum.GetValues(typeof(Restriction)).Length));
    }
}