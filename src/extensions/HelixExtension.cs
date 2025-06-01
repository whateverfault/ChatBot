using System.Net.Http.Headers;
using System.Text;
using ChatBot.bot;
using ChatBot.utils;
using Newtonsoft.Json;
using TwitchLib.Client.Interfaces;

namespace ChatBot.extensions;

public static class HelixExtension {
    public static async Task BanUserHelix(this ITwitchClient client, ChatBotOptions options, string username, string message) {
        try {
            var userId = await TwitchLibUtils.GetUserId(options, username);
            var channelId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            var botId = await TwitchLibUtils.GetUserId(options, options.Username!);
            if (string.IsNullOrEmpty(userId)) {
                Console.WriteLine($"User {username} not found");
                return;
            } if (string.IsNullOrEmpty(channelId)) {
                Console.WriteLine($"User {options.Channel!} not found");
                return;
            } if (string.IsNullOrEmpty(botId)) {
                Console.WriteLine($"User {options.Username!} not found");
                return;
            }


            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.OAuth);
            
            var request = new
                          {
                              data = new
                                     {
                                         user_id = userId,
                                         reason = message
                                     }
                          };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"https://api.twitch.tv/helix/moderation/bans?broadcaster_id={channelId}&moderator_id={botId}", content);

            if (response.IsSuccessStatusCode) {
                Console.WriteLine($"Successfully banned {username}. Message: {message}");
                client.SendMessage(options.Channel, message);
            }
            else {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to ban {username}. Status: {response.StatusCode}. Response: {responseContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error banning user: {ex.Message}");
        }
    }
    public static async Task TimeoutUserHelix(this ITwitchClient client, ChatBotOptions options, string username, TimeSpan durationSeconds, string message) {
        try {
            var userId = await TwitchLibUtils.GetUserId(options, username);
            var channelId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            var botId = await TwitchLibUtils.GetUserId(options, options.Username!);
            if (string.IsNullOrEmpty(userId)) {
                Console.WriteLine($"User {username} not found");
                return;
            } if (string.IsNullOrEmpty(channelId)) {
                Console.WriteLine($"User {options.Channel!} not found");
                return;
            } if (string.IsNullOrEmpty(botId)) {
                Console.WriteLine($"User {options.Username!} not found");
                return;
            } 

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.OAuth);
            
            var request = new
                          {
                              data = new
                                     {
                                         user_id = userId,
                                         duration = (int)durationSeconds.TotalSeconds,
                                         reason = message
                                     }
                          };

            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Post,
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/moderation/bans?broadcaster_id={channelId}&moderator_id={botId}"),
                                     Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"),
                                     Headers =
                                     {
                                         { "Client-ID", options.ClientId },
                                         { "Authorization", $"Bearer {options.OAuth}" }
                                     }
                                 };

            var response = await httpClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode) {
                Console.WriteLine($"Successfully timed out {username} for {(int)durationSeconds.TotalSeconds} seconds. Reason: {message}");
            }
            else {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to timeout {username}. Status: {response.StatusCode}. Response: {responseContent}");
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Error timing out user: {ex.Message}");
        }
    }
}