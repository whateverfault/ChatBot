using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.telegram_notifications.data.saved;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.telegram_notifications;

public class TgNotificationsOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    private static string Name => "tg_notifications";
    private static string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    
    public string BotToken => _saveData!.BotToken;
    
    public long ChatId => _saveData!.ChatId;

    public string NotificationPrompt => _saveData!.NotificationPrompt;

    public long Cooldown => _saveData!.Cooldown;

    public bool IsSent => _saveData!.IsSent;
    
    public long LastSent => _saveData!.LastSent;
    
    public long LastMessageId => _saveData!.LastMessageId;
    
    
    public override void Load() {
        if (!Json.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
    }

    public override void Save() {
        lock (_fileLock) {
            Json.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
        }
    }

    public override void SetDefaults() {
        _saveData = new SaveData();
        Save();
    }

    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }
    
    public void SetBotToken(string token) {
        _saveData!.BotToken = token;
        Save();
    }
    
    public void SetChatId(long chatId) {
        _saveData!.ChatId = chatId;
        Save();
    }
    
    public void SetNotificationPrompt(string prompt) {
        _saveData!.NotificationPrompt = prompt;
        Save();
    }

    public void SetCooldown(long cooldown) {
        _saveData!.Cooldown = cooldown;
        Save();
    }

    public void SetIsSent(bool value) {
        _saveData!.IsSent = value;
        Save();
    }
    
    public void SetLastSentTime() {
        _saveData!.LastSent = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Save();
    }
    
    public void SetLastMessageId(long id) {
        _saveData!.LastSent = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _saveData!.LastMessageId = id;
        Save();
    }
}