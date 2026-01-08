using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class ClanData {
    public readonly string Id;
    public readonly string GlobalName;
    public readonly string Tag;
    public string Description;

    
    public ClanData(
        [JsonProperty("id")] string id,
        [JsonProperty("tag")] string tag,
        [JsonProperty("global_name")] string globalName,
        [JsonProperty("description")] string description) {
        Id = id;
        Tag = tag;
        GlobalName = globalName;
        Description = description;
    }
}