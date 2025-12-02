using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class SubmissionInfo {
    public string? Id;
    public UserInfo? SubmittedBy;
    public bool? Mobile;
    public readonly string? VideoUrl;

    
    [JsonConstructor]
    public SubmissionInfo(
        [JsonProperty(PropertyName = "id")] string? id,
        [JsonProperty(PropertyName = "submitted_by")] UserInfo? submittedBy,
        [JsonProperty(PropertyName = "mobile")] bool? mobile,
        [JsonProperty(PropertyName = "video_url")] string? videoUrl) {
        Id = id;
        Mobile = mobile;
        SubmittedBy = submittedBy;
        VideoUrl = videoUrl;
    }
}
