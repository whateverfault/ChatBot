using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.text_generator.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; } 
    
    [JsonProperty("context_size")]
    public int ContextSize { get; set; }
    
    [JsonProperty("max_length")]
    public int MaxLength { get; set; }
    
    [JsonProperty("model")]
    public Dictionary<string, Dictionary<string, int>> Model { get; private set; } = null!;


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("context_size")] int contextSize,
        [JsonProperty("max_length")] int maxLength,
        [JsonProperty("model")] Dictionary<string, Dictionary<string, int>> model) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  contextSize,
                                  maxLength,
                                  model
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        ContextSize = dto.ContextSize.Value;
        MaxLength = dto.MaxLength.Value;
        Model = dto.Model.Value;
    }
}