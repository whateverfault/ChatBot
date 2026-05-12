using Newtonsoft.Json;

namespace ChatBot.api.demon_list.glist.data;

public class UserData {
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("placement")]
    public int Placement { get; set; }
    
    [JsonProperty("username")]
    public string Username { get; set; }
    
    [JsonProperty("points")]
    public string Points { get; set; }
    
    
    public UserData(
        [JsonProperty("id")] int id,
        [JsonProperty("placement")] int placement,
        [JsonProperty("username")] string username,
        [JsonProperty("points")] string points) {
        Id = id;
        Placement = placement;
        Username = username;
        Points = points;
    }
}