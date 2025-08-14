using Newtonsoft.Json;

namespace ChatBot.utils.GD.AREDL.Data;

public class ClanData {
    public string id;
    public string globalName;
    public string tag;
    public string description;

    
    public ClanData(
        [JsonProperty(PropertyName = "id")] string id,
        [JsonProperty(PropertyName = "global_name")] string globalName,
        [JsonProperty(PropertyName = "tag")] string tag,
        [JsonProperty(PropertyName = "description")] string description) {
        this.id = id;
        this.globalName = globalName;
        this.tag = tag;
        this.description = description;
    }
}