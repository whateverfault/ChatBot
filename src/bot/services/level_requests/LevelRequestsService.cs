using ChatBot.bot.interfaces;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.message_filter;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;
using TwitchAPI.client.data;

namespace ChatBot.bot.services.level_requests;

public enum ReqState {
    Off,
    Points,
    On,
}

public class LevelRequestsService : Service {
    public override LevelRequestsOptions Options { get; } = new LevelRequestsOptions();


    public async void HandleMessage(ChatMessage chatMessage, FilterStatus status, int patternIndex) {
        try {
            if (Options.ServiceState == State.Disabled) return;
            if (Options.PatternIndex != patternIndex) return;
            if (status == FilterStatus.Ok) return;
            if (chatMessage.Fits(Options.Restriction)) return;
            
            switch (Options.ReqState) {
                case ReqState.On:
                case ReqState.Points when Options.RewardId.Equals(chatMessage.RewardId):
                    return;
            }
            
            await LevelRequestsOptions.ModerationService.WarnUser(chatMessage, Options.PatternIndex, $"Req {GetReqStateStr(Options.ReqState, eng: true)}");
        } catch (Exception e) {
            ErrorHandler.LogMessage(LogLevel.Error, $"Error while handling a potential level request: {e.Data}");
        }
    }

    public int GetPatternIndex() {
        return Options.PatternIndex;
    }

    public bool SetPatternIndex(int index) {
        if (index < 0 || index >= LevelRequestsOptions.ModerationService.GetModActions().Count) return false;
        
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

    public string GetReqStateStr(ReqState reqState, bool eng = false) {
        if (eng) {
            return reqState switch { 
                       ReqState.Off    => "off.", 
                       ReqState.Points => "for channel points.", 
                       ReqState.On     => "on.", 
                       _               => throw new ArgumentOutOfRangeException(),
                   };
        }
        
        return reqState switch { 
                   ReqState.Off    => "отключены.", 
                   ReqState.Points => "за баллы.", 
                   ReqState.On     => "включены.", 
                   _               => throw new ArgumentOutOfRangeException(),
               };
    }
}