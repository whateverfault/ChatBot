using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.casino.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);

    public readonly SafeField<float> RandomValue = new SafeField<float>(0f);

    public readonly SafeField<float> BaseMultiplier = new SafeField<float>(1.51f);

    public readonly SafeField<float> AdditionalMultiplier = new SafeField<float>(1.3f);

    public readonly SafeField<List<Duel>> Duels = new SafeField<List<Duel>>([]);


    public SaveDataDto() { }
    
    public SaveDataDto(
        State state,
        float randomValue,
        float baseMultiplier,
        float additional,
        List<Duel> duels) {
        ServiceState.Value = state;
        RandomValue.Value = randomValue;
        BaseMultiplier.Value = baseMultiplier;
        AdditionalMultiplier.Value = additional;
        Duels.Value = duels;
    }
}