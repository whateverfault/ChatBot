using System.Text.RegularExpressions;
using ChatBot.bot.interfaces;
using ChatBot.CLI.CliNodes.Directories;
using ChatBot.Services.interfaces;
using ChatBot.Services.Static;
using ChatBot.shared.interfaces;
using ChatBot.shared.Logging;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace ChatBot.Services.message_filter;

public enum FilterStatus {
    Match,
    NotMatch
}

public delegate void MessageHandler(ChatMessage message, FilterStatus status, int patternIndex);

public class MessageFilterService : Service {
    public override string Name => ServiceName.MessageFilter;
    public override MessageFilterOptions Options { get; } = new();
    public event MessageHandler? OnMessageFiltered;


    public void HandleMessage(object? sender, OnMessageReceivedArgs args) {
        var patterns = Options.GetPatterns();
        var message = args.ChatMessage.Message;
        var status = FilterStatus.NotMatch;
        var index = 0;

        for (var i = 0; i < patterns.Count; i++) {
            if (!patterns[i].Regex.Match(message).Success) continue;
            status = FilterStatus.Match;
            index = i;
            break;
        }

        OnMessageFiltered?.Invoke(args.ChatMessage, status, index);
    }

    public void AddPattern(string pattern) {
        Options.AddPattern(new CommentedRegex(new Regex(pattern), false, ""));
    }

    public void AddPatternWithComment(string pattern, bool hasComment, string comment = "") {
        Options.AddPattern(new CommentedRegex(new Regex(pattern), hasComment, comment));
    }
    
    public List<string> GetPatterns() {
        return Options.GetPatterns().Select(pattern => pattern.ToString()).ToList()!;
    }
    
    public List<Content> GetPatternsWithComments() {
        return Options
           .GetPatterns()
           .Select(pattern => new Content(pattern.Regex.ToString(), pattern.HasComment, pattern.Comment))
           .ToList();
    }
    
    public CommentedRegex GetPatternWithComment(int index) {
        return Options.GetPatterns()[index];
    }

    public string GetComment(int index) {
        return Options.GetPatterns()[index].Comment;
    }
    
    public string GetPattern(int index) {
        return Options.GetPatterns()[index].ToString() ?? string.Empty;
    }

    public void RemovePattern(int index) {
        var patterns = Options.GetPatterns();
        if (index > patterns.Count || index < 0) {
            Logger.Log(LogLevel.Error, "Tried to delete not existing pattern");
            return;
        }

        Options.RemovePattern(index);
    }

    public override void Init(Bot bot) {
        if (!Options.TryLoad()) {
            Options.SetDefaults();
        }
    }

    public override State GetServiceState() {
        return Options.GetState();
    }
    
    public override void ToggleService() {
        Options.SetState(Options.ServiceState == State.Enabled ? State.Disabled : State.Enabled);
    }
}