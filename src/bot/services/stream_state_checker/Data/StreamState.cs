using Newtonsoft.Json;

namespace ChatBot.bot.services.stream_state_checker.Data;

public class StreamState {
    [JsonProperty("stream_start")]
    public long StreamStart { get; set; }
    
    [JsonProperty("last_online")]
    public long LastOnline { get; set; }
    
    [JsonProperty("was_online")]
    public bool Online { get; set; }
    
    [JsonProperty("online_time")]
    public long OnlineTime { get; set; }
    
    [JsonProperty("offline_time")]
    public long OfflineTime { get; set; }

    
    public StreamState() {
        StreamStart = 0;
        LastOnline = 0;
        Online = false;
        OnlineTime = 0;
        OfflineTime = 0;
    }
    
    [JsonConstructor]
    public StreamState(
        [JsonProperty("stream_start")] long streamStart,
        [JsonProperty("last_streamed")] long lastOnline,
        [JsonProperty("was_streaming")] bool online,
        [JsonProperty("online_time")] long onlineTime,
        [JsonProperty("offline_time")] long offlineTime) {
        StreamStart = streamStart;
        LastOnline = lastOnline;
        Online = online;
        OnlineTime = onlineTime;
        OfflineTime = offlineTime;
    }
}