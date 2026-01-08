using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class RecordLevelInfo {
    public readonly string Id;
    public readonly string Name;
    public readonly int Position;
    public readonly bool Legacy;


    [JsonConstructor]
    public RecordLevelInfo(
        [JsonProperty("id")] string id,
        [JsonProperty("name")] string name,
        [JsonProperty("position")] int position, 
        [JsonProperty("legacy")] bool legacy) {
        Id = id;
        Name = name;
        Position = position;
        Legacy = legacy;
    }
}