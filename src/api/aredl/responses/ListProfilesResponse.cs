using ChatBot.api.aredl.data;
using Newtonsoft.Json;

namespace ChatBot.api.aredl.responses;

public class ListProfilesResponse {
    public List<UserProfile> Data;


    public ListProfilesResponse(
        [JsonProperty("data")] List<UserProfile> data) {
        Data = data;
    }
}