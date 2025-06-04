using System.Text;
using ChatBot.bot;
using ChatBot.bot.interfaces;
using ChatBot.extensions;
using ChatBot.Services.interfaces;
using ChatBot.Services.logger;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.moderation;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace ChatBot.Services.chat_commands;

public class ChatCommandsService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    private Bot _bot = null!;
    private ITwitchClient Client => _bot.GetClient();

    public override string Name => ServiceName.ChatCommands;
    public override ChatCommandsOptions Options { get; } = new();


    public override void Init(Bot bot) {
        _bot = bot;

        if (!Options.TryLoad()) {
            Options.SetDefaults();
        }

        Options.SetServices(
                            (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer),
                            (ModerationService)ServiceManager.GetService(ServiceName.Moderation)
                           );
    }

    public override State GetServiceState() {
        return Options.GetState();
    }
    
    public override void ToggleService() {
        Options.SetState(Options.ServiceState == State.Enabled ? State.Disabled : State.Enabled);
    }

    public void SetCommandIdentifier(char identifier) {
        var err = _bot.TryGetClient(out _);
        if (ErrorHandler.LogErrorAndPrint(err)) {
            return;
        }
        
        Options.SetCommandIdentifier(identifier);
    }
    
    public void ChangeCommandIdentifier(char newId, char oldId) {
        Client.RemoveChatCommandIdentifier(oldId);
        Client.AddChatCommandIdentifier(newId);
    }

    private string?[] ProcessArgs(List<string?> args) {
        return args.Where(arg => !string.IsNullOrEmpty(arg)).ToArray();
    }

    public int GetRequiredRoleAsInt() {
        return (int)Options.GetRequiredRole();
    }
    
    public void RequiredRoleNext() {
        Options.SetRequiredRole((Restriction)(((int)Options.RequiredRole+1)%Enum.GetValues(typeof(Restriction)).Length));
    }

    public int GetModActionIndex() {
        return Options.GetModActionIndex();
    }

    public void SetModActionIndex(int index) {
        var modAction = Options.ModerationService.GetModActions();
        if (index < 0 || index >= modAction.Count) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.InvalidInput);
            return;
        }
        Options.SetModActionIndex(index);
    }
    
    public async void HandleMessage(object? sender, OnChatCommandReceivedArgs args) {
        try {
            var commandArgs = ProcessArgs(args.Command.ArgumentsAsList);
            var errorHandler = new ErrorHandler(Client);
            var chatMessage = args.Command.ChatMessage;
            ErrorCode err;

            switch (args.Command.CommandText) {
                #region General

                case "cmds": {
                    if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                        SendClippedCmds(args, commandArgs);
                        return;
                    }
                
                    SendFullCmds(args, commandArgs!);
                    break;
                }
                case "help": {
                    SendUsage(args);
                    break;
                }
                case "when": {
                    if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                        await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                        return;
                    }
                
                    if (commandArgs.Length < 1) {
                        err = ErrorCode.TooFewArgs;
                        errorHandler.ReplyWithError(err, args.Command.ChatMessage);
                        return;
                    }

                    var message = args.Command.ChatMessage;
                    var argSb = new StringBuilder();
                
                    foreach (var arg in commandArgs) {
                        argSb.Append($"{arg} ");
                    }

                    var random = Random.Shared.Next(0, 2);
                    var randomizedMessage = 
                        random == 0 ? 
                            $"{argSb} уже завтра! PewPewPew PewPewPew PewPewPew" : 
                            $"{argSb} никогда GAGAGA GAGAGA GAGAGA";
                    Client.SendReply(message.Channel, message.Id, randomizedMessage);
                    break;
                }
                case "ban": {
                    if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                        await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                        return;
                    }
                
                    if (commandArgs.Length < 1) {
                        err = ErrorCode.TooFewArgs;
                        errorHandler.ReplyWithError(err, args.Command.ChatMessage);
                        return;
                    }

                    var message = args.Command.ChatMessage;
                    var argSb = new StringBuilder();
                
                    foreach (var arg in commandArgs) {
                        argSb.Append($"{arg} ");
                    }
                
                    Client.SendReply(message.Channel, message.Id, $"{argSb} отправлен в бан sillyJAIL sillyJAIL sillyJAIL");
                    break;
                }
                case "echo": {
                    if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                        await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                        return;
                    }
                
                    var argSb = new StringBuilder();
                
                    foreach (var arg in commandArgs) {
                        argSb.Append($"{arg} ");
                    }

                    var message =
                        commandArgs.Length == 0 ?
                            "Я GANDON" :
                            argSb.ToString();
                
                    Client.SendMessage(chatMessage.Channel, message);
                    break;
                }
                case "rizz": {
                    if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                        await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                        return;
                    }
                    var argSb = new StringBuilder();

                    foreach (var arg in commandArgs) {
                        argSb.Append($"{arg} ");
                    }
                
                    var message =
                        commandArgs.Length == 0 ?
                            "КШЯЯ" :
                            argSb.ToString();
                
                    Client.SendMessage(chatMessage.Channel, $"{message} RIZZ RIZZ RIZZ");
                    break;
                }
                case "followage": {
                    string message;
                    var username = chatMessage.Username;
                    if (commandArgs.Length > 0) {
                        username = commandArgs[0];
                    }
                
                    var followage = await Client.GetFollowageHelix((ChatBotOptions)_bot.Options, username!);
                    if (followage == null) {
                        message = 
                            username == chatMessage.Channel ?
                                $"{username} это владелец канала" :
                                $"{username} не фолловнут на {chatMessage.Channel}";
                        Client.SendReply(chatMessage.Channel, chatMessage.Id, message);
                        return;
                    }
                    var years =
                        followage.Value.Days/365 == 0 ?
                            "" :
                            $"{followage.Value.Days/365} {Declensioner.Years(followage.Value.Days/365)}";
                    var months =
                        followage.Value.Days%365/30 == 0 ?
                            "" :
                            $"{followage.Value.Days%365/30} {Declensioner.Months(followage.Value.Days%365/30)}";
                    var days =
                        followage.Value.Days%365%30 == 0 ?
                            "" :
                            $"{followage.Value.Days%365%30} {Declensioner.Days(followage.Value.Days%365%30)}";
                    message = 
                        commandArgs.Length > 0 ? 
                            $"{username} фолловнут на {chatMessage.Channel} {years} {months} {days}" :
                            $"Вы фолловнуты на {chatMessage.Channel} {years} {months} {days}";
                    Client.SendReply(chatMessage.Channel, chatMessage.Id, message);
                    break;
                }
                #endregion
                #region MessageRandomizerService

                case "guess": {
                    if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                        await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                        return;
                    }
                
                    if (Options.MessageRandomizerService.Options.MessageState == MessageState.Guessed) {
                        Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, "Уже отгадано.");
                        _logger.Log(LogLevel.Error, $"{args.Command.ChatMessage.Username} tried to guess already guessed message");
                        return;
                    }

                    if (commandArgs.Length < 1) {
                        ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, args.Command.ChatMessage, Client);
                        _logger.Log(LogLevel.Error, $"Too few arguments for '{args.Command.CommandText}' command");
                        return;
                    }

                    err = Options.MessageRandomizerService.GetLastGeneratedMessage(out var message);
                    if (ErrorHandler.ReplyWithError(err, args.Command.ChatMessage, Client)) {
                        _logger.Log(LogLevel.Info, "Tried to access last random message while there is no such.");
                        return;
                    }

                    if (commandArgs[0] != message!.Username) {
                        Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, "Неправильно.");
                        _logger.Log(LogLevel.Info, $"'{args.Command.ChatMessage.Username}' guessed wrong.");
                    } else {
                        Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id,
                                         $"Правильно, это было сообщение от {message.Username}.");
                        _logger.Log(LogLevel.Info, $"'{args.Command.ChatMessage.Username}' guessed right.");
                        Options.MessageRandomizerService.Options.SetMessageState(MessageState.Guessed);
                    }
                    break;
                }
                case "whose": {
                    if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                        await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                        return;
                    }

                    err = Options.MessageRandomizerService.GetLastGeneratedMessage(out var message);
                    if (ErrorHandler.ReplyWithError(err, args.Command.ChatMessage, Client)) {
                        ErrorHandler.ReplyWithError(err, args.Command.ChatMessage, Client);
                        _logger.Log(LogLevel.Info, "Tried to access last random message while there are no such.");
                        return;
                    }

                    Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, $"Это было сообщение от '{message!.Username}'");
                    break;
                }
                case "repeat": {
                    if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                        await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                        return;
                    }
                
                    err = Options.MessageRandomizerService.GetLastGeneratedMessage(out var message);
                    if (ErrorHandler.ReplyWithError(err, args.Command.ChatMessage, Client)) {
                        ErrorHandler.ReplyWithError(err, args.Command.ChatMessage, Client);
                        _logger.Log(LogLevel.Info, "Tried to access last random message while there are no such.");
                        return;
                    }
                    Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, message!.Msg);
                    _logger.Log(LogLevel.Info, $"Repeated last message for {args.Command.ChatMessage.Username}.");
                    break;
                }
                case "carrot": {
                    if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                        return;
                    }
                
                    Options.MessageRandomizerService.GenerateAndSendRandomMessage(Client, chatMessage.Channel);
                    break;
                }
                #endregion
                default: {
                    Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id,
                                     $"Неизвестная комманда: {Options.CommandIdentifier}{args.Command.CommandText}");
                    break;
                }
            }
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Error while handling the command: {e.Message}");
        }
    }

    private void SendUsage(OnChatCommandReceivedArgs args) {
        var usage = $"{Options.CommandIdentifier}<комманда> \"аргумент1\" \"аргумент2\" ... | {Options.CommandIdentifier}cmds для списка комманд";
        Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, usage);
    }

    private void SendClippedCmds(OnChatCommandReceivedArgs args, string?[] commandArgs) {
        var cmdId = Options.CommandIdentifier;
        var cmds = new[] {
                             $"1. {cmdId}cmds - список комманд.",
                             $"2. {cmdId}help - использование комманд.",
                             $"3. {cmdId}followage [username] - время, которое пользователь отслеживает канал",
                         };
        
        SendPagedReply(cmds, args, commandArgs);
    }
    
    private void SendFullCmds(OnChatCommandReceivedArgs args, string[] commandArgs) {
        var cmdId = Options.CommandIdentifier;
        var cmds = new[] {
                             $"1. {cmdId}cmds - список комманд. ",
                             $"2. {cmdId}help - использование комманд. ",
                             $"3. {cmdId}echo [message] - эхо",
                             $"4. {cmdId}rizz [message] - RIZZ",
                             $"5. {cmdId}followage [username] - время, которое пользователь отслеживает канал",
                             Page.PageTerminator,
                             $"1. {cmdId}guess <nick_name> - угадать ник написавшего",
                             $"2. {cmdId}whose - вывести ник написавшего",
                             $"3. {cmdId}carrot - сгенерировать новое сообщение",
                             $"4. {cmdId}repeat - повторить сообщение",
                         };

        SendPagedReply(cmds, args, commandArgs);
    }

    private void SendPagedReply(string[] reply, OnChatCommandReceivedArgs args, string?[] commandArgs) {
        var page = 0;
        if (commandArgs.Length > 0) {
            int.TryParse(commandArgs[0], out page);
        }

        var pages = Page.CalculatePages(reply);

        if (page < pages[0]) {
            page = pages[0];
        } else if (page > pages[^1]) {
            page = pages[^1];
        }

        var message = new StringBuilder();
        var pageTerminatorsCount = 0;
        for (var i = 0; i < reply.Length; i++) {
            if (reply[i] == Page.PageTerminator) {
                pageTerminatorsCount++;
                continue;
            }
            if (pages[i-pageTerminatorsCount] == page) {
                message.Append($"{reply[i]} ");
            }
        }

        message.Append($"|Page {page} of {pages[^1]}| ");
        Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, message.ToString());
    }
}