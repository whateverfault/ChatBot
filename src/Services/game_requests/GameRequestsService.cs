using System.Text;
using ChatBot.bot.interfaces;
using ChatBot.Services.game_requests.Data;
using ChatBot.Services.interfaces;
using ChatBot.Services.logger;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils.HowLongToBeat;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.Services.game_requests;

public class GameRequestsService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private bot.ChatBot _bot = null!;
    private ITwitchClient? Client => _bot.GetClient();
    
    public override string Name => ServiceName.GameRequests;
    public override GameRequestsOptions Options { get; } = new();


    public async void HandleMessage(object? sender, OnMessageReceivedArgs args) {
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
    }

    public async Task AddGameRequest(List<string> args, ChatMessage chatMessage) {
        var gameNameSb = new StringBuilder();
        var yearFilter = Options.GameRequestsFilter.RangeYear;

        foreach (var word in args) {
            if (word == "--year") break;
            gameNameSb.Append($"{word} ");
        }

        var index = args.IndexOf("--year");
        if (index >= 0 && index < args.Count-1) {
            var year = args[index+1];
            yearFilter.Min = year;
            yearFilter.Max = year;
        }
        
        var gameName = gameNameSb.ToString().Trim();
        var gameFilter = new GameFilter(
                                        gameName,
                                        Options.GameRequestsFilter.Platforms,
                                        Options.GameRequestsFilter.RangeTime,
                                        yearFilter
                                        );
        
        try {
            var gameInfoResponse = await HltbUtils.FetchGamesByName(gameFilter, _logger);
            if (gameInfoResponse == null) {
                ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, Client);
                return;
            } if (gameInfoResponse.Count <= 0) {
                ErrorHandler.ReplyWithError(ErrorCode.NothingFound, chatMessage, Client);
                return;
            }
            var gameInfo = gameInfoResponse.Games[0];

            if (Options.GameRequests.Any(request => request.GameId == gameInfo.GameId)) {
                ErrorHandler.ReplyWithError(ErrorCode.AlreadyContains, chatMessage, Client);
                return;
            }
            
            var gameRequest = new GameRequest(
                                              gameInfo.GameId,
                                              gameInfo.GameName,
                                              (int)MathF.Round((float)gameInfo.CompMain/3600),
                                              gameInfo.ReleaseYear,
                                              chatMessage.Username
                                              );
            Options.AddRequest(gameRequest);
            Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Игра {gameInfo.GameName} добавлена в очередь на позицию {Options.GameRequests.Count}.");
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Exception: {e.Message}");
        }
    }
    
    public List<GameRequest> GetGameRequests() {
        return Options.GameRequests;
    }

    public string GetFilterPlatform() {
        return Options.GameRequestsFilter.Platforms;
    }

    public int GetFilterMinTime() {
        return Options.GameRequestsFilter.RangeTime.Min ?? -1;
    }
    
    public int GetFilterMaxTime() {
        return Options.GameRequestsFilter.RangeTime.Max ?? -1;
    }
    
    public string GetFilterMinYear() {
        return Options.GameRequestsFilter.RangeYear.Min;
    }
    
    public string GetFilterMaxYear() {
        return Options.GameRequestsFilter.RangeYear.Max;
    }
    
    public override void Init(Bot bot) {
        _bot = (bot.ChatBot)bot;
        base.Init(bot);
    }
}