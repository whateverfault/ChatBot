using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class ClanSubmissionInfo {
    public readonly string? Id;
    public readonly string? SubmittedBy;
    public readonly bool? Mobile;
    public readonly string? VideoUrl;
    public readonly RecordLevelInfo? Level;

    
    [JsonConstructor]
    public ClanSubmissionInfo(
        [JsonProperty("id")] string? id,
        [JsonProperty("submitted_by")] string? submittedBy,
        [JsonProperty("mobile")] bool? mobile,
        [JsonProperty("video_url")] string? videoUrl, 
        [JsonProperty("level")] RecordLevelInfo? level) {
        SubmittedBy = submittedBy;
        Id = id;
        Mobile = mobile;
        VideoUrl = videoUrl;
        Level = level;
    }
}