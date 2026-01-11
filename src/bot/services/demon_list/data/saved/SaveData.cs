using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.demon_list.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }

    [JsonProperty("def_username")]
    public string DefaultUserName { get; set; } = null!;

    [JsonProperty("use_def_username")]
    public bool UseDefaultUserName { get; set; }
    

    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("def_username")] string defaultUserName,
        [JsonProperty("use_def_username")] bool useDefaultUserName) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  defaultUserName,
                                  useDefaultUserName
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        DefaultUserName = dto.DefaultUserName.Value;
        UseDefaultUserName = dto.UseDefaultUserName.Value;
    }
}