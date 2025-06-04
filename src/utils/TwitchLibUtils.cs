using ChatBot.bot;
using ChatBot.Services.logger;
using TwitchLib.Api;

namespace ChatBot.utils;

public static class TwitchLibUtils {
    public static async Task<string> GetUserId(ChatBotOptions options, string username, LoggerService? logger = null) {
        var api = new TwitchAPI {
                                    Settings = {
                                                   AccessToken = options.OAuth,
                                                   ClientId = options.ClientId
                                               }
                                };

        var user = await api.Helix.Users.GetUsersAsync(logins: [username]);
        if (user.Users.Length > 0) {
            var userId = user.Users[0].Id;
            logger?.Log(LogLevel.Error, $"User {username} successfully found under id: {userId}");
            return userId;
        }

        logger?.Log(LogLevel.Error, $"Could not find user {username}");
        return "-1";
    }
}