using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.telegram;

public class SaveData {
    [JsonProperty(PropertyName ="service_state")]
    public State ServiceState { get; set; }

    [JsonProperty(PropertyName ="bot_token")]
    public string BotToken { get; set; } = string.Empty;

    [JsonProperty(PropertyName ="chat_id")]
    public long ChatId { get; set; }
    
    [JsonProperty(PropertyName ="notification_prompt")]
    public string NotificationPrompt { get; set; } = string.Empty;

    [JsonProperty(PropertyName ="cooldown")]
    public long Cooldown { get; set; }

    [JsonProperty(PropertyName ="last_msg_id")]
    public int? LastMessageId { get; set; }


    public SaveData() {
        NotificationPrompt = NotificationPrompt = "Стрим начался! {title}\n{link}";
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName ="bot_token")] string botToken,
        [JsonProperty(PropertyName ="chat_id")] long chatId,
        [JsonProperty(PropertyName ="notification_prompt")] string notificationPrompt,
        [JsonProperty(PropertyName ="cooldown")] long cooldown,
        [JsonProperty(PropertyName ="last_msg_id")] int? lastMessageId) {
        ServiceState = serviceState;
        BotToken = botToken;
        ChatId = chatId;
        NotificationPrompt = notificationPrompt;
        Cooldown = cooldown;
        LastMessageId = lastMessageId;
    }
}