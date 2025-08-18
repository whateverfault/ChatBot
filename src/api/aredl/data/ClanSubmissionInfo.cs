using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class ClanSubmissionInfo {
    public string? id;
    public string? submittedBy;
    public bool? mobile;
    public string? videoUrl;
    public RecordLevelInfo? level;

    
    [JsonConstructor]
    public ClanSubmissionInfo(
        [JsonProperty(PropertyName = "id")] string? id,
        [JsonProperty(PropertyName = "submitted_by")] string? submittedBy,
        [JsonProperty(PropertyName = "mobile")] bool? mobile,
        [JsonProperty(PropertyName = "video_url")] string? videoUrl, 
        [JsonProperty(PropertyName = "level")] RecordLevelInfo? level) {
        this.submittedBy = submittedBy;
        this.id = id;
        this.mobile = mobile;
        this.videoUrl = videoUrl;
        this.level = level;
    }
}