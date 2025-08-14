using Newtonsoft.Json;

namespace ChatBot.bot.utils.GD.AREDL.Data;

public class LevelDetails {
    public string id;
    public int position;
    public string name;
    public List<SubmissionInfo> verifications;

    [JsonConstructor]
    public LevelDetails(
        [JsonProperty(PropertyName = "id")] string id,
        [JsonProperty(PropertyName = "position")] int position,
        [JsonProperty(PropertyName = "name")] string name,
        [JsonProperty(PropertyName = "verifications")] List<SubmissionInfo> verifications) {
        this.id = id;
        this.position = position;
        this.name = name;
        this.verifications = verifications;
    }
}