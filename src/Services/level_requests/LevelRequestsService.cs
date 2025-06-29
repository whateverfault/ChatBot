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

public enum ReqState {
    Off,
    Points,
    On,
}

public class LevelRequestsService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    public override string Name => ServiceName.LevelRequests;
    public override LevelRequestsOptions Options { get; } = new();


    public async void HandleMessage(ChatMessage message, FilterStatus status, int patternIndex) {
        try {
            if (Options.ServiceState == State.Disabled) return;
            if (Options.PatternIndex != patternIndex) return;
            if (status == FilterStatus.NotMatch) return;
            if (RestrictionHandler.Handle(Options.Restriction, message)) return;
            
            switch (Options.ReqState) {
                case ReqState.On:
                case ReqState.Points when Options.RewardId.Equals(message.CustomRewardId):
                    return;
            }
            
            await Options.ModerationService.WarnUser(message, Options.PatternIndex, $"Реквесты {GetReqStateStr(Options.ReqState)}");
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

    public ReqState GetReqState() {
        return Options.ReqState;
    }

    public int GetReqStateAsInt() {
        return (int)Options.ReqState;
    }

    public void ReqStateNext() {
        Options.SetReqState((ReqState)(((int)Options.ReqState+1)%Enum.GetValues(typeof(ReqState)).Length));
    }

    public string GetRewardId() {
        return Options.RewardId;
    }

    public void SetRewardId(string rewardId) {
        Options.SetRewardId(rewardId);
    }
    
    public string GetReqStateStr(ReqState reqState) {
        return reqState switch { 
                   ReqState.Off    => "отключены.", 
                   ReqState.Points => "за баллы.", 
                   ReqState.On     => "включены.", 
                   _               => throw new ArgumentOutOfRangeException()
               };
    }
}