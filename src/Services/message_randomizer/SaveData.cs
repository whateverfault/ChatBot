using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.message_randomizer;

public class SaveData {
    [JsonProperty(PropertyName ="logs")]
    public List<Message> Logs { get; }
    [JsonProperty(PropertyName ="counter_max")]
    public int CounterMax { get; set; }
    [JsonProperty(PropertyName ="last_generated_message")]
    public Message LastGeneratedMessage { get; set; }
    [JsonProperty(PropertyName ="logger_state")]
    public State LoggerState { get; set; }
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


    public SaveData(int counterMax, State serviceState, State loggerState, State randomness, int spreadingFrom, int spreadingTo,
                    MessageState messageState, Message lastGeneratedMessage, List<Message> logs) {
        CounterMax = counterMax;
        ServiceState = serviceState;
        LoggerState = loggerState;
        Randomness = randomness;
        SpreadingFrom = spreadingFrom;
        SpreadingTo = spreadingTo;
        MessageState = messageState;
        LastGeneratedMessage = lastGeneratedMessage;
        Logs = logs;
    }
}