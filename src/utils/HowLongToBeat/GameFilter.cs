using ChatBot.utils.HowLongToBeat.Request.Data;

namespace ChatBot.utils.HowLongToBeat;

public class GameFilter {
    public string GameName { get; set; }
    public string Platform { get; set; }
    public RangeTime RangeTime { get; set; }
    public RangeYear RangeYear { get; set; }
    
    
    public GameFilter(string gameName, string platform, RangeTime rangeTime, RangeYear rangeYear) {
        GameName = gameName;
        Platform = platform;
        RangeTime = rangeTime;
        RangeYear = rangeYear;
    }
}