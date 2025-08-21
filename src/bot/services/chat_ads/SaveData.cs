using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_ads.Data;
using Newtonsoft.Json;

namespace ChatBot.bot.services.chat_ads;

public class SaveData {
    [JsonProperty(PropertyName = "service_state")]
    public State ServiceState { get; set; }

    [JsonProperty("chat_adds")]
    public List<ChatAd> ChatAds { get; set; }


    public SaveData() {
        ChatAds = [];
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty("chat_adds")] List<ChatAd> chatAds) {
        ServiceState = serviceState;
        ChatAds = chatAds;
    }
}