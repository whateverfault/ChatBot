using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class ClanData {
    public readonly string Id;
    public readonly string GlobalName;
    public readonly string Tag;
    public string Description;

    
    public ClanData(
        [JsonProperty(PropertyName = "id")] string id,
        [JsonProperty(PropertyName = "tag")] string tag,
        [JsonProperty(PropertyName = "global_name")] string globalName,
        [JsonProperty(PropertyName = "description")] string description) {
        Id = id;
        Tag = tag;
        GlobalName = globalName;
        Description = description;
    }
}