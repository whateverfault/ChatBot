using ChatBot.bot.interfaces;
using ChatBot.bot.services.stream_state_checker.Data;
using Newtonsoft.Json;

namespace ChatBot.bot.services.stream_state_checker.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("stream_state")]
    public StreamState StreamState { get; set; } = null!;

    [JsonProperty("stream_state_meta")]
    public StreamStateMeta StreamStateMeta { get; set; } = null!;


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("stream_state")] StreamState streamState,
        [JsonProperty("stream_state_meta")] StreamStateMeta streamStateMeta) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  streamState,
                                  streamStateMeta
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        StreamState = dto.StreamState.Value;
        StreamStateMeta = dto.StreamStateMeta.Value;
    }
}