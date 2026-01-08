using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class LevelDetails {
    public readonly string Id;
    public readonly string Name;
    public readonly int Position;
    public readonly List<SubmissionInfo> Verifications;

    [JsonConstructor]
    public LevelDetails(
        [JsonProperty("id")] string id,
        [JsonProperty("position")] int position,
        [JsonProperty("name")] string name,
        [JsonProperty("verifications")] List<SubmissionInfo> verifications) {
        Id = id;
        Position = position;
        Name = name;
        Verifications = verifications;
    }
}