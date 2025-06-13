using Newtonsoft.Json;

namespace ChatBot.utils.GD.AREDL.Data;

public class RecordInfo {
    public string id;
    public string videoUrl;
    public RecordLevelInfo level;

    
    [JsonConstructor]
    public RecordInfo(
        [JsonProperty(PropertyName = "id")] string id,
        [JsonProperty(PropertyName = "video_url")] string videoUrl,
        [JsonProperty(PropertyName = "level")] RecordLevelInfo level) {
        this.id = id;
        this.videoUrl = videoUrl;
        this.level = level;
    }
}