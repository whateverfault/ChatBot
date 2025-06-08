using System.Text.Json.Serialization;
using ChatBot.bot;
using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;
using ChatBot.Services.logger;
using ChatBot.Services.message_filter;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;
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
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private Bot _bot = null!;
    
    public override string Name => ServiceName.Moderation;
    public override ModerationOptions Options { get; } = new();


    public async void HandleMessage(ChatMessage message, FilterStatus status, int patternIndex) {
        if (Options.ServiceState == State.Disabled) return;
        if (status == FilterStatus.NotMatch) return;

        if (patternIndex < 0 || patternIndex >= Options.ModerationActions.Count) return;
        
        var err = _bot.TryGetClient(out var client);
        if (ErrorHandler.LogError(err)) return;

        var modAction = Options.ModerationActions[patternIndex];

        if (modAction.Type is ModerationActionType.Ban or ModerationActionType.Timeout) {
            await modAction.Activate(client, (ChatBotOptions)_bot.Options, message);
        } else {
            await modAction.ActivateWarn(client, (ChatBotOptions)_bot.Options, message, Options.WarnedUsers);
        }
    }

    public async Task WarnUser(ChatMessage message, int patternIndex) {
        if (Options.ServiceState == State.Disabled) return;

        if (patternIndex < 0 || patternIndex >= Options.ModerationActions.Count) {
            _logger.Log(LogLevel.Error, $"Couldn't Warn User {message.Username}");
            return;
        }
        
        var err = _bot.TryGetClient(out var client);
        if (ErrorHandler.LogError(err)) return;
        
        var userId = await TwitchLibUtils.GetUserId((ChatBotOptions)_bot.Options, message.Username, _logger);
        
        if (string.IsNullOrEmpty(userId)) {
            _logger.Log(LogLevel.Error, $"Couldn't Warn User {message.Username}");
            return;
        }
        var modAction = Options.ModerationActions[patternIndex];
        await modAction.ActivateWarn(client, (ChatBotOptions)_bot.Options, message, Options.WarnedUsers, true);
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
    
    public bool GetModAction(int index, out ModAction? modAction) {
        modAction = null;
        if (index < 0 || index >= Options.ModerationActions.Count) {
            return false;
        }

        modAction = Options.ModerationActions[index];
        return true;
    }
    
    public override void Init(Bot bot) {
        if (!Options.TryLoad()) {
            Options.SetDefaults();
        }

        _bot = bot;
    }
}