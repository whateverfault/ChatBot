using System.Text;
using ChatBot.bot.chat_bot;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.game_requests.Data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;
using TwitchAPI.client.data;
using TwitchAPI.helix;

namespace ChatBot.bot.services.game_requests;

public class GameRequestsService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private static ITwitchClient? Client => TwitchChatBot.Instance.GetClient();
    
    public override string Name => ServiceName.GameRequests;
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
            _logger.Log(LogLevel.Error, $"Exception: {e}");
        }
    }

    public async Task AddGameRequest(List<string> args, ChatMessage chatMessage) {
        if (Client?.Credentials == null) return;
        
        var gameNameSb = new StringBuilder();

        foreach (var arg in args) {
            if (arg.Length >= 2 && arg[..2].Contains("--")) {
                break;
            }

            gameNameSb.Append($"{arg} ");
        }
        
        var index = args.IndexOf("--position");
        var position = -1;
        if (index >= 0 && index < args.Count-1) {
            position = int.Parse(args[index+1]);
        }
        
        index = args.IndexOf("--user");
        var requesterId = chatMessage.UserId;
        if (index >= 0 && index < args.Count-1) {
            var userId = await Helix.GetUserId(args[index+1], Client.Credentials, (_, message) => {
                                                   _logger.Log(LogLevel.Error, message);
                                               });
            if (userId == null) {
                await ErrorHandler.ReplyWithError(ErrorCode.UserNotFound, chatMessage, Client);
                return;
            }

            requesterId = userId;
        }
        
        if (position <= 0 || position > Options.GameRequests!.Count+1) {
            position = Options.GameRequests!.Count+1;
        }
        
        var gameName = gameNameSb.ToString().Trim();
        
        try {
            if (Options.GameRequests.Any(request => string.Equals(request.GameName, gameName, StringComparison.OrdinalIgnoreCase))) {
                await ErrorHandler.ReplyWithError(ErrorCode.AlreadyContains, chatMessage, Client);
                return;
            }
            
            var gameRequest = new GameRequest(
                                              gameName,
                                              requesterId
                                              );
            
            Options.AddRequest(gameRequest, position-1);
            await Client.SendMessage($"Игра {gameName} добавлена в очередь на {position} позицию.", chatMessage.Id);
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Exception: {e}");
        }
    }
    
    public List<GameRequest>? GetGameRequests() {
        return Options.GameRequests;
    }
}