using Newtonsoft.Json;

namespace ChatBot.bot.services.game_requests.Data;

public class GameRequest {
    [JsonProperty("game_name")]
    public string GameName { get; private set; }
    [JsonProperty("username")]
    public string RequesterUsername { get; private set; }
    
    
    [JsonConstructor]
    public GameRequest(
        [JsonProperty("game_name")] string gameName,
        [JsonProperty("username")] string requesterUsername) {
        GameName = gameName;
        RequesterUsername = requesterUsername;
    }
}