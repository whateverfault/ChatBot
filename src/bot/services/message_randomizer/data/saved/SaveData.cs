using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_logs.data;
using Newtonsoft.Json;
using Range = ChatBot.api.basic.Range;

namespace ChatBot.bot.services.message_randomizer.data.saved;

internal class SaveData {
    [JsonProperty("counter_max")]
    public int CounterMax { get; set; }
    
    [JsonProperty("last_generated_message")]
    public Message LastGeneratedMessage { get; set; } = null!;

    [JsonProperty("message_state")]
    public MessageState MessageState { get; set; }
    
    [JsonProperty("randomness")]
    public State Randomness { get; set; }
    
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("spreading")]
    public Range Spreading { get; set; } = null!;


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("last_generated_message")] Message lastGeneratedMessage, 
        [JsonProperty("message_state")] MessageState messageState,
        [JsonProperty("counter_max")] int counterMax,
        [JsonProperty("randomness")] State randomness,
        [JsonProperty("spreading")] Range spreading) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  lastGeneratedMessage,
                                  messageState,
                                  counterMax,
                                  randomness,
                                  spreading
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        CounterMax = dto.CounterMax.Value;
        ServiceState = dto.ServiceState.Value;
        Randomness = dto.Randomness.Value;
        Spreading = dto.Spreading.Value;
        MessageState = dto.MessageState.Value;
        LastGeneratedMessage = dto.LastGeneratedMessage.Value;
    }
}