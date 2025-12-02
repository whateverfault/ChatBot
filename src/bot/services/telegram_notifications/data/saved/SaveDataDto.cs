using ChatBot.api.basic;
using ChatBot.bot.interfaces;

namespace ChatBot.bot.services.telegram_notifications.data.saved;

internal class SaveDataDto {
    public readonly SafeField<State> ServiceState = new SafeField<State>(State.Disabled);

    public readonly SafeField<string> BotToken = new SafeField<string>(String.Empty);

    public readonly SafeField<long> ChatId = new SafeField<long>(-1);

    public readonly SafeField<string> NotificationPrompt = new SafeField<string>("Стрим начался! {title}\\n{link}");

    public readonly SafeField<long> Cooldown = new SafeField<long>(3600);

    public readonly SafeField<bool> IsSent = new SafeField<bool>(false);

    public readonly SafeField<long> LastSent = new SafeField<long>(0);

    public readonly SafeField<long> LastMessageId = new SafeField<long>(-1);


    public SaveDataDto() { }
    
    public SaveDataDto(
        State serviceState,
        string botToken,
        long chatId,
        string notificationPrompt,
        long cooldown,
        bool isSent,
        long lastSent,
        long lastMessageId) {
        ServiceState.Value = serviceState;
        BotToken.Value = botToken;
        ChatId.Value = chatId;
        NotificationPrompt.Value = notificationPrompt;
        Cooldown.Value = cooldown;
        IsSent.Value = isSent;
        LastSent.Value = lastSent;
        LastMessageId.Value = lastMessageId;
    }
}