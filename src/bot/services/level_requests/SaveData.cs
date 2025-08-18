using ChatBot.bot.shared.handlers;
using ChatBot.bot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.level_requests;

public class SaveData {
    [JsonProperty(PropertyName = "service_state")]
    public State ServiceState { get; set; }
    [JsonProperty(PropertyName = "pattern_index")]
    public int PatternIndex { get; set; }
    [JsonProperty(PropertyName = "restriction")]
    public Restriction Restriction { get; set; }
    [JsonProperty(PropertyName = "req_state")]
    public ReqState ReqState { get; set; }
    [JsonProperty(PropertyName = "reward_id")]
    public string RewardId { get; set; } = null!;


    public SaveData() {}
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName = "pattern_index")] int patternIndex,
        [JsonProperty(PropertyName = "restriction")] Restriction restriction,
        [JsonProperty(PropertyName = "req_state")] ReqState reqState,
        [JsonProperty(PropertyName = "reward_id")] string rewardId) {
        ServiceState = serviceState;
        PatternIndex = patternIndex;
        Restriction = restriction;
        ReqState = reqState;
        RewardId = rewardId;
    }
}