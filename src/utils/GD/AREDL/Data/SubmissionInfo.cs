using Newtonsoft.Json;

namespace ChatBot.utils.GD.AREDL.Data;

public class SubmissionInfo {
    public string? id;
    public UserInfo? submittedBy;
    public bool? mobile;
    public string? videoUrl;

    
    public SubmissionInfo(
        [JsonProperty(PropertyName = "id")] string? id,
        [JsonProperty(PropertyName = "submitted_by")] UserInfo? submittedBy,
        [JsonProperty(PropertyName = "mobile")] bool? mobile,
        [JsonProperty(PropertyName = "video_url")] string? videoUrl) {
        this.submittedBy = submittedBy;
        this.id = id;
        this.mobile = mobile;
        this.videoUrl = videoUrl;
    }
}
