using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class HardestInfo {
    public string Id;
    public string Name;


    [JsonConstructor]
    public HardestInfo(
        [JsonProperty(PropertyName = "id")] string id,
        [JsonProperty(PropertyName = "name")] string name) {
        Id = id;
        Name = name;
    }
}