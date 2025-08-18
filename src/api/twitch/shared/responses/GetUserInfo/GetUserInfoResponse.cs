using Newtonsoft.Json;

namespace ChatBot.api.twitch.shared.responses.GetUserInfo;

public class GetUserInfoResponse {
    [JsonProperty("data")]
    public UserInfo[] Data { get; private set; }
    
    
    [JsonConstructor]
    public GetUserInfoResponse(
        [JsonProperty("data")] UserInfo[] data) {
        Data = data;
    }
}