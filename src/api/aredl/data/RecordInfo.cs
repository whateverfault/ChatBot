using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class RecordInfo {
    public readonly string Id;
    public readonly string VideoUrl;
    public readonly bool IsVerification;
    
    public readonly RecordLevelInfo Level;

    
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