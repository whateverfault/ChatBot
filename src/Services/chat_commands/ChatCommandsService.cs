using System.Text;
using ChatBot.Services.game_requests;
using ChatBot.Services.interfaces;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.shared.Logging;
using ChatBot.twitchAPI.interfaces;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace ChatBot.Services.chat_commands;

public class ChatCommandsService : Service {
    private Bot _bot;
    private ITwitchClient Client => _bot.GetClient();

    public override string Name => ServiceName.ChatCommands;
    public override ChatCommandsOptions Options { get; } = new();


    public override void Init(Bot bot) {
        _bot = bot;

        if (!Options.TryLoad()) {
            Options.SetDefaults();
        }

        Options.SetServices(
                            (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests),
                            (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer)
                           );
    }

    public override State GetServiceState() {
        return Options.GetState();
    }
    
    public override void ToggleService() {
        Options.SetState(Options.State == State.Enabled ? State.Disabled : State.Enabled);
    }

    public void ChangeCommandIdentifier(char newId, char oldId) {
        Client.RemoveChatCommandIdentifier(oldId);
        Client.AddChatCommandIdentifier(newId);
    }

    private string?[] ProcessArgs(List<string?> args) {
        return args.Where(arg => !string.IsNullOrEmpty(arg)).ToArray();
    }

