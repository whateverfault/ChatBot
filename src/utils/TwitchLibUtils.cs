using ChatBot.bot;
using TwitchLib.Api;

namespace ChatBot.utils;

public static class TwitchLibUtils {
    public static async Task<string> GetUserId(ChatBotOptions options, string username) {
        // TODO Unharcode clientId
        
        var api = new TwitchAPI {
                                    Settings = {
                                                   AccessToken = options.OAuth,
                                                   ClientId = options.ClientId
                                               }
                                };

        var user = await api.Helix.Users.GetUsersAsync(logins: [username]);
        if (user.Users.Length > 0) {
            var userId = user.Users[0].Id;
            return userId;
        }
        
        Console.WriteLine("User not found.");
        return "-1";
    }
}