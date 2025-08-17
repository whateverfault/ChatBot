using ChatBot.api.client;
using ChatBot.api.client.data;
using ChatBot.api.shared.requests;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.message_filter;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.Handlers;
using ChatBot.bot.shared.interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ChatBot.bot.services.moderation;

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
    private static Bot Bot => TwitchChatBot.Instance;
    
    public override string Name => ServiceName.Moderation;
    public override ModerationOptions Options { get; } = new ModerationOptions();

    public EventHandler<ModAction>? OnModActionAdded;
    public EventHandler<int>? OnModActionRemoved;
    
    
    public async void HandleMessage(ChatMessage message, FilterStatus status, int patternIndex) {
        try {
            if (Options.ServiceState == State.Disabled) return;
            if (status == FilterStatus.NotMatch) return;

            if (patternIndex < 0 || patternIndex >= Options.ModerationActions.Count) return;
        
            var err = Bot.TryGetClient(out var client);
            if (ErrorHandler.LogError(err)) return;

            var modAction = Options.ModerationActions[patternIndex];

            if (modAction.Type is ModerationActionType.Ban or ModerationActionType.Timeout) {
                await modAction.Activate(client, message);
            } else {
                await modAction.ActivateWarn(client, message);
            }
        }
        catch (Exception e) {
            _logger.Log(LogLevel.Error, $"An exception occured while moderating a message. {e}");
        }
    }

    public async Task WarnUser(ChatMessage message, int patternIndex, string? text = null) {
        if (Options.ServiceState == State.Disabled) return;

        if (patternIndex < 0 || patternIndex >= Options.ModerationActions.Count) {
            _logger.Log(LogLevel.Error, $"Couldn't Warn User {message.Username}");
            return;
        }

        var client = Bot.GetClient();
        if (client?.Credentials == null) return;

        var userId = await Requests.GetUserId(message.Username, client.Credentials);
        if (userId == null) return;    
        
        var modAction = Options.ModerationActions[patternIndex];
        var temp = modAction.ModeratorComment;
        if (text != null) {
            modAction.SetComment(text);
        }
        await modAction.ActivateWarn(client, message, true);
        modAction.SetComment(temp);
    }
    
    public void AddModAction(ModAction action) {
        Options.AddModAction(action);
        OnModActionAdded?.Invoke(this, action);
    }

    public bool RemoveModAction(int index) {
        var result = Options.RemoveModAction(index);
        if (result) OnModActionRemoved?.Invoke(this, index);
        return result;
    }

    public List<ModAction> GetModActions() {
        return Options.ModerationActions;
    }
}