    public void HandleMessage(object? sender, OnChatCommandReceivedArgs args) {
        var commandArgs = ProcessArgs(args.Command.ArgumentsAsList);
        var errorHandler = new ErrorHandler(Client);
        ErrorCode err;

        switch (args.Command.CommandText) {
            #region General

            case "cmds": {
                SendCmds(args, commandArgs!);
                break;
            }
            case "help": {
                SendUsage(args);
                break;
            }

            #endregion
            #region GameRequestsService

            case "gr-add": {
                if (commandArgs.Length < 1) {
                    err = ErrorCode.TooFewArgs;
                    errorHandler.ReplyWithError(err, args.Command.ChatMessage);
                    return;
                }
                var gameRequest = new GameRequest(args.Command.ChatMessage.Username, commandArgs[0]!);
                err = Options.GameRequestsService.AppendRequest(gameRequest, args.Command.ChatMessage);
                if (errorHandler.ReplyWithError(err, args.Command.ChatMessage)) {
                    Logger.Log(LogLevel.Error, $"'GameRequestsService.AppendRequest' returned an error: {err}");
                    return;
                }

                Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, "Игра добалвена в очередь.");
                Logger.Log(LogLevel.Info, "Game requests has been appended");
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
                err = Options.GameRequestsService.RemoveRequestAt(index-1, args.Command.ChatMessage);
                if (errorHandler.ReplyWithError(err, args.Command.ChatMessage)) {
                    Logger.Log(LogLevel.Error, $"'RemoveRequestAt' returned an error: {err}");
                    return;
                }

                Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, "Игра удалена из очереди.");
                Logger.Log(LogLevel.Info, "Game requests has been removed");
                break;
            }
            case "gr-list": {
                var page = 0;
                if (commandArgs.Length > 0) {
                    int.TryParse(commandArgs[0], out page);
                }

                err = Options.GameRequestsService.ListGameRequests(out var gameReqs);
                if (errorHandler.ReplyWithError(err, args.Command.ChatMessage)) {
                    Logger.Log(LogLevel.Error, $"'GetGameRequests' returned an error: {err}");
                    return;
                }
                if (gameReqs.Length == 0) {
                    Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, "Пусто.");
                    Logger.Log(LogLevel.Info, $"User '{args.Command.ChatMessage.Username}' tried to access empty game requests list");
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
                Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, message.ToString());
                break;
            }
            case "gr-give-point": {
                if (commandArgs.Length < 1) {
                    err = ErrorCode.TooFewArgs;
                    errorHandler.ReplyWithError(err, args.Command.ChatMessage);
                    return;
                }

                err = Options.GameRequestsService.GivePoint(args.Command.ChatMessage, commandArgs[0]);

                if (errorHandler.ReplyWithError(err, args.Command.ChatMessage)) {
                    Logger.Log(LogLevel.Error, $"'GameRequestsService.GivePoint' returned an error: {err}");
                    return;
                }

                Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, $"Очко выдано пользователю '{commandArgs[0]}'.");
                Logger.Log(LogLevel.Info, $"User '{args.Command.ChatMessage.Username}' has obtained a point");
                break;
            }
            case "gr-take-point": {
                if (commandArgs.Length < 1) {
                    err = ErrorCode.TooFewArgs;
                    errorHandler.ReplyWithError(err, args.Command.ChatMessage);
                    return;
                }

                err = Options.GameRequestsService.TakePoint(args.Command.ChatMessage, commandArgs[0]);
                if (errorHandler.ReplyWithError(err, args.Command.ChatMessage)) {
                    Logger.Log(LogLevel.Error, $"'GetGameRequests' returned an error: {err}");
                    return;
                }

                Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id,
                                 $"Очко забрано у пользователя '{commandArgs[0]}'.");
                Logger.Log(LogLevel.Info, $"User '{args.Command.ChatMessage.Username}' has spent a point");
                break;
            }
            case "gr-disable": {
                err = Options.GameRequestsService.Disable(args.Command.ChatMessage);

                if (errorHandler.ReplyWithError(err, args.Command.ChatMessage)) {
                    Logger.Log(LogLevel.Error, $"'GameRequestsService.Disable' returned an error: {err}");
                    return;
                }

                Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, "Заказы отключены.");
                Logger.Log(LogLevel.Info, "GameRequestsService is now disabled");
                break;
            }
            case "gr-enable": {
                err = Options.GameRequestsService.Enable(args.Command.ChatMessage);

                if (errorHandler.ReplyWithError(err, args.Command.ChatMessage)) {
                    Logger.Log(LogLevel.Error, $"'GameRequestsService.Enable' returned an error: {err}");
                    return;
                }

                Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, "Заказы включены.");
                Logger.Log(LogLevel.Info, "GameRequestsService is now enabled");
                break;
            }

            #endregion
            #region MessageRandomizerService

            case "mr-guess": {
                if (Options.MessageRandomizerService.Options.MessageState == MessageState.Guessed) {
                    Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, "Уже отгадано.");
                    Logger.Log(LogLevel.Error, $"{args.Command.ChatMessage.Username} tried to guess already guessed message");
                    return;
                }

                if (commandArgs.Length < 1) {
                    ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, args.Command.ChatMessage, Client);
                    Logger.Log(LogLevel.Error, $"Too few arguments for '{args.Command.CommandText}' command");
                    return;
                }

                err = Options.MessageRandomizerService.GetLastGeneratedMessage(out var message);
                if (ErrorHandler.ReplyWithError(err, args.Command.ChatMessage, Client)) {
                    Logger.Log(LogLevel.Info, "Tried to access last random message while there is no such.");
                    return;
                }

                if (commandArgs[0] != message!.Username) {
                    Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, "Неправильно.");
                    Logger.Log(LogLevel.Info, $"'{args.Command.ChatMessage.Username}' guessed wrong.");
                } else {
                    Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id,
                                     $"Правильно, это было сообщение от {message.Username}.");
                    Logger.Log(LogLevel.Info, $"'{args.Command.ChatMessage.Username}' guessed right.");
                    Options.MessageRandomizerService.Options.SetMessageState(MessageState.Guessed);
                }
                break;
            }
            case "mr-whose": {
                if (!PermissionHandler.Handle(Permission.Dev, args.Command.ChatMessage)) {
                    ErrorHandler.ReplyWithError(ErrorCode.PermDeny, args.Command.ChatMessage, Client);
                    Logger.Log(LogLevel.Info, $"'{args.Command.ChatMessage.Username}' doesn't have enough rights to call this command.");
                }

                err = Options.MessageRandomizerService.GetLastGeneratedMessage(out var message);
                if (ErrorHandler.ReplyWithError(err, args.Command.ChatMessage, Client)) {
                    ErrorHandler.ReplyWithError(err, args.Command.ChatMessage, Client);
                    Logger.Log(LogLevel.Info, "Tried to access last random message while there are no such.");
                    return;
                }

                Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, $"Это было сообщение от '{message!.Username}'");
                break;
            }
            case "mr-repeat": {
                err = Options.MessageRandomizerService.GetLastGeneratedMessage(out var message);
                if (ErrorHandler.ReplyWithError(err, args.Command.ChatMessage, Client)) {
                    ErrorHandler.ReplyWithError(err, args.Command.ChatMessage, Client);
                    Logger.Log(LogLevel.Info, "Tried to access last random message while there are no such.");
                    return;
                }
                Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, message!.Msg);
                Logger.Log(LogLevel.Info, $"Repeated last message for {args.Command.ChatMessage.Username}.");
                break;
            }

            #endregion
            default: {
                Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id,
                                 $"Неизвестная комманда: {Options.CommandIdentifier}{args.Command.CommandText}");
                break;
            }
        }
    }

    private void SendUsage(OnChatCommandReceivedArgs args) {
        var usage = $"{Options.CommandIdentifier}<комманда> \"аргумент1\" \"аргумент2\" ...";
        Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, usage);
    }

    private void SendCmds(OnChatCommandReceivedArgs args, string[] commandArgs) {
        var cmdId = Options.CommandIdentifier;
        var cmds = new[] {
                             $"1. {cmdId}cmds - список комманд. ",
                             $"2. {cmdId}help - использование комманд. ",
                             $"3. {cmdId}gr-add <game_name> - добавить заказ игры. ",
                             $"4. {cmdId}gr-rem <game_position> - удалить заказ игры. ",
                             $"5. {cmdId}gr-list - список заказов игр. ",
                             $"6. {cmdId}gr-give-point <nickname> - выдать очко.",
                             $"7. {cmdId}gr-take-point <nickname> - забрать очко.",
                             $"8. {cmdId}gr-enable - включить заказы.",
                             $"9. {cmdId}gr-disable - отключить заказы.",
                             $"10. {cmdId}mr-guess <nick_name> - угадать ник написавшего сообщение",
                             $"11. {cmdId}mr-whose - вывести ник написавшего сообщение"
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
        Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, message.ToString());
    }
}