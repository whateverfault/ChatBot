using ChatBot.bot.services.chat_logs;
using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.message_randomizer;

public class SaveData {
    [JsonProperty(PropertyName ="counter_max")]
    public int CounterMax { get; set; }
    [JsonProperty(PropertyName ="last_generated_message")]
    public Message LastGeneratedMessage { get; set; } = null!;

    [JsonProperty(PropertyName ="message_state")]
    public MessageState MessageState { get; set; }
    [JsonProperty(PropertyName ="randomness")]
    public State Randomness { get; set; }
    [JsonProperty(PropertyName ="service_state")]
    public State ServiceState { get; set; }
    [JsonProperty(PropertyName ="spreading_from")]
    public int SpreadingFrom { get; set; }
    [JsonProperty(PropertyName ="spreading_to")]
    public int SpreadingTo { get; set; }


    public SaveData() {
        CounterMax = 25;
        SpreadingFrom = 15;
        SpreadingTo = 30;
    }
    
    public SaveData(
        [JsonProperty(PropertyName ="counter_max")] int counterMax,
        [JsonProperty(PropertyName ="service_state")] State serviceState,
        [JsonProperty(PropertyName ="randomness")] State randomness,
        [JsonProperty(PropertyName ="spreading_from")] int spreadingFrom,
        [JsonProperty(PropertyName ="spreading_to")] int spreadingTo,
        [JsonProperty(PropertyName ="message_state")] MessageState messageState,
        [JsonProperty(PropertyName ="last_generated_message")] Message lastGeneratedMessage
        ) {
        CounterMax = counterMax;
        ServiceState = serviceState;
        Randomness = randomness;
        SpreadingFrom = spreadingFrom;
        SpreadingTo = spreadingTo;
        MessageState = messageState;
        LastGeneratedMessage = lastGeneratedMessage;
    }
}