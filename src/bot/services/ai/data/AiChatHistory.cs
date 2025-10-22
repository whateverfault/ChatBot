namespace ChatBot.bot.services.ai.data;

public class AiChatHistory {
    public TimeSpan LastUsed;
    public readonly string Id;
    public readonly List<AiMessage> Messages;


    public AiChatHistory(string id) {
        Id = id;
        Messages = [];
        LastUsed = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    public void AddMessage(string prompt, string response) {
        var message = new AiMessage(prompt, response);
        Messages.Add(message);
        
        LastUsed = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }
}