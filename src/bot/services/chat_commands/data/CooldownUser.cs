using Newtonsoft.Json;

namespace ChatBot.bot.services.chat_commands.data;

public class CooldownUser {
    [JsonProperty("user_id")]
    public string UserId { get; private set; }
    
    [JsonProperty("used_at")]
    public long UsedAt { get; private set; }
    
    
    [JsonConstructor]
    public CooldownUser(string userId) {
        UserId = userId;
        UsedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}