using ChatBot.Services.Static;
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
        ModAction = modAction;
        Warns = 0;
    }
    
    [JsonConstructor]
    public WarnedUser(
        [JsonProperty(PropertyName = "user_id")] string userId,
        [JsonProperty(PropertyName = "warns")] int warns,
        [JsonProperty(PropertyName = "moderation_action")] ModAction modAction
        ) {
        UserId = userId;
        Warns = warns;
        ModAction = modAction;
    }

    public void GiveWarn() {
        Warns++;
        ServiceManager.GetService(ServiceName.Moderation).Options.Save();
    }
}