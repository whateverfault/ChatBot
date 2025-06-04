using System.Net.Http.Headers;
using System.Text;
using ChatBot.bot;
using ChatBot.Services.logger;
using ChatBot.Services.Static;
using ChatBot.utils;
using Newtonsoft.Json;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using LogLevel = ChatBot.Services.logger.LogLevel;

namespace ChatBot.extensions;

public static class HelixExtension {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    
    public static async Task BanUserHelix(this ITwitchClient client, ChatBotOptions options, string username, string message) {
        try {
            var userId = await TwitchLibUtils.GetUserId(options, username);
            var broadcasterId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            var botId = await TwitchLibUtils.GetUserId(options, options.Username!);
            if (string.IsNullOrEmpty(userId)) {
                _logger.Log(LogLevel.Error, $"User {username} not found");
                return;
            } if (string.IsNullOrEmpty(broadcasterId)) {
                _logger.Log(LogLevel.Error, $"User {options.Channel!} not found");
                return;
            } if (string.IsNullOrEmpty(botId)) {
                _logger.Log(LogLevel.Error, $"User {options.Username!} not found");
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
            var response = await httpClient.PostAsync($"https://api.twitch.tv/helix/moderation/bans?broadcaster_id={broadcasterId}&moderator_id={botId}", content);

            if (response.IsSuccessStatusCode) {
                Console.WriteLine();
                _logger.Log(LogLevel.Error, $"Successfully banned {username}. Message: {message}");
            } else {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Log(LogLevel.Error, $"Failed to ban {username}. Status: {response.StatusCode}. Response: {responseContent}");
            }
        }
        catch (Exception ex) {
            _logger.Log(LogLevel.Error, $"Error banning user: {ex.Message}");
        }
    }
    public static async Task TimeoutUserHelix(this ITwitchClient client, ChatBotOptions options, string username, TimeSpan durationSeconds, string message) {
        try {
            var userId = await TwitchLibUtils.GetUserId(options, username);
            var broadcasterId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            var botId = await TwitchLibUtils.GetUserId(options, options.Username!);
            if (string.IsNullOrEmpty(userId)) {
                Console.WriteLine($"User {username} not found");
                return;
            } if (string.IsNullOrEmpty(broadcasterId)) {
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
                                     RequestUri = new Uri($"https://api.twitch.tv/helix/moderation/bans?broadcaster_id={broadcasterId}&moderator_id={botId}"),
                                     Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"),
                                     Headers =
                                     {
                                         { "Client-ID", options.ClientId },
                                         { "Authorization", $"Bearer {options.OAuth}" }
                                     }
                                 };

            var response = await httpClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode) {
                _logger.Log(LogLevel.Error, $"Successfully timed out {username} for {(int)durationSeconds.TotalSeconds} seconds. Reason: {message}");
            }
            else {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Log(LogLevel.Error, $"Failed to timeout {username}. Status: {response.StatusCode}. Response: {responseContent}");
            }
        }
        catch (Exception ex) {
            _logger.Log(LogLevel.Error, $"Error timing out user: {ex.Message}");
        }
    }
    
    public static async Task DeleteMessageHelix(this ITwitchClient client, ChatBotOptions options, ChatMessage message) {
        try {
            var broadcasterId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            var botId = await TwitchLibUtils.GetUserId(options, options.Username!);
            var userId = await TwitchLibUtils.GetUserId(options, message.Username);
            
            if (string.IsNullOrEmpty(broadcasterId)) {
                _logger.Log(LogLevel.Error, $"Channel {options.Channel!} not found");
                return;
            } if (string.IsNullOrEmpty(botId)) {
                _logger.Log(LogLevel.Error, $"User {options.Username!} not found");
                return;
            } if (string.IsNullOrEmpty(userId)) {
                _logger.Log(LogLevel.Error, $"User {message.Username} not found");
                return;
            }

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.OAuth);
            
            var requestUri = $"https://api.twitch.tv/helix/moderation/chat?broadcaster_id={broadcasterId}&moderator_id={botId}&message_id={message.Id}&user_id={userId}";
            var requestMessage = new HttpRequestMessage
                                 {
                                     Method = HttpMethod.Delete,
                                     RequestUri = new Uri(requestUri),
                                     Headers =
                                     {
                                         { "Client-ID", options.ClientId },
                                         { "Authorization", $"Bearer {options.OAuth}" }
                                     }
                                 };
            var response = await httpClient.SendAsync(requestMessage);
            
            if (response.IsSuccessStatusCode) {
                _logger.Log(LogLevel.Info, $"Successfully deleted message {message.Id ?? $"from user {message.Username}"}");
            } else {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Log(LogLevel.Error, $"Failed to delete message. Status: {response.StatusCode}. Response: {responseContent}");
            }
        }
        catch (Exception ex) {
            _logger.Log(LogLevel.Error, $"Error deleting message: {ex.Message}");
        }
    }

    public static async Task<TimeSpan?> GetFollowageHelix(this ITwitchClient client, ChatBotOptions options, string username) {
        try {
            var userId = await TwitchLibUtils.GetUserId(options, username);
            var broadcasterId = await TwitchLibUtils.GetUserId(options, options.Channel!);
            
            if (string.IsNullOrEmpty(userId)) {
                _logger.Log(LogLevel.Error, $"User {username} not found");
                return null;
            }
            if (string.IsNullOrEmpty(broadcasterId)) {
                _logger.Log(LogLevel.Error, $"Channel {options.Channel!} not found");
                return null;
            }

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Client-ID", options.ClientId);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.OAuth);
            
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.twitch.tv/helix/channels/followers?user_id={userId}&broadcaster_id={broadcasterId}"),
                Headers =
                {
                    { "Client-ID", options.ClientId },
                    { "Authorization", $"Bearer {options.OAuth}" }
                }
            };

            var response = await httpClient.SendAsync(requestMessage);
            
            if (response.IsSuccessStatusCode) {
                var responseContent = await response.Content.ReadAsStringAsync();
                var followData = JsonConvert.DeserializeObject<FollowResponse>(responseContent);
                
                if (followData?.Data.Length > 0) {
                    var followDate = followData.Data[0].FollowedAt;
                    var followDuration = DateTime.UtcNow - followDate;
                    _logger.Log(LogLevel.Info, $"{username} has been following since {followDate} ({followDuration.TotalDays} days)");
                    return followDuration;
                }
                _logger.Log(LogLevel.Info, $"{username} is not following {options.Channel}");
            }
            else {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.Log(LogLevel.Error, $"Failed to get followage for {username}. Status: {response.StatusCode}. Response: {responseContent}");
            }
            return null;
        }
        catch (Exception ex) {
            _logger.Log(LogLevel.Error, $"Error getting followage: {ex.Message}");
            return null;
        } 
    }
}