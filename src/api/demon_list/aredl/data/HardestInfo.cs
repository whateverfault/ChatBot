using Newtonsoft.Json;

namespace ChatBot.api.demon_list.aredl.data;

public class HardestInfo {
    public readonly string Id;
    public readonly string Name;


    [JsonConstructor]
    public HardestInfo(
        [JsonProperty("id")] string id,
        [JsonProperty("name")] string name) {
        Id = id;
        Name = name;
    }
}