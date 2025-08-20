using ChatBot.api.aredl.data;
using Newtonsoft.Json;

namespace ChatBot.api.aredl.responses;

public class ListClansResponse {
    public List<ClanInfo> Data { get; private set; }

    
    [JsonConstructor]
    public ListClansResponse(
        [JsonProperty("data")] List<ClanInfo> data) {
        Data = data;
    }
}