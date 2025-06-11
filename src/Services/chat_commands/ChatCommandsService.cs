using System.Text;
using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;
using ChatBot.Services.level_requests;
using ChatBot.Services.logger;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.moderation;
using ChatBot.Services.Static;
using ChatBot.Services.text_generator;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;
using ChatBot.utils.Helix;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace ChatBot.Services.chat_commands;

public class ChatCommandsService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    private bot.ChatBot _bot = null!;
    private long _time;
    private ITwitchClient Client => _bot.GetClient();

    public override string Name => ServiceName.ChatCommands;
    public override ChatCommandsOptions Options { get; } = new();


    public override void Init(Bot bot) {
        _bot = (bot.ChatBot)bot;

        if (!Options.TryLoad()) {
            Options.SetDefaults();
        }

        Options.SetServices(
                            (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer),
                            (ModerationService)ServiceManager.GetService(ServiceName.Moderation),
                            (TextGeneratorService)ServiceManager.GetService(ServiceName.TextGenerator),
                            (LevelRequestsService)ServiceManager.GetService(ServiceName.LevelRequests)
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

    private List<string> ProcessArgs(List<string?> args) {
        var processedArgs = new List<string>();
        foreach (var arg in args) {
            if (string.IsNullOrEmpty(arg)) continue;
            if (arg.Length is 1 or 2 && !char.IsLetterOrDigit(arg[0])) continue;
            processedArgs.Add(arg);
        }
        return processedArgs;
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

    public bool SetModActionIndex(int index) {
        var modAction = Options.ModerationService.GetModActions();
        if (index < 0 || index >= modAction.Count) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.InvalidInput);
            return false;
        }
        Options.SetModActionIndex(index);
        return true;
    }

    public int GetCooldown() {
        return Options.Cooldown;
    }

    public void SetCooldown(int cooldown) {
        Options.SetCooldown(cooldown);
    }
    
    public async void HandleCmd(object? sender, OnChatCommandReceivedArgs args) {
        try {
            var commandArgs = ProcessArgs(args.Command.ArgumentsAsList);
            var errorHandler = new ErrorHandler(Client);
            var chatMessage = args.Command.ChatMessage;
            ErrorCode err;

            var curTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (curTime-_time < Options.Cooldown) {
                return;
            }
            _time = curTime;

            switch (args.Command.CommandText) {
                #region General

                case "cmds": {
                    if (!RestrictionHandler.Handle(Restriction.Vip, chatMessage)) {
                        SendEveryonesCmds(args, commandArgs);
                        return;
                    } if (!RestrictionHandler.Handle(Restriction.DevMod, chatMessage)) {
                        SendVipCmds(args, commandArgs);
                        return;
                    }
                
                    SendDevModCmds(args, commandArgs);
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
                
                    if (commandArgs.Count < 1) {
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
                
                    if (commandArgs.Count < 1) {
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
                        commandArgs.Count == 0 ?
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
                        commandArgs.Count == 0 ?
                            "КШЯЯ" :
                            argSb.ToString();
                
                    Client.SendMessage(chatMessage.Channel, $"{message} RIZZ RIZZ RIZZ");
                    break;
                }
                case "followage": {
                    string message;
                    var username = chatMessage.Username;
                    if (commandArgs.Count > 0) {
                        username = commandArgs[0];
                    }
                
                    var followage = await HelixUtils.GetFollowageHelix(_bot.Options, username!);
                    if (followage == null) {
                        if (commandArgs.Count > 0) {
                            message = 
                                username == chatMessage.Channel ?
                                    $"{username} это владелец канала RIZZ" :
                                    $"{username} не фолловнут на {chatMessage.Channel} Sadding";
                        } else {
                            message = 
                                username == chatMessage.Channel ?
                                    "Вы владелец канала RIZZ" :
                                    $"Вы не фолловнуты на {chatMessage.Channel} Sadding";
                        }

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
                        commandArgs.Count > 0 ? 
                            $"{username} фолловнут на {chatMessage.Channel} {years} {months} {days}" :
                            $"Вы фолловнуты на {chatMessage.Channel} {years} {months} {days}";
                    Client.SendReply(chatMessage.Channel, chatMessage.Id, message);
                    break;
                }
                case "potato": {
                    if (Options.TextGeneratorService.GetServiceState() == State.Disabled) {
                        ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, Client);
                        return;
                    }
                    
                    Options.TextGeneratorService.GenerateAndSend();
                    break;
                }

                case "clip": {
                    if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                        await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                        return;
                    }
                    
                    var clipId = await HelixUtils.CreateClipHelix(_bot.Options);

                    if (clipId == null) {
                        ErrorHandler.ReplyWithError(ErrorCode.ClipCreationFailed, chatMessage, Client);
                        return;
                    }
                    Client.SendReply(_bot.Options.Channel!, chatMessage.Id, $"Клип создан - https://www.twitch.tv/{_bot.Options.Channel}/clip/{clipId}");
                    break;
                }
                #endregion
                #region Moderation

                case "req": {
                    var reqsStateStr = Options.LevelRequestsService.GetServiceState() == State.Enabled? "включены" : "отключены";
                    var reqsState = Options.LevelRequestsService.GetServiceState();
                    var comment = reqsState == State.Enabled ? "PIZDEC" : "RIZZ";
                    if (!RestrictionHandler.Handle(Restriction.DevMod, chatMessage)) {
                        Client.SendReply(chatMessage.Channel, chatMessage.Id, $"Реквесты {reqsStateStr} {comment}");
                        return;
                    }

                    if (commandArgs.Count > 0) {
                        if (commandArgs[0] == "on") {
                            reqsState = State.Enabled;
                        }
                        if (commandArgs[0] == "off") {
                            reqsState = State.Disabled;
                        }
                    }
                    
                    Options.LevelRequestsService.Options.SetState(reqsState);
                    
                    reqsStateStr = Options.LevelRequestsService.GetServiceState() == State.Enabled? "включены" : "отключены";
                    comment = reqsState == State.Enabled ? "PIZDEC" : "RIZZ";
                    Client.SendReply(chatMessage.Channel, chatMessage.Id, $"Реквесты теперь {reqsStateStr} {comment}");
                    break;
                }

                case "title": {
                    var channelInfo = await HelixUtils.GetChannelInfo(_bot.Options);
                    
                    if (!RestrictionHandler.Handle(Restriction.DevMod, chatMessage) || commandArgs.Count < 1) {
                        Client.SendReply(chatMessage.Channel, chatMessage.Id, $"Название стрима - {channelInfo!.Title}");
                        return;
                    }
                    
                    var titleSb = new StringBuilder();

                    foreach (var arg in commandArgs) {
                        titleSb.Append($"{arg} ");
                    }
                    
                    var result = await HelixUtils.UpdateChannelInfo(_bot.Options, titleSb.ToString(), channelInfo!.GameId);
                    if (!result) {
                        Client.SendReply(chatMessage.Channel, chatMessage.Id, $"Не удалось изменить категорию на {titleSb}");
                        return;
                    }
                    channelInfo = await HelixUtils.GetChannelInfo(_bot.Options);
                    Client.SendReply(chatMessage.Channel, chatMessage.Id, $"Название стрима изменено на {channelInfo!.Title}");
                    break;
                }
                
                case "game": {
                    var channelInfo = await HelixUtils.GetChannelInfo(_bot.Options);
                    
                    if (!RestrictionHandler.Handle(Restriction.DevMod, chatMessage) || commandArgs.Count < 1) {
                        Client.SendReply(chatMessage.Channel, chatMessage.Id, $"Текущая категория - {channelInfo!.GameName}");
                        return;
                    }

                    var gameSb = new StringBuilder();

                    for (var i = 0; i < commandArgs.Count; i++) {
                        if (i == commandArgs.Count-1) {
                            gameSb.Append($"{commandArgs[i]}");
                            break;
                        }
                        gameSb.Append($"{commandArgs[i]} ");
                    }

                    var gameId = await HelixUtils.FindGameId(_bot.Options, gameSb.ToString());
                    var result = await HelixUtils.UpdateChannelInfo(_bot.Options, channelInfo!.Title, gameId!);
                    if (!result || gameId == null) {
                        Client.SendReply(chatMessage.Channel, chatMessage.Id, $"Не удалось изменить категорию на {gameSb}");
                        return;
                    }
                    channelInfo = await HelixUtils.GetChannelInfo(_bot.Options);
                    Client.SendReply(chatMessage.Channel, chatMessage.Id, $"Категория изменена на {channelInfo!.GameName}");
                    break;
                }
                
                case "delay": {
                    var channelInfo = await HelixUtils.GetChannelInfo(_bot.Options);
                    
                    Client.SendReply(chatMessage.Channel, chatMessage.Id, $"Текущая задержка - {channelInfo!.Delay} {Declensioner.Secs(channelInfo.Delay)}");
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

                    if (commandArgs.Count < 1) {
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
                    if (Options.MessageRandomizerService.GetServiceState() == State.Disabled) {
                        ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, Client);
                        return;
                    }
                    
                    Options.MessageRandomizerService.GenerateAndSend(Client, chatMessage.Channel);
                    break;
                }
                #endregion
            }
        } catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Error while handling the command: {e.Message}");
        }
    }

    private void SendUsage(OnChatCommandReceivedArgs args) {
        var usage = $"{Options.CommandIdentifier}<комманда> \"аргумент1\" \"аргумент2\" ... | {Options.CommandIdentifier}cmds для списка комманд";
        Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, usage);
    }

    private void SendEveryonesCmds(OnChatCommandReceivedArgs args, List<string> commandArgs) {
        var cmdId = Options.CommandIdentifier;
        var cmds = new[] {
                             $"1. {cmdId}cmds - список комманд.",
                             $"2. {cmdId}help - использование комманд.",
                             $"3. {cmdId}followage [username] - время, которое пользователь отслеживает канал",
                             $"4. {cmdId}title - название стрима",
                             $"5. {cmdId}game - категория стрима",
                             $"6. {cmdId}req - включены ли реквесты"
                         };
        
        SendPagedReply(cmds, args, commandArgs);
    }
    
    private void SendVipCmds(OnChatCommandReceivedArgs args, List<string> commandArgs) {
        var cmdId = Options.CommandIdentifier;
        var cmds = new[] {
                             $"1. {cmdId}cmds - список комманд.",
                             $"2. {cmdId}help - использование комманд.",
                             $"3. {cmdId}followage [username] - время, которое пользователь отслеживает канал",
                             $"4. {cmdId}title - название стрима",
                             $"5. {cmdId}game - категория стрима",
                             Page.PageTerminator,
                             $"1. {cmdId}echo [message] - эхо",
                             $"2. {cmdId}rizz [message] - RIZZ",
                             $"3. {cmdId}potato - сгенерировать новое сообщение",
                             Page.PageTerminator,
                             $"1. {cmdId}guess <username> - угадать ник написавшего",
                             $"2. {cmdId}whose - вывести ник написавшего",
                             $"3. {cmdId}carrot - зарандомить новое сообщение",
                             $"4. {cmdId}repeat - повторить сообщение",
                         };

        SendPagedReply(cmds, args, commandArgs);
    }

    
    private void SendDevModCmds(OnChatCommandReceivedArgs args, List<string> commandArgs) {
        var cmdId = Options.CommandIdentifier;
        var cmds = new[] {
                             $"1. {cmdId}cmds - список комманд.",
                             $"2. {cmdId}help - использование комманд.",
                             $"3. {cmdId}followage [username] - время, которое пользователь отслеживает канал",
                             Page.PageTerminator,
                             $"1. {cmdId}echo [message] - эхо",
                             $"2. {cmdId}rizz [message] - RIZZ",
                             $"3. {cmdId}potato - сгенерировать новое сообщение",
                             Page.PageTerminator,
                             $"1. {cmdId}req [on/off] - включить/выключить реквесты",
                             $"2. {cmdId}title [new_title] - посмотреть/изменить название стрима",
                             $"3. {cmdId}game [new_game] - посмотреть/изменить категорию стрима",
                             Page.PageTerminator,
                             $"1. {cmdId}guess <nick_name> - угадать ник написавшего",
                             $"2. {cmdId}whose - вывести ник написавшего",
                             $"3. {cmdId}carrot - зарандомить новое сообщение",
                             $"4. {cmdId}repeat - повторить сообщение",
                         };

        SendPagedReply(cmds, args, commandArgs);
    }

    private void SendPagedReply(string[] reply, OnChatCommandReceivedArgs args, List<string> commandArgs) {
        var page = 0;
        if (commandArgs.Count > 0) {
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
        
        message.Append($"Page {page} of {pages[^1]} | ");
        
        for (var i = 0; i < reply.Length; i++) {
            if (reply[i] == Page.PageTerminator) {
                pageTerminatorsCount++;
                continue;
            }
            if (pages[i-pageTerminatorsCount] == page) {
                message.Append($"{reply[i]} ");
            }
        }
        
        Client.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, message.ToString());
    }
}