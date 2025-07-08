using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.telegram;

public class TgNotificationsOptions : Options {
    private SaveData? _saveData;
    
    protected override string Name => "tg_notifications";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");

    public override State ServiceState => _saveData!.ServiceState;
    
    public string BotToken => _saveData!.BotToken;
    public long ChatId => _saveData!.ChatId;

    public string NotificationPrompt => _saveData!.NotificationPrompt;

    public long Cooldown => _saveData!.Cooldown;
    public long LastStreamed => _saveData!.LastStreamed;
    public int? LastMessageId => _saveData!.LastMessageId;

    public bool WasStreaming => _saveData!.WasStreaming;


    public override bool TryLoad() {
        return JsonUtils.TryRead(OptionsPath, out _saveData);
    }

    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.SaveIssue);
            SetDefaults();
        }
    }

    public override void Save() {
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
    }

    public override void SetDefaults() {
        _saveData = new() {
                              NotificationPrompt = "Стрим начался! {title}\n{link}"
                          };
        Save();
    }

    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }

    public override State GetState() {
        return ServiceState;
    }

    public void SetBotToken(string token) {
        _saveData!.BotToken = token;
        Save();
    }
    
    public void SetChatId(long chatId) {
        _saveData!.ChatId = chatId;
        Save();
    }

    public void SetLastStreamedTime(long time) {
        _saveData!.LastStreamed = time;
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

    public void SetLastMessageId(int? id) {
        _saveData!.LastMessageId = id;
        Save();
    }
    
    public void SetWasStreaming(bool wasStreaming) {
        _saveData!.WasStreaming = wasStreaming;
        Save();
    }
}