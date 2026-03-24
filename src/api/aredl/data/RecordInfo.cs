using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class RecordInfo {
    [JsonProperty("id")]
    public string Id { get; private set; }
    
    [JsonProperty("video_url")]
    public string VideoUrl { get; private set; }
    
    [JsonProperty("is_verification")]
    public bool IsVerification { get; private set; }
    
    [JsonProperty("level")]
    public RecordLevelInfo Level { get; private set; }

    
    [JsonConstructor]
    public RecordInfo(
        [JsonProperty("id")] string id,
        [JsonProperty("video_url")] string videoUrl,
        [JsonProperty("is_verification")] bool isVerification,
        [JsonProperty("level")] RecordLevelInfo level) {
        Id = id;
        VideoUrl = videoUrl;
        IsVerification = isVerification;
        Level = level;
    }
}