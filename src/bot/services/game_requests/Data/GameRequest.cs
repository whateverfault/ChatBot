using Newtonsoft.Json;

namespace ChatBot.bot.services.game_requests.Data;

public class GameRequest {
    [JsonProperty("game_name")]
    public string GameName { get; private set; }
    
    [JsonProperty("user_id")]
    public string UserId { get; private set; }
    
    
    [JsonConstructor]
    public GameRequest(
        [JsonProperty("game_name")] string gameName,
        [JsonProperty("user_id")] string userId) {
        GameName = gameName;
        UserId = userId;
    }
}