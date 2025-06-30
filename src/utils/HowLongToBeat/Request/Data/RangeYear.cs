using Newtonsoft.Json;

namespace ChatBot.utils.HowLongToBeat.Request.Data;

public class RangeYear {
    [JsonProperty("min")]
    public string Min { get; set; }
    [JsonProperty("max")]
    public string Max { get; set; }


    public RangeYear() {
        Min = "";
        Max = "";
    }

    [JsonConstructor]
    public RangeYear(
        [JsonProperty("min")] string min,
        [JsonProperty("max")] string max
    ) {
        Min = min;
        Max = max;
    }
}