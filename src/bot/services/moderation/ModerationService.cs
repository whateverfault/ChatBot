using ChatBot.bot.chat_bot;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.message_filter;
using ChatBot.bot.services.moderation.data;
using ChatBot.bot.shared.handlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TwitchAPI.client;
using TwitchAPI.client.data;

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
    private static Bot Bot => TwitchChatBot.Instance;
    
    public override ModerationOptions Options { get; } = new ModerationOptions();

    public event EventHandler<ModAction>? OnModActionAdded;
    public event EventHandler<int>? OnModActionRemoved;
    
    
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
            ErrorHandler.LogMessage(LogLevel.Error, $"An exception occured while moderating a message. {e.Data}");
        }
    }

    public async Task WarnUser(ChatMessage message, int patternIndex, string? text = null) {
        if (Options.ServiceState == State.Disabled) return;

        if (patternIndex < 0 || patternIndex >= Options.ModerationActions.Count) {
            ErrorHandler.LogMessage(LogLevel.Error, $"Couldn't Warn User {message.UserName}");
            return;
        }

        var client = Bot.GetClient();
        if (client?.Credentials == null) return;

        var userId = await Bot.Api.GetUserId(message.UserName, client.Credentials);
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