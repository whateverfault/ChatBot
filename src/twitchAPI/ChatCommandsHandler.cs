using System.Text;
using ChatBot.Services;
using ChatBot.Services.game_requests;
using ChatBot.twitchAPI.interfaces;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace ChatBot.twitchAPI;

public class ChatCommandsHandler : CommandsHandler {
    private const char COMMAND_IDENTIFIER = '~';
    
    private readonly ITwitchClient _client;
    private GameRequestsService _gameRequestsService = null!;

    
    public ChatCommandsHandler(ITwitchClient client) {
        _client = client;
        
        _client.RemoveChatCommandIdentifier('!');
        _client.AddChatCommandIdentifier(COMMAND_IDENTIFIER);
        
        InitServices();
    }

    private void InitServices() {
        _gameRequestsService = (GameRequestsService)ServiceManager.GetService("GameRequestsService");
    }

    public override void Handle(object? sender, OnChatCommandReceivedArgs args) {
        Console.WriteLine($"Command received: {COMMAND_IDENTIFIER}{args.Command.CommandText}");
        var commandArgs = ProcessArgs(args.Command.ArgumentsAsList);
        var errorHandler = new ErrorHandler(_client);
        ErrorCode err;
        
        switch (args.Command.CommandText) {
            case "cmds": {
                SendCmds(args, commandArgs!);
                break;
            }
            case "help": {
                SendUsage(args);
                break;
            }
            #region GameRequests
            case "gr-add": {
                if (commandArgs.Length < 1) {
                    err = ErrorCode.TooFewArgs;
                    errorHandler.ReplyWithError(err, args.Command.ChatMessage);
                    return;
                }
                var gameRequest = new GameRequest(args.Command.ChatMessage.Username, commandArgs[0]!);
                err = _gameRequestsService.AppendRequest(gameRequest, args.Command.ChatMessage);
                if (errorHandler.ReplyWithError(err, args.Command.ChatMessage)) {
                    _client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, "Такой заказ уже есть.");
                    Console.WriteLine($"_gameRequestsService.AppendRequest returned an error: {err}");
                    return;
                }
                Console.WriteLine("Game requests has been appended");
                break;
            }
            case "gr-rem": {
                if (commandArgs.Length < 1) {
                    err = ErrorCode.TooFewArgs;
                    errorHandler.ReplyWithError(err, args.Command.ChatMessage);
                    return;
                }

                if (!int.TryParse(commandArgs[0], out var index)) {
                    err = ErrorCode.WrongInput;
                    errorHandler.ReplyWithError(err, args.Command.ChatMessage);
                    return;
                }
                err = _gameRequestsService.RemoveRequestAt(index-1, args.Command.ChatMessage);
                if (errorHandler.ReplyWithError(err, args.Command.ChatMessage)) {
                    Console.WriteLine($"_gameRequestsService.RemoveRequestAt returned an error: {err}");
                    return;
                }
                
                Console.WriteLine("Game requests has been removed");
                break;
            }
            case "gr-list": {
                var page = 0;
                if (commandArgs.Length > 0) {
                    int.TryParse(commandArgs[0], out page);
                }
                
                var gameReqs = _gameRequestsService.GetGameRequests();
                if (gameReqs.Length == 0) {
                    _client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, "Пусто.");
                    break;
                }
                
                var reqsArr = new List<string>();
                for (var i = 0; i < gameReqs.Length; i++) {
                    reqsArr.Add($"{i+1}. {gameReqs[i].GameName} - {gameReqs[i].Requester}");
                }

                var pages = Page.CalculatePages(reqsArr.ToArray());

                if (page < pages[0]) {
                    page = pages[0];
                } else if (page > pages[^1]) {
                    page = pages[^1];
                }
                
                var message = new StringBuilder();
                for (var i = 0; i < reqsArr.Count; i++) {
                    if (pages[i] == page) {
                        message.Append($"{reqsArr[i]} ");
                    }
                }
                
                message.Append($"|Page {page} of {pages[^1]}| ");
                _client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, message.ToString());
                break;
            }
            #endregion
            default: {
                _client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, $"Неизвестная комманда: {COMMAND_IDENTIFIER}{args.Command.CommandText}");
                break;
            }
        }
    }

    private string?[] ProcessArgs(List<string?> args) 
    {
        return args.Where(arg => !string.IsNullOrEmpty(arg)).ToArray();
    }
    
    private void SendUsage(OnChatCommandReceivedArgs args) {
        var usage = "~<комманда> \"аргумент1\" \"аргумент2\" ...";
        _client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, usage);
    }

    private void SendCmds(OnChatCommandReceivedArgs args, string[] commandArgs) {
        var cmds = new[] {
                             "1. ~cmds - список комманд. ",
                             "2. ~help - использование комманд. ",
                             "3. ~gr-add - добавить заказ игры. ",
                             "4. ~gr-rem - удалить заказ игры. ",
                             "5. ~gr-list - список заказов игр. ",
                         };
        
        var page = 0;
        if (commandArgs.Length > 0) {
            int.TryParse(commandArgs[0], out page);
        }
        
        var pages = Page.CalculatePages(cmds);

        if (page < pages[0]) {
            page = pages[0];
        } else if (page > pages[^1]) {
            page = pages[^1];
        }
        
        var message = new StringBuilder();
        for (var i = 0; i < cmds.Length; i++) {
            if (pages[i] == page) {
                message.Append($"{cmds[i]} ");
            }
        }
        
        message.Append($"|Page {page} of {pages[^1]}| ");
        _client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, message.ToString());
    }
}