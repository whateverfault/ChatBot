using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.emotes.data.saved;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("use_7tv")]
    public bool Use7Tv { get; set; }

    [JsonProperty("emotes")]
    public IReadOnlyDictionary<EmoteId, Emote> Emotes { get; set; } = null!;
    

    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("use_7tv")] bool use7Tv,
        [JsonProperty("emotes")] IReadOnlyDictionary<EmoteId, Emote> emotes) {
        var dto = new SaveDataDto(
                                  state,
                                  use7Tv,
                                  emotes
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        Use7Tv = dto.Use7Tv.Value;
        Emotes = dto.Emotes.Value;
    }
}