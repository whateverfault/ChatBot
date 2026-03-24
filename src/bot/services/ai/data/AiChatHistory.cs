using ChatBot.bot.services.Static;

namespace ChatBot.bot.services.ai.data;

public class AiChatHistory {
    private static readonly AiService _ai = (AiService)Services.Get(ServiceId.Ai);
    private readonly object _sync = new object();
    
    public TimeSpan LastUsed;
    public readonly string Id;
    public readonly List<AiMessage> Messages;
    
    
    public AiChatHistory(string id) {
        Id = id;
        Messages = [];
        LastUsed = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }
    
    public void AddMessage(string prompt, string response) {
        lock (_sync) {
            var message = new AiMessage(prompt, response);
            Messages.Add(message);

            LastUsed = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            _ai.Options.Save();
        }
    }
    
    public void AddMessages(params AiMessage[] messages) {
        lock (_sync) {
            foreach (var msg in messages) {
                var message = new AiMessage(msg.UserPrompt, msg.AiResponse);
                Messages.Add(message);
            }

            LastUsed = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            _ai.Options.Save();
        }
    }
}