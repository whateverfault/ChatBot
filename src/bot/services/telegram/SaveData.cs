using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.telegram;

internal class SaveData {
    [JsonProperty("service_state")]
    public State ServiceState { get; set; }

    [JsonProperty("bot_token")]
    public string BotToken { get; set; }

    [JsonProperty("chat_id")]
    public long ChatId { get; set; }
    
    [JsonProperty("notification_prompt")]
    public string NotificationPrompt { get; set; }

    [JsonProperty("cooldown")]
    public long Cooldown { get; set; }

    [JsonProperty("last_sent")]
    public long? LastSent { get; set; }
    
    [JsonProperty("last_msg_id")]
    public int? LastMessageId { get; set; }


    public SaveData() {
        NotificationPrompt = NotificationPrompt = "Стрим начался! {title}\n{link}";
        BotToken = string.Empty;
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName ="bot_token")] string botToken,
        [JsonProperty(PropertyName ="chat_id")] long chatId,
        [JsonProperty(PropertyName ="notification_prompt")] string notificationPrompt,
        [JsonProperty(PropertyName ="cooldown")] long cooldown,
        [JsonProperty(PropertyName ="last_sent")] int? lastSent,
        [JsonProperty(PropertyName ="last_msg_id")] int? lastMessageId) {
        ServiceState = serviceState;
        BotToken = botToken;
        ChatId = chatId;
        NotificationPrompt = notificationPrompt;
        Cooldown = cooldown;
        LastMessageId = lastMessageId;
    }
}