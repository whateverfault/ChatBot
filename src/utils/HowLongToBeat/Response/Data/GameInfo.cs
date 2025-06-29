using Newtonsoft.Json;

namespace ChatBot.utils.HowLongToBeat.Response.Data;

public class GameInfo {
    [JsonProperty("game_id")]
    public int GameId { get; set; }
    [JsonProperty("game_name")]
    public string GameName { get; set; }
    [JsonProperty("game_alias")]
    public string GameAlias { get; set; }
    [JsonProperty("comp_main")]
    public long CompMain { get; set; }
    [JsonProperty("comp_plus")]
    public long CompPlus { get; set; }
    [JsonProperty("review_score")]
    public int ReviewScore { get; set; }
    [JsonProperty("profile_platform")]
    public string Platforms { get; set; }
    [JsonProperty("release_world")]
    public int ReleaseDate { get; set; }
    
    
    public GameInfo(
        int gameId,
        string gameName,
        string gameAlias,
        long compMain,
        long compPlus,
        int reviewScore,
        string platforms,
        int releaseDate) {
        GameId = gameId;
        GameName = gameName;
        GameAlias = gameAlias;
        CompMain = compMain;
        CompPlus = compPlus;
        ReviewScore = reviewScore;
        Platforms = platforms;
        ReleaseDate = releaseDate;
    }
}