using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.message_filter;

public class SaveData {
    [JsonProperty("filters")]
    public List<Filter> Filters { get; }
    [JsonProperty("state")]
    public State State { get; set; }


    public SaveData() {
        Filters = [
                      new Filter(
                                 "Level Requests",
                                 @"\b\d{8,11}\b",
                                 true
                                ),
                      new Filter(
                                 "Special Symbols",
                                 "^[!@~]+",
                                 true
                                ),
                  ];
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName ="filters")] List<Filter> filters,
        [JsonProperty(PropertyName ="state")] State state) {
        Filters = filters;
        State = state;
    }
}