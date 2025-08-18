using ChatBot.api.aredl.data;
using Newtonsoft.Json;

namespace ChatBot.api.aredl.responses;

public class GetClanResponse {
    public List<ClanInfo?> data;

    
    [JsonConstructor]
    public GetClanResponse(
        [JsonProperty(PropertyName = "data")] List<ClanInfo?> data) {
        this.data = data;
    }
}