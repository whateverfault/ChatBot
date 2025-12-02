using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.chat_ads.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }

    [JsonProperty("chat_adds")]
    public List<ChatAd> ChatAds { get; set; } = null!;


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("chat_adds")] List<ChatAd> chatAds) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  chatAds
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        ChatAds = dto.ChatAds.Value;
    }
}