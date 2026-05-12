using ChatBot.api.demon_list.glist.data;
using Newtonsoft.Json;

namespace ChatBot.api.demon_list.glist.responses;

public class GetUserProfileResponse {
    [JsonProperty("data")]
    public GListUserProfile Data { get; set; }
    
    
    public GetUserProfileResponse([JsonProperty("data")] GListUserProfile data) {
        Data = data;
    }
}