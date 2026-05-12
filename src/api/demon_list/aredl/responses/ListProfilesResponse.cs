using ChatBot.api.demon_list.aredl.data;
using Newtonsoft.Json;

namespace ChatBot.api.demon_list.aredl.responses;

public class ListProfilesResponse {
    public readonly List<AredlUserProfile> Data;


    public ListProfilesResponse(
        [JsonProperty("data")] List<AredlUserProfile> data) {
        Data = data;
    }
}