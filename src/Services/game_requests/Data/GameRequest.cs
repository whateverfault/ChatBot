using Newtonsoft.Json;

namespace ChatBot.Services.game_requests.Data;

public class GameRequest {
    [JsonProperty("game_id")]
    public int GameId { get; private set; }
    [JsonProperty("game_name")]
    public string GameName { get; private set; }
    [JsonProperty("game_lenght")]
    public int GameLenght { get; private set; }
    [JsonProperty("release_year")]
    public int ReleaseYear { get; private set; }
    [JsonProperty("username")]
    public string RequesterUsername { get; private set; }
    
    
    [JsonConstructor]
    public GameRequest(
        [JsonProperty("game_id")] int gameId,
        [JsonProperty("game_name")] string gameName,
        [JsonProperty("game_lenght")] int gameLenght,
        [JsonProperty("release_year")] int releaseYear,
        [JsonProperty("username")] string requesterUsername) {
        GameId = gameId;
        GameName = gameName;
        GameLenght = gameLenght;
        ReleaseYear = releaseYear;
        RequesterUsername = requesterUsername;
    }
}