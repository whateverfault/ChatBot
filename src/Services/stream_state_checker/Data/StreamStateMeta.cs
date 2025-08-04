using Newtonsoft.Json;

namespace ChatBot.services.stream_state_checker.Data;

public class StreamStateMeta {
    [JsonProperty("check_cooldown")]
    public long CheckCooldown { get; set; }
    [JsonProperty("last_checked")]
    public long LastChecked { get; set; }


    [JsonConstructor]
    public StreamStateMeta(
        [JsonProperty("check_cooldown")] long checkCooldown,
        [JsonProperty("last_checked")] long lastChecked) {
        CheckCooldown = checkCooldown;
        LastChecked = lastChecked;
    }
}