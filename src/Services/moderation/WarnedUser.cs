using Newtonsoft.Json;

namespace ChatBot.Services.moderation;

public class WarnedUser {
    [JsonProperty(PropertyName = "user_id")]
    public string UserId { get; }
    [JsonProperty(PropertyName = "warns")]
    public int Warns { get; private set; }
    [JsonProperty(PropertyName = "moderation_action")]
    public ModAction ModAction { get; }
    
    
    public WarnedUser() {}

    public WarnedUser(string userId, ModAction modAction) {
        UserId = userId;
        Warns = 0;
        ModAction = modAction;
    }

    public void GiveWarn() {
        Warns++;
    }
}