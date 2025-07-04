using ChatBot.Services.game_requests.Data;
using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.game_requests;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }

    [JsonProperty("game_requests")]
    public List<GameRequest> GameRequests { get; set; }

    [JsonProperty("game_requests_rewards")]
    public List<string> GameRequestsRewards { get; set; }

    [JsonProperty("rawg_api_key")]
    public string RawgApiKey { get; set; } = null!;


    public SaveData() {
        GameRequests = [];
        GameRequestsRewards = [];
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("game_requests")] List<GameRequest> gameRequests,
        [JsonProperty("game_requests_rewards")] List<string> gameRequestsRewards,
        [JsonProperty("rawg_api_key")] string rawgApiKey) {
        ServiceState = serviceState;
        GameRequests = gameRequests;
        GameRequestsRewards = gameRequestsRewards;
        RawgApiKey = rawgApiKey;
    }
}