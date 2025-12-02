using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.text_generator.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);

    public readonly SafeField<int> ContextSize = new SafeField<int>(1);

    public readonly SafeField<int> MaxLength = new SafeField<int>(10);

    public readonly SafeField<Dictionary<string, Dictionary<string, int>>> Model =
        new SafeField<Dictionary<string, Dictionary<string, int>>>([]);


    public SaveDataDto() { }
    
    public SaveDataDto(
        State serviceState,
        int contextSize,
        int maxLength,
        Dictionary<string, Dictionary<string, int>> model) {
        ServiceState.Value = serviceState;
        ContextSize.Value = contextSize;
        MaxLength.Value = maxLength;
        Model.Value = model;
    }
}