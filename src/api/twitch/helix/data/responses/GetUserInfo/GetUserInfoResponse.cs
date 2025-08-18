using Newtonsoft.Json;

namespace ChatBot.api.twitch.helix.data.responses.GetUserInfo;

public class GetUserInfoResponse {
    [JsonProperty("data")]
    public UserInfo[] Data { get; private set; }
    
    
    [JsonConstructor]
    public GetUserInfoResponse(
        [JsonProperty("data")] UserInfo[] data) {
        Data = data;
    }
}