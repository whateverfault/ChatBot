using Newtonsoft.Json;

namespace ChatBot.bot.services.casino.data;

public class Duel {
    [JsonProperty("subject")]
    public string Subject { get; private set; }
    
    [JsonProperty("object")]
    public string Object { get; private set; }
    
    [JsonProperty("money")]
    public long Quantity { get; private set; }
    
    
    [JsonConstructor]
    public Duel(
        [JsonProperty("subject")] string subject,
        [JsonProperty("object")] string obj,
        [JsonProperty("money")] long quantity) {
        Subject = subject;
        Object = obj;
        Quantity = quantity;
    }
}