using ChatBot.bot.services.stream_state_checker.Data;
using ChatBot.bot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.stream_state_checker;

public class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    [JsonProperty("stream_state")]
    public StreamState StreamState { get; set; }
    [JsonProperty("stream_state_meta")]
    public StreamStateMeta StreamStateMeta { get; set; }


    public SaveData() {
        ServiceState = State.Disabled;
        StreamState = new StreamState();
        StreamStateMeta = new StreamStateMeta(120, 0);
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName ="service_state")] State serviceState,
        [JsonProperty(PropertyName ="stream_state")] StreamState streamState,
        [JsonProperty("stream_state_meta")] StreamStateMeta streamStateMeta) {
        ServiceState = serviceState;
        StreamState = streamState;
        StreamStateMeta = streamStateMeta;
    }
}