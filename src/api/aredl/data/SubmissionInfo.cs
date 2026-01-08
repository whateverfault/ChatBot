using Newtonsoft.Json;

namespace ChatBot.api.aredl.data;

public class SubmissionInfo {
    public readonly string? Id;
    public readonly UserInfo? SubmittedBy;
    public readonly bool? Mobile;
    public readonly string? VideoUrl;

    
    [JsonConstructor]
    public SubmissionInfo(
        [JsonProperty("id")] string? id,
        [JsonProperty("submitted_by")] UserInfo? submittedBy,
        [JsonProperty("mobile")] bool? mobile,
        [JsonProperty("video_url")] string? videoUrl) {
        Id = id;
        Mobile = mobile;
        SubmittedBy = submittedBy;
        VideoUrl = videoUrl;
    }
}
