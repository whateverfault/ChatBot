using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.interpreter.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }

    [JsonProperty("vars")]
    public Dictionary<string, StoredVariable> Variables { get; set; } = null!;


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("vars")] Dictionary<string, StoredVariable> vars) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  vars
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        Variables = dto.Variables.Value;
    }
}