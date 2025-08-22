using Newtonsoft.Json;

namespace ChatBot.api.basic;

public class Range {
    [JsonProperty("start")]
    public int Start { get; private set; }
    
    [JsonProperty("end")]
    public int End { get; private set; }


    [JsonConstructor]
    public Range(
        [JsonProperty("start")] int start,
        [JsonProperty("end")] int end) {
        Start = start;
        End = end;
    }
}