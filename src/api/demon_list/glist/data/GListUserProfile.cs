using Newtonsoft.Json;

namespace ChatBot.api.demon_list.glist.data;

public class GListUserProfile {
    [JsonProperty("placement")]
    public int Placement { get; set; }
    
    [JsonProperty("username")]
    public string Username { get; set; }
    
    [JsonProperty("points")]
    public string Points { get; set; }
    
    [JsonProperty("levels")]
    public UserLevels? Levels { get; set; }
    
    
    public GListUserProfile(
        [JsonProperty("placement")] int placement,
        [JsonProperty("username")] string username,
        [JsonProperty("points")] string points,
        [JsonProperty("levels")] UserLevels? levels) {
        Placement = placement;
        Username = username;
        Points = points;
        Levels = levels;
    }
}