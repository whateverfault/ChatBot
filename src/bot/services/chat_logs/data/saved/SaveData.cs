using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.chat_logs.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }
    
    [JsonProperty("logs")]
    public List<Message> Logs { get; private set; } = null!;


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("logs")] List<Message> logs) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  logs
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        Logs = dto.Logs.Value;
    }
}