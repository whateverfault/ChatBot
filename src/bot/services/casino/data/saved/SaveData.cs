using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.casino.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }

    [JsonProperty("random_val")]
    public double RandomValue { get; set; }
    
    [JsonProperty("base")]
    public float BaseCoefficient { get; set; }
    
    [JsonProperty("additional")]
    public float AdditionalCoefficient { get; set; }

    [JsonProperty("emotes")]
    public List<GambleEmote> Emotes { get; set; } = null!;
    
    [JsonProperty("emote_slots")]
    public int EmoteSlots { get; set; }
    
    [JsonProperty("duels")]
    public List<Duel> Duels { get; set; } = null!;


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("random_val")] float randomValue,
        [JsonProperty("base")] float baseMultiplier,
        [JsonProperty("additional")] float additional,
        [JsonProperty("emotes")] List<GambleEmote> emotes,
        [JsonProperty("emote_slots")] int emoteSlots,
        [JsonProperty("duels")] List<Duel> duels) {
        var dto = new SaveDataDto(
                                  state,
                                  randomValue,
                                  baseMultiplier,
                                  additional,
                                  emotes,
                                  emoteSlots,
                                  duels
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        RandomValue = dto.RandomValue.Value;
        BaseCoefficient = dto.BaseMultiplier.Value;
        AdditionalCoefficient = dto.AdditionalMultiplier.Value;
        Emotes = dto.Emotes.Value;
        EmoteSlots = dto.EmoteSlots.Value;
        Duels = dto.Duels.Value;
    }
}