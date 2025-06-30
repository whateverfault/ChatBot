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
    public int CompMain { get; set; }
    [JsonProperty("comp_plus")]
    public int CompPlus { get; set; }
    [JsonProperty("review_score")]
    public int ReviewScore { get; set; }
    [JsonProperty("profile_platform")]
    public string Platforms { get; set; }
    [JsonProperty("release_world")]
    public int ReleaseYear { get; set; }
    
    
    public GameInfo(
        [JsonProperty("game_id")] int gameId,
        [JsonProperty("game_name")] string gameName,
        [JsonProperty("game_alias")] string gameAlias,
        [JsonProperty("comp_main")] int compMain,
        [JsonProperty("comp_plus")] int compPlus,
        [JsonProperty("review_score")] int reviewScore,
        [JsonProperty("profile_platform")] string platforms,
        [JsonProperty("release_world")] int releaseYear) {
        GameId = gameId;
        GameName = gameName;
        GameAlias = gameAlias;
        CompMain = compMain;
        CompPlus = compPlus;
        ReviewScore = reviewScore;
        Platforms = platforms;
        ReleaseYear = releaseYear;
    }
}