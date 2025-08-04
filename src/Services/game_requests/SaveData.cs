using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.services.game_requests;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    

    [JsonProperty("game_requests_rewards")]
    public List<string> GameRequestsRewards { get; set; }

    [JsonProperty("rawg_api_key")]
    public string RawgApiKey { get; set; } = null!;


    public SaveData() {
        GameRequestsRewards = [];
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("game_requests_rewards")] List<string> gameRequestsRewards,
        [JsonProperty("rawg_api_key")] string rawgApiKey) {
        ServiceState = serviceState;
        GameRequestsRewards = gameRequestsRewards;
        RawgApiKey = rawgApiKey;
    }
}