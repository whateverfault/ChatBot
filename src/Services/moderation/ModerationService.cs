using System.Text.Json.Serialization;
using ChatBot.bot;
using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;
using ChatBot.Services.message_filter;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using Newtonsoft.Json.Converters;
using TwitchLib.Client.Models;

namespace ChatBot.Services.moderation;

[JsonConverter(typeof(StringEnumConverter))]
public enum ModerationActionType {
    Ban = 0,
    Timeout = 1,
    Warn = 2,
    WarnWithTimeout = 3,
    WarnWithBan = 4,
}

public class ModerationService : Service {
    private Bot _bot = null!;
    
    public override string Name => ServiceName.Moderation;
    public override ModerationOptions Options { get; } = new();


    public async void HandleMessage(ChatMessage message, FilterStatus status, int patternIndex) {
        if (Options.ServiceState == State.Disabled) return;
        if (status == FilterStatus.NotMatch) return;

        if (patternIndex < 0 || patternIndex >= Options.ModerationActions.Count) return;
        
        var err = _bot.TryGetClient(out var client);
        if (err != ErrorCode.None) return;

        var modAction = Options.ModerationActions[patternIndex];

        if (modAction.Type is ModerationActionType.Ban or ModerationActionType.Timeout) {
            await modAction.Activate(client, (ChatBotOptions)_bot.Options, message);
        } else {
            await modAction.ActivateWarn(client, (ChatBotOptions)_bot.Options, message, Options.WarnedUsers);
        }
    }

    public void AddModAction(ModAction action) {
        Options.AddModAction(action);
    }

    public void RemoveModAction(int index) {
        Options.RemoveModAction(index);
    }

    public List<ModAction> GetModActions() {
        return Options.ModerationActions;
    }
    
    public override void Init(Bot bot) {
        if (!Options.TryLoad()) {
            Options.SetDefaults();
        }

        _bot = bot;
    }
}