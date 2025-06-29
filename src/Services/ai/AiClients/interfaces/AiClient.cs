using ChatBot.Services.logger;

namespace ChatBot.Services.ai.AiClients.interfaces;

public abstract class AiClient {
    public abstract Task<string?> GetResponse(string prompt, AiData aiData, LoggerService? logger = null);
}