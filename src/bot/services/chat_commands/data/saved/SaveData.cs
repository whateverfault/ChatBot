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
    
    [JsonProperty("ai_on_mention")]
    public bool AiOnMention { get; set; }
    
    [JsonProperty("mention_сmd_id")]
    public int MentionCmdId { get; set; }
    

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
        [JsonProperty("ai_on_mention")] bool aiOnMention,
        [JsonProperty("mention_сmd_id")] int mentionCmdId) {
        var dto = new SaveDataDto(
                                  serviceState,
                                  commandIdentifier,
                                  verboseState,
                                  baseTitle,
                                  sendWhisperIfPossible,
                                  aiOnMention,
                                  mentionCmdId
                                  );

        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        CommandIdentifier = dto.CommandIdentifier.Value;
        VerboseState = dto.VerboseState.Value;
        BaseTitle = dto.BaseTitle.Value;
        SendWhisperIfPossible = dto.SendWhisperIfPossible.Value;
        AiOnMention = dto.AiOnMention.Value;
        MentionCmdId = dto.MentionCmdId.Value;
    }
}