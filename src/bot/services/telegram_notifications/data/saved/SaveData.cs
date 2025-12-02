using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.telegram_notifications.data.saved;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }

    [JsonProperty("bot_token")]
    public string BotToken { get; set; } = null!;

    [JsonProperty("chat_id")]
    public long ChatId { get; set; }
    
    [JsonProperty("notification_prompt")]
    public string NotificationPrompt { get; set; } = null!;

    [JsonProperty("cooldown")]
    public long Cooldown { get; set; }

    [JsonProperty("is_sent")]
    public bool IsSent { get; set; }
    
    [JsonProperty("last_sent")]
    public long LastSent { get; set; }
    
    [JsonProperty("last_msg_id")]
    public long LastMessageId { get; set; }


    public SaveData() {
        FromDto(new SaveDataDto());
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty("service_state")] State state,
        [JsonProperty("bot_token")] string botToken,
        [JsonProperty("chat_id")] long chatId,
        [JsonProperty("notification_prompt")] string notificationPrompt,
        [JsonProperty("cooldown")] long cooldown,
        [JsonProperty("is_sent")] bool isSent,
        [JsonProperty("last_sent")] long lastSent,
        [JsonProperty("last_msg_id")] long lastMessageId) {
        var dto = new SaveDataDto(
                                  state,
                                  botToken,
                                  chatId,
                                  notificationPrompt,
                                  cooldown,
                                  isSent,
                                  lastSent,
                                  lastMessageId
                                  );
        FromDto(dto);
    }

    private void FromDto(SaveDataDto dto) {
        ServiceState = dto.ServiceState.Value;
        BotToken = dto.BotToken.Value;
        ChatId = dto.ChatId.Value;
        NotificationPrompt = dto.NotificationPrompt.Value;
        Cooldown = dto.Cooldown.Value;
        IsSent = dto.IsSent.Value;
        LastMessageId = dto.LastMessageId.Value;
    }
}