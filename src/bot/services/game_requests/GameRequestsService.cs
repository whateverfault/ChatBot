using System.Text;
using ChatBot.bot.chat_bot;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.game_requests.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;
using TwitchAPI.client.data;
using TwitchAPI.helix;

namespace ChatBot.bot.services.game_requests;

public class GameRequestsService : Service {
    private static ITwitchClient? Client => TwitchChatBot.Instance.GetClient();
    
    public override GameRequestsOptions Options { get; } = new GameRequestsOptions();


    public async void HandleMessage(object? sender, ChatMessage chatMessage) {
        try {
            if (string.IsNullOrEmpty(chatMessage.RewardId) || !Options.GameRequestsRewards.Contains(chatMessage.RewardId)) {
                return;
            }
            if (Options.ServiceState == State.Disabled) {
                await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, Client);
                return;
            }

            var splitted = chatMessage.Text.Split(' ').ToList();
            await AddGameRequest(splitted, chatMessage);
        } catch (Exception e) {
            ErrorHandler.LogMessage(LogLevel.Error, $"Exception while handling a potential game request: {e.Data}");
        }
    }

    public async Task AddGameRequest(List<string> args, ChatMessage chatMessage) {
        try {
            if (Client?.Credentials == null) return;

            var gameNameSb = new StringBuilder();

            foreach (var arg in args) {
                if (arg.Length >= 2 && arg[..2].Contains("--")) {
                    break;
                }

                gameNameSb.Append($"{arg} ");
            }

            var requesterId = chatMessage.UserId;
            var position = -1;
            
            if (chatMessage.Fits(Restriction.DevBroad)) {
                var index = args.IndexOf("--position");
                if (index >= 0 && index < args.Count - 1) {
                    position = int.Parse(args[index + 1]);
                }
                
                index = args.IndexOf("--user");
                if (index >= 0 && index < args.Count - 1) {
                    var userId = await Helix.GetUserId(args[index + 1], Client.Credentials,
                                                       (_, message) => {
                                                           ErrorHandler.LogMessage(LogLevel.Error, message);
                                                       });
                    if (userId == null) {
                        await ErrorHandler.ReplyWithError(ErrorCode.UserNotFound, chatMessage, Client);
                        return;
                    }

                    requesterId = userId;
                }
            }
            
            if (position <= 0 || position > Options.GameRequests!.Count + 1) {
                position = Options.GameRequests!.Count + 1;
            }

            var gameName = gameNameSb.ToString().Trim();

            if (Options.GameRequests.Any(request => string.Equals(request.GameName, gameName,
                                                                  StringComparison.OrdinalIgnoreCase))) {
                await ErrorHandler.ReplyWithError(ErrorCode.AlreadyExists, chatMessage, Client);
                return;
            }

            var gameRequest = new GameRequest(
                                              gameName,
                                              requesterId
                                             );

            Options.AddRequest(gameRequest, position - 1);
            await Client.SendMessage($"Игра {gameName} добавлена в очередь на {position} позицию.", chatMessage.Id);
        }
        catch (Exception e) {
            ErrorHandler.LogMessage(LogLevel.Error, $"Exception while adding a game request: {e.Data}");
        }
    }
    
    public List<GameRequest>? GetGameRequests() {
        return Options.GameRequests;
    }
}