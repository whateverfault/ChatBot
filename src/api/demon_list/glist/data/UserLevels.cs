using Newtonsoft.Json;

namespace ChatBot.api.demon_list.glist.data;

public class UserLevels {
    [JsonProperty("hardest")]
    public ShortLevelInfo? Hardest { get; set; }
}