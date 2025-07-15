using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.message_filter;

public class SaveData {
    [JsonProperty(PropertyName ="filters")]
    public List<Filter> Filters { get; }
    [JsonProperty(PropertyName ="state")]
    public State State { get; set; }


    public SaveData(
        [JsonProperty(PropertyName ="filters")] List<Filter> filters,
        [JsonProperty(PropertyName ="state")] State state) {
        State = state;
        Filters = filters;
    }
}