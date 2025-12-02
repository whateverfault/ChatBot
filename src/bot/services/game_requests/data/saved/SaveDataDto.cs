using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.game_requests.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);

    public readonly SafeField<List<string>> GameRequestsRewards = new SafeField<List<string>>([]);


    public SaveDataDto() { }
    
    public SaveDataDto(
        State serviceState,
        List<string> gameRequestsRewards) {
        ServiceState.Value = serviceState;
        GameRequestsRewards.Value = gameRequestsRewards;
    }
}