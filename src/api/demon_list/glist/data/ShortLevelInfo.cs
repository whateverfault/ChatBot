using Newtonsoft.Json;

namespace ChatBot.api.demon_list.glist.data;

public class ShortLevelInfo {
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("placement")]
    public int Position { get; set; }
    
    [JsonProperty("video_url")]
    public string VerificationLink { get; set; }

    
    public ShortLevelInfo(
        [JsonProperty("id")] int id,
        [JsonProperty("name")] string name,
        [JsonProperty("placement")] int position,
        [JsonProperty("video_url")] string verificationLink) {
        Id = id;
        Name = name;
        Position = position;
        VerificationLink = verificationLink;
    }
}