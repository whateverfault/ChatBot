using ChatBot.api.basic;
using ChatBot.bot.interfaces;
using ChatBot.bot.shared.handlers;

namespace ChatBot.bot.services.level_requests.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);

    public readonly SafeField<int> PatternIndex = new SafeField<int>(0);

    public readonly SafeField<Restriction> Restriction = new SafeField<Restriction>(shared.handlers.Restriction.Vip);

    public readonly SafeField<ReqState> ReqState = new SafeField<ReqState>(level_requests.ReqState.Off);

    public readonly SafeField<string> RewardId = new SafeField<string>(string.Empty);


    public SaveDataDto() {}
    
    public SaveDataDto(
        State serviceState,
        int patternIndex,
        Restriction restriction,
        ReqState reqState,
        string rewardId) {
        ServiceState.Value = serviceState;
        PatternIndex.Value = patternIndex;
        Restriction.Value = restriction;
        ReqState.Value = reqState;
        RewardId.Value = rewardId;
    }
}