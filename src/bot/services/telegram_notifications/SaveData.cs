using ChatBot.bot.interfaces;
using Newtonsoft.Json;

namespace ChatBot.bot.services.telegram_notifications;

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

    [JsonProperty("is_sent")]
    public bool IsSent { get; set; }
    
    [JsonProperty("last_sent")]
    public long LastSent { get; set; }
    
    [JsonProperty("last_msg_id")]
    public long LastMessageId { get; set; }


    public SaveData() {
        LastSent = 0;
        LastMessageId = -1;
        
        NotificationPrompt = NotificationPrompt = "Стрим начался! {title}\n{link}";
        BotToken = string.Empty;
    }
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty( "service_state")] State serviceState,
        [JsonProperty("bot_token")] string botToken,
        [JsonProperty("chat_id")] long chatId,
        [JsonProperty("notification_prompt")] string notificationPrompt,
        [JsonProperty("cooldown")] long cooldown,
        [JsonProperty("is_sent")] bool isSent,
        [JsonProperty("last_sent")] long lastSent,
        [JsonProperty("last_msg_id")] long lastMessageId) {
        ServiceState = serviceState;
        BotToken = botToken;
        ChatId = chatId;
        NotificationPrompt = notificationPrompt;
        Cooldown = cooldown;
        IsSent = isSent;
        LastMessageId = lastMessageId;
    }
}