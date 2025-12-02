using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class RecordInfo {
    public string Id;
    public readonly string VideoUrl;
    public readonly RecordLevelInfo Level;

    
    [JsonConstructor]
    public RecordInfo(
        [JsonProperty(PropertyName = "id")] string id,
        [JsonProperty(PropertyName = "video_url")] string videoUrl,
        [JsonProperty(PropertyName = "level")] RecordLevelInfo level) {
        Id = id;
        VideoUrl = videoUrl;
        Level = level;
    }
}