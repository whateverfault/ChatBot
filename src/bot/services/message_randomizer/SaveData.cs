using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_logs;
using Newtonsoft.Json;
using Range = ChatBot.api.basic.Range;

namespace ChatBot.bot.services.message_randomizer;

internal class SaveData {
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
    
    [JsonProperty(PropertyName ="spreading")]
    public Range Spreading { get; set; }


    public SaveData() {
        CounterMax = 25;
        Spreading = new Range(15, 30);
    }
    
    public SaveData(
        [JsonProperty(PropertyName ="counter_max")] int counterMax,
        [JsonProperty(PropertyName ="service_state")] State serviceState,
        [JsonProperty(PropertyName ="randomness")] State randomness,
        [JsonProperty(PropertyName ="spreading")] Range spreading,
        [JsonProperty(PropertyName ="message_state")] MessageState messageState,
        [JsonProperty(PropertyName ="last_generated_message")] Message lastGeneratedMessage
        ) {
        CounterMax = counterMax;
        ServiceState = serviceState;
        Randomness = randomness;
        Spreading = spreading;
        MessageState = messageState;
        LastGeneratedMessage = lastGeneratedMessage;
    }
}