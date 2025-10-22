namespace ChatBot.bot.services.ai.data;

public class AiMessage {
    public readonly string UserPrompt;
    public readonly string AiResponse;


    public AiMessage(string prompt, string response) {
        UserPrompt = prompt;
        AiResponse = response;
    }
}