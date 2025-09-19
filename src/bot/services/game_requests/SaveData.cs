using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.game_requests;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("game_requests_rewards")]
    public List<string> GameRequestsRewards { get; set; }


    public SaveData() {
        GameRequestsRewards = [];
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("game_requests_rewards")] List<string> gameRequestsRewards) {
        ServiceState = serviceState;
        GameRequestsRewards = gameRequestsRewards;
    }
}