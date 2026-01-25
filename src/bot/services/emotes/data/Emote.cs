using ChatBot.bot.services.Static;
using Newtonsoft.Json;

namespace ChatBot.bot.services.emotes.data;

public enum EmoteId {
    Sad,
    Wait,
    Rizz,
    Jail,
    Laugh,
    Aga,
    Like,
    Happy,
}

public class Emote {
    private static readonly EmotesService _emotes = (EmotesService)Services.Get(ServiceId.Emotes); 
        
    [JsonProperty("id")]
    public EmoteId Id { get; private set; }
    
    [JsonProperty("text")]
    public string Text { get; private set; }

    
    [JsonConstructor]
    public Emote(
        [JsonProperty("id")] EmoteId id,
        [JsonProperty("text")] string text) {
        Id = id;
        Text = text;
    }

    public EmoteId GetId() {
        return Id;
    }
    
    public int GetIdAsInt() {
        return (int)Id;
    }
    
    public void SetText(string text) {
        Text = text;
        _emotes.Options.Save();
    }

    public string GetText() {
        return Text;
    }
}