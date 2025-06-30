using ChatBot.Services.game_requests.Data;
using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.game_requests;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }

    [JsonProperty("game_requests")]
    public List<GameRequest> GameRequests { get; set; }
    
    [JsonProperty("game_requests_filter")]
    public GameRequestsFilter GameRequestsFilter { get; set; }

    [JsonProperty("game_requests_rewards")]
    public List<string> GameRequestsRewards { get; set; }


    public SaveData() {
        GameRequestsFilter = new GameRequestsFilter();
        GameRequests = [];
        GameRequestsRewards = [];
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("game_requests")] List<GameRequest> gameRequests,
        [JsonProperty("game_requests_filter")] GameRequestsFilter gameRequestsFilter,
        [JsonProperty("game_requests_rewards")] List<string> gameRequestsRewards) {
        ServiceState = serviceState;
        GameRequests = gameRequests;
        GameRequestsFilter = gameRequestsFilter;
        GameRequestsRewards = gameRequestsRewards;
    }
}