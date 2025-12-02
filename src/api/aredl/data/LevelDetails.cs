using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class LevelDetails {
    public string Id;
    public string Name;
    public readonly int Position;
    public readonly List<SubmissionInfo> Verifications;

    [JsonConstructor]
    public LevelDetails(
        [JsonProperty(PropertyName = "id")] string id,
        [JsonProperty(PropertyName = "position")] int position,
        [JsonProperty(PropertyName = "name")] string name,
        [JsonProperty(PropertyName = "verifications")] List<SubmissionInfo> verifications) {
        Id = id;
        Position = position;
        Name = name;
        Verifications = verifications;
    }
}