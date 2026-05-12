using Newtonsoft.Json;

namespace ChatBot.api.demon_list.glist.data;

public class GListLevelInfo {
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("ingame_id")]
    public int LevelId { get; set; }
    
    [JsonProperty("placement")]
    public int Position { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("points")]
    public string Points { get; set; }
    
    [JsonProperty("list_percent")]
    public int ListPercent { get; set; }
    
    [JsonProperty("length")]
    public int LengthSecs { get; set; }
    
    [JsonProperty("holder")]
    public string Creator { get; set; }
    
    [JsonProperty("verification_url")]
    public string VerificationLink { get; set; }
    
    
    [JsonConstructor]
    public GListLevelInfo(
        [JsonProperty("id")] int id,
        [JsonProperty("ingame_id")] int levelId,
        [JsonProperty("placement")] int position,
        [JsonProperty("name")] string name,
        [JsonProperty("points")] string points,
        [JsonProperty("list_percent")] int listPercent,
        [JsonProperty("length")] int lengthSecs,
        [JsonProperty("holder")] string creator,
        [JsonProperty("verification_url")] string verificationLink) {
        Id = id;
        LevelId = levelId;
        Position = position;
        Name = name;
        Points = points;
        ListPercent = listPercent;
        LengthSecs = lengthSecs;
        Creator = creator;
        VerificationLink = verificationLink;
    }
}