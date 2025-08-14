using ChatBot.bot.utils.GD.AREDL.Data;
using Newtonsoft.Json;

namespace ChatBot.bot.utils.GD.AREDL.Responses;

public class GetClanResponse {
    public List<ClanInfo?> data;

    
    [JsonConstructor]
    public GetClanResponse(
        [JsonProperty(PropertyName = "data")] List<ClanInfo?> data) {
        this.data = data;
    }
}