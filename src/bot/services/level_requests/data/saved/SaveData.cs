using ChatBot.bot.interfaces;
using ChatBot.bot.shared.handlers;
using Newtonsoft.Json;

namespace ChatBot.bot.services.level_requests.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("pattern_index")]
    public int PatternIndex { get; set; }
    
    [JsonProperty("restriction")]
    public Restriction Restriction { get; set; }
    
    [JsonProperty("req_state")]
    public ReqState ReqState { get; set; }
    
    [JsonProperty("enable_on_stream_start")]
    public State EnableOnStreamStart { get; set; }
    
    [JsonProperty("reward_id")]
    public string RewardId { get; set; } = null!;


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("pattern_index")] int patternIndex,
        [JsonProperty("restriction")] Restriction restriction,
        [JsonProperty("req_state")] ReqState reqState,
        [JsonProperty("enable_on_stream_start")] State enableOnStreamStart,
        [JsonProperty("reward_id")] string rewardId) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  patternIndex,
                                  restriction,
                                  reqState,
                                  enableOnStreamStart,
                                  rewardId
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        PatternIndex = dto.PatternIndex.Value;
        Restriction = dto.Restriction.Value;
        ReqState = dto.ReqState.Value;
        EnableOnStreamStart = dto.EnableOnStreamStart.Value;
        RewardId = dto.RewardId.Value;
    }
}