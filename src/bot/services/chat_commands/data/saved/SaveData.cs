using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.chat_commands.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState{ get; set; }
    
    [JsonProperty("command_identifier")]
    public char CommandIdentifier { get; set; }

    [JsonProperty("verbose_state")]
    public State VerboseState { get; set; }

    [JsonProperty("base_title")]
    public string BaseTitle { get; set; } = null!;

    [JsonProperty("swip")]
    public bool SendWhisperIfPossible { get; set; }

    [JsonProperty("use_7tv")]
    public bool Use7Tv { get; set; }
    

    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State serviceState,
        [JsonProperty("command_identifier")] char commandIdentifier,
        [JsonProperty("verbose_state")] State verboseState,
        [JsonProperty("base_title")] string baseTitle,
        [JsonProperty("swip")] bool sendWhisperIfPossible,
        [JsonProperty("use_7tv")] bool use7Tv) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  commandIdentifier,
                                  verboseState,
                                  baseTitle,
                                  sendWhisperIfPossible, 
                                  use7Tv
                                  );

        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        CommandIdentifier = dto.CommandIdentifier.Value;
        VerboseState = dto.VerboseState.Value;
        BaseTitle = dto.BaseTitle.Value;
        SendWhisperIfPossible = dto.SendWhisperIfPossible.Value;
        Use7Tv = dto.Use7Tv.Value;
    }
}