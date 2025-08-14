using Newtonsoft.Json;

namespace ChatBot.utils.GD.AREDL.Data;

public class HardestInfo {
    public string id;
    public string name;


    [JsonConstructor]
    public HardestInfo(
        [JsonProperty(PropertyName = "id")] string id,
        [JsonProperty(PropertyName = "name")] string name) {
        this.id = id;
        this.name = name;
    }
}