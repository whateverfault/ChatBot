using ChatBot.shared.interfaces;
using Newtonsoft.Json;

namespace ChatBot.Services.telegram;

public class SaveData {
    [JsonProperty(PropertyName ="service_state")]
    public State ServiceState { get; set; }

    [JsonProperty(PropertyName ="bot_token")]
    public string BotToken { get; set; } = null!;

    [JsonProperty(PropertyName ="chat_id")]
    public long ChatId { get; set; }
    
    [JsonProperty(PropertyName ="notification_prompt")]
    public string NotificationPrompt { get; set; } = null!;

    [JsonProperty(PropertyName ="cooldown")]
    public long Cooldown { get; set; }

    [JsonProperty(PropertyName ="last_streamed")]
    public long LastStreamed { get; set; }

    [JsonProperty(PropertyName ="last_msg_id")]
    public int? LastMessageId { get; set; }
    
    [JsonProperty(PropertyName ="was_streaming")]
    public bool WasStreaming { get; set; }


    public SaveData(){}
    
    [JsonConstructor]
    public SaveData(
        [JsonProperty(PropertyName = "service_state")] State serviceState,
        [JsonProperty(PropertyName ="bot_token")] string botToken,
        [JsonProperty(PropertyName ="chat_id")] long chatId,
        [JsonProperty(PropertyName ="notification_prompt")] string notificationPrompt,
        [JsonProperty(PropertyName ="cooldown")] long cooldown,
        [JsonProperty(PropertyName ="last_streamed")] long lastStreamed,
        [JsonProperty(PropertyName ="last_msg_id")] int? lastMessageId,
        [JsonProperty(PropertyName ="was_streaming")] bool wasStreaming) {
        ServiceState = serviceState;
        BotToken = botToken;
        ChatId = chatId;
        NotificationPrompt = notificationPrompt;
        Cooldown = cooldown;
        LastStreamed = lastStreamed;
        WasStreaming = wasStreaming;
        LastMessageId = lastMessageId;
    }
}