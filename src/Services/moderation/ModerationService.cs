using ChatBot.bot;
using ChatBot.bot.interfaces;
using ChatBot.services.interfaces;
using ChatBot.services.logger;
using ChatBot.services.message_filter;
using ChatBot.services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TwitchLib.Client.Models;

namespace ChatBot.services.moderation;

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


    public async void HandleMessage(ChatMessage message, FilterStatus status, int patternIndex) {
        try {
            if (Options.ServiceState == State.Disabled) return;
            if (status == FilterStatus.NotMatch) return;

            if (patternIndex < 0 || patternIndex >= Options.ModerationActions.Count) return;
        
            var err = Bot.TryGetClient(out var client);
            if (ErrorHandler.LogError(err)) return;

            var modAction = Options.ModerationActions[patternIndex];

            if (modAction.Type is ModerationActionType.Ban or ModerationActionType.Timeout) {
                await modAction.Activate(client, (ChatBotOptions)Bot.Options, message);
            } else {
                await modAction.ActivateWarn(client, (ChatBotOptions)Bot.Options, message);
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
        
        var err = Bot.TryGetClient(out var client);
        if (ErrorHandler.LogError(err)) return;
        
        var userId = await TwitchLibUtils.GetUserId((ChatBotOptions)Bot.Options, message.Username, _logger);
        
        if (string.IsNullOrEmpty(userId)) {
            _logger.Log(LogLevel.Error, $"Couldn't Warn User {message.Username}");
            return;
        }
        var modAction = Options.ModerationActions[patternIndex];
        var temp = modAction.ModeratorComment;
        if (text != null) {
            modAction.SetComment(text);
        }
        await modAction.ActivateWarn(client, (ChatBotOptions)Bot.Options, message, true);
        modAction.SetComment(temp);
    }
    
    public void AddModAction(ModAction action) {
        Options.AddModAction(action);
    }

    public bool RemoveModAction(int index) {
        return Options.RemoveModAction(index);
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
}