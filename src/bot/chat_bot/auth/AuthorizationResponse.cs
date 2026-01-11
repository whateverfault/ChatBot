namespace ChatBot.bot.chat_bot;

public class AuthorizationResponse {
    public string? Broadcaster { get; }
    public string? Bot { get; }


    public AuthorizationResponse(
        string? broadcaster = null,
        string? bot = null) {
        Broadcaster = broadcaster;
        Bot = bot;
    }
}