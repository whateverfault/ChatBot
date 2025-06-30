using ChatBot.utils.HowLongToBeat.Request.Data;
using Newtonsoft.Json;

namespace ChatBot.Services.game_requests.Data;

public class GameRequestsFilter {
    [JsonProperty("platform")]
    public string Platforms { get; set; }
    [JsonProperty("range_time")]
    public RangeTime RangeTime { get; set; }
    [JsonProperty("range_year")]
    public RangeYear RangeYear { get; set; }


    public GameRequestsFilter() {
        Platforms = "";
        RangeTime = new RangeTime();
        RangeYear = new RangeYear();
    }
    
    [JsonConstructor]
    public GameRequestsFilter(
        [JsonProperty("platform")] string platforms,
        [JsonProperty("range_time")] RangeTime rangeTime,
        [JsonProperty("range_year")] RangeYear rangeYear) {
        Platforms = platforms;
        RangeTime = rangeTime;
        RangeYear = rangeYear;
    }
}