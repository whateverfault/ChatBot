using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.bank_account_filtering.data.saved;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("last_online")]
    public long LastOnline { get; set; }
    
    [JsonProperty("streams_passed")]
    public long StreamsPassed { get; set; }
    
    [JsonProperty("min_stream_len")]
    public long MinStreamLength { get; set; }
    
    [JsonProperty("last_active_threshold")]
    public long LastActiveThreshold { get; set; }
    
    [JsonProperty("streams_time_gap_threshold")]
    public long StreamsTimeGapThreshold { get; set; }
    
    [JsonProperty("passed_streams_threshold")]
    public long PassedStreamsThreshold { get; set; }
    
    [JsonProperty("stream_filtered")]
    public bool StreamFiltered { get; set; }
    
    
    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("min_stream_len")] long minStreamLen,
        [JsonProperty("last_active_threshold")] long lastActiveThreshold,
        [JsonProperty("streams_time_gap_threshold")] long streamsTimeGapThreshold,
        [JsonProperty("last_online")] long lastOnline,
        [JsonProperty("streams_passed")] long streamsPassed,
        [JsonProperty("passed_streams_threshold")] long passedStreamsThreshold,
        [JsonProperty("stream_filtered")] bool streamFiltered) {
        var dto = new SaveDataDto(
                                  state,
                                  minStreamLen,
                                  lastActiveThreshold,
                                  streamsTimeGapThreshold,
                                  lastOnline,
                                  streamsPassed,
                                  passedStreamsThreshold,
                                  streamFiltered
                                 );
        FromDto(dto);
    }
    
    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        MinStreamLength = dto.MinStreamLength.Value;
        LastActiveThreshold = dto.LastActiveThreshold.Value;
        LastOnline = dto.LastOnline.Value;
        StreamsTimeGapThreshold = dto.StreamsTimeGapThreshold.Value;
        StreamsPassed = dto.StreamsPassed.Value;
        PassedStreamsThreshold = dto.PassedStreamsThreshold.Value;
        StreamFiltered = dto.StreamFiltered.Value;
    }
}