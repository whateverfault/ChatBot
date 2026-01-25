using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.emotes.data.saved;

public class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Enabled);
    
    public readonly SafeField<bool> Use7Tv = new SafeField<bool>(false);
    
    public readonly SafeField<IReadOnlyDictionary<EmoteId, Emote>> Emotes = new SafeField<IReadOnlyDictionary<EmoteId, Emote>>(EmotesList.Emotes);
    
    
    public SaveDataDto() { }
    
    public SaveDataDto(
        State serviceState,
        bool use7Tv,
        IReadOnlyDictionary<EmoteId, Emote> emotes) {
        ServiceState.Value = serviceState;
        Use7Tv.Value = use7Tv;
        Emotes.Value = emotes;
    }
}