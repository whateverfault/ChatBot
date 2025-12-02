using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class ClanSubmissionInfo {
    public string? Id;
    public string? SubmittedBy;
    public bool? Mobile;
    public readonly string? VideoUrl;
    public readonly RecordLevelInfo? Level;

    
    [JsonConstructor]
    public ClanSubmissionInfo(
        [JsonProperty(PropertyName = "id")] string? id,
        [JsonProperty(PropertyName = "submitted_by")] string? submittedBy,
        [JsonProperty(PropertyName = "mobile")] bool? mobile,
        [JsonProperty(PropertyName = "video_url")] string? videoUrl, 
        [JsonProperty(PropertyName = "level")] RecordLevelInfo? level) {
        SubmittedBy = submittedBy;
        Id = id;
        Mobile = mobile;
        VideoUrl = videoUrl;
        Level = level;
    }
}