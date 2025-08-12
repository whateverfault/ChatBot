using Newtonsoft.Json;

namespace ChatBot.api.shared.responses.GetUserInfo;

public class GetUserInfoResponse {
    [JsonProperty("data")]
    public UserInfo[] Data { get; private set; }
}