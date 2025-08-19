namespace ChatBot.bot.services.ai.AiClients.interfaces;

public abstract class AiClient {
    public abstract Task<string?> GetResponse(string prompt, AiData aiData, EventHandler<string>? callback = null);
}