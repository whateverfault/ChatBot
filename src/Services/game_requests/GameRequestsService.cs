using System.Text;
using ChatBot.bot;
using ChatBot.services.game_requests.Data;
using ChatBot.services.interfaces;
using ChatBot.services.logger;
using ChatBot.services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.services.game_requests;

public class GameRequestsService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private static ITwitchClient? Client => TwitchChatBot.Instance.GetClient();
    
    public override string Name => ServiceName.GameRequests;
    public override GameRequestsOptions Options { get; } = new GameRequestsOptions();


    public async void HandleMessage(object? sender, OnMessageReceivedArgs args) {
        try {
            var chatMessage = args.ChatMessage;

            if (string.IsNullOrEmpty(chatMessage.CustomRewardId) || !Options.GameRequestsRewards.Contains(chatMessage.CustomRewardId)) {
                return;
            }
            if (Options.ServiceState == State.Disabled) {
                ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, Client);
                return;
            }

            var splitted = chatMessage.Message.Split(' ').ToList();
            await AddGameRequest(splitted, chatMessage);
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Exception: {e.Message}");
        }
    }

    public Task AddGameRequest(List<string> args, ChatMessage chatMessage) {
        var gameNameSb = new StringBuilder();

        foreach (var arg in args) {
            if (arg.Length >= 2 && arg[0..2].Contains("--")) {
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
        var requester = chatMessage.Username;
        if (index >= 0 && index < args.Count-1) {
            requester = args[index+1];
        }
        
        if (position <= 0 || position > Options.GameRequests!.Count+1) {
            position = Options.GameRequests!.Count+1;
        }
        
        var gameName = gameNameSb.ToString().Trim();
        
        try {
            if (Options.GameRequests.Any(request => string.Equals(request.GameName, gameName, StringComparison.OrdinalIgnoreCase))) {
                ErrorHandler.ReplyWithError(ErrorCode.AlreadyContains, chatMessage, Client);
                return Task.CompletedTask;
            }
            
            var gameRequest = new GameRequest(
                                              gameName,
                                              requester
                                              );
            
            Options.AddRequest(gameRequest, position-1);
            Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Игра {gameName} добавлена в очередь на {position} позицию.");
            return Task.CompletedTask;
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Exception: {e.Message}");
            return Task.CompletedTask;
        }
    }
    
    public List<GameRequest>? GetGameRequests() {
        return Options.GameRequests;
    }
}