using ChatBot.utils.HowLongToBeat.Response.Data;
using Newtonsoft.Json;

namespace ChatBot.utils.HowLongToBeat.Response;

public class FetchGameInfoResponse {
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("count")]
    public int Count { get; set; }
    [JsonProperty("data")]
    public List<GameInfo> GameInfo { get; set; }
    

    [JsonConstructor]
    public FetchGameInfoResponse(
        [JsonProperty("title")] string title,
        [JsonProperty("count")] int count,
        [JsonProperty("data")] List<GameInfo> gameInfo) {
        Title = title;
        Count = count;
        GameInfo = gameInfo;
    }
}