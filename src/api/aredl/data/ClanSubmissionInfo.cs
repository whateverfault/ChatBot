using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class ClanSubmissionInfo {
    public readonly string Id;
    public readonly bool Mobile;
    public readonly bool IsVerification;
    public readonly string VideoUrl;
    
    public readonly RecordLevelInfo? Level;

    
    [JsonConstructor]
    public ClanSubmissionInfo(
        [JsonProperty("id")] string id,
        [JsonProperty("mobile")] bool mobile,
        [JsonProperty("is_verification")] bool isVerification,
        [JsonProperty("video_url")] string videoUrl,
        [JsonProperty("level")] RecordLevelInfo level) {
        Id = id;
        Mobile = mobile;
        IsVerification = isVerification;
        VideoUrl = videoUrl;
        Level = level;
    }
}