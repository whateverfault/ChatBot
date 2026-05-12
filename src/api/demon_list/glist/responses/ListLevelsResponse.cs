using ChatBot.api.demon_list.glist.data;
using Newtonsoft.Json;

namespace ChatBot.api.demon_list.glist.responses;

public class ListLevelsResponse {
    [JsonProperty("data")]
    public LevelsData Data { get; private set; }

    
    public ListLevelsResponse(
        [JsonProperty("data")] LevelsData data) {
        Data = data;
    }
}