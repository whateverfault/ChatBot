using Newtonsoft.Json;

namespace ChatBot.api.twitch.shared.requests.data;

public class GameData
{
    [JsonProperty("id")]
    public string? Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("box_art_url")]
    public string BoxArtUrl { get; set; }
    
    
    public GameData(
        [JsonProperty("id")] string? id,
        [JsonProperty("name")] string name,
        [JsonProperty("box_art_url")] string boxArtUrl
        ) {
        Id = id;
        Name = name;
        BoxArtUrl = boxArtUrl;
    }
}