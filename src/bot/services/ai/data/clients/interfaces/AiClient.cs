namespace ChatBot.bot.services.ai.data.clients.interfaces;

public abstract class AiClient {
    public abstract Task<string?> GetResponse(string prompt, AiChatHistory chatHistory, AiData aiData, EventHandler<string>? callback = null);
}