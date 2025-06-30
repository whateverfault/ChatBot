using Newtonsoft.Json;

namespace ChatBot.utils.HowLongToBeat.Request.Data;

public class RangeTime {
    [JsonProperty("min")]
    public int? Min { get; set; }
    [JsonProperty("max")]
    public int? Max { get; set; }


    public RangeTime() {
        Min = null;
        Max = null;
    }

    [JsonConstructor]
    public RangeTime(
        [JsonProperty("min")] int? min,
        [JsonProperty("max")] int? max
    ) {
        Min = min;
        Max = max;
    }
}