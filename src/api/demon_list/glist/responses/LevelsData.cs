using ChatBot.api.demon_list.glist.data;
using Newtonsoft.Json;

namespace ChatBot.api.demon_list.glist.responses;

public class LevelsData {
    [JsonProperty("levels")]
    public List<GListLevelInfo> Levels;


    public LevelsData([JsonProperty("levels")] List<GListLevelInfo> levels) {
        Levels = levels;
    }
}