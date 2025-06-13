using ChatBot.Services.interfaces;
using ChatBot.Services.message_filter;
using ChatBot.Services.Static;
using ChatBot.shared;
using ChatBot.shared.interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.Services.chat_logs;

public delegate void LogsAppendedHandler(ChatMessage message);

public class ChatLogsService : Service {
    public override string Name => ServiceName.ChatLogs;
    public override ChatLogsOptions Options { get; } = new();
    public event LogsAppendedHandler? OnLogsAppended;


    public void HandleMessage(ChatMessage message, FilterStatus status, int patternIndex) {
        if (Options.ServiceState == State.Disabled) return;
        if (status == FilterStatus.Match) return;
        //TODO unhardcode this feature
        if (Constants.excludeUsersIds.Contains(message.UserId)) return;
        
        var msg = new Message(message.Message, message.Username);
        Options.AddLog(msg);
        OnLogsAppended?.Invoke(message);
    }
    
    public List<Message> GetLogs() {
        return Options.Logs;
    }

    public void AddLog(Message message) {
        Options.Logs.Add(message);
    }
}