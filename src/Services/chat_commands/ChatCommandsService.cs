using System.Text;
using ChatBot.bot.interfaces;
using ChatBot.Services.ai;
using ChatBot.Services.demon_list;
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
using ChatBot.utils.GD.AREDL;
using ChatBot.utils.Helix;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace ChatBot.Services.chat_commands;

public class ChatCommandsService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    private bot.ChatBot _bot = null!;
    private long _time;
    private ITwitchClient? Client => _bot.GetClient();

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
                            (LevelRequestsService)ServiceManager.GetService(ServiceName.LevelRequests),
                            (DemonListService)ServiceManager.GetService(ServiceName.DemonList),
                            (AiService)ServiceManager.GetService(ServiceName.AI)
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
        Client?.RemoveChatCommandIdentifier(oldId);
        Client?.AddChatCommandIdentifier(newId);
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

    public int GetVerboseStateAsInt() {
        return (int)Options.VerboseState;
    }

    public void VerboseStateNext() {
        Options.SetVerboseState((State)(((int)Options.VerboseState+1)%Enum.GetValues(typeof(State)).Length));
    }
    
    public async void HandleCmd(object? sender, OnChatCommandReceivedArgs args) {
        try {
            var commandArgs = ProcessArgs(args.Command.ArgumentsAsList);
            var errorHandler = new ErrorHandler(Client);
            var chatMessage = args.Command.ChatMessage;

            try {
                var curTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (curTime-_time < Options.Cooldown) {
                    return;
                }
                _time = curTime;

                ErrorCode err;
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
                        Client?.SendReply(message.Channel, message.Id, randomizedMessage);
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
                
                        Client?.SendReply(message.Channel, message.Id, $"{argSb} отправлен в бан sillyJAIL sillyJAIL sillyJAIL");
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
                
                        Client?.SendMessage(chatMessage.Channel, $"{message} RIZZ RIZZ RIZZ");
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

                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, message);
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
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, message);
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
                        Client?.SendReply(_bot.Options.Channel!, chatMessage.Id, $"Клип создан - https://www.twitch.tv/{_bot.Options.Channel}/clip/{clipId}");
                        break;
                    }
                    case "ai": {
                        if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                            await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                            return;
                        }

                        if (commandArgs.Count < 1) {
                            errorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage);
                            return;
                        }
                        
                        var prompt = new StringBuilder();
                        foreach (var word in commandArgs) {
                            prompt.Append($"{word} ");
                        }

                        var response = await Options.AiService.GenerateText(prompt.ToString().Trim());
                        if (string.IsNullOrEmpty(response)) {
                            errorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage);
                            return;
                        }
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, response);
                        break;
                    }
                    #endregion
                    #region Moderation

                    case "verbose": {
                        var verboseStateStr = 
                            Options.VerboseState == State.Enabled?
                                "включены" :
                                "отключены";
                        var comment =
                            Options.VerboseState == State.Enabled ?
                                "Shiza":
                                "ZACHTO";
                        if (!RestrictionHandler.Handle(Restriction.DevMod, chatMessage) || commandArgs.Count < 1) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Дополнительные логи {verboseStateStr} {comment}");
                            return;
                        }
                    
                        if (commandArgs.Count > 0) {
                            if (commandArgs[0] == "on") {
                                Options.SetVerboseState(State.Enabled);
                            }
                            if (commandArgs[0] == "off") {
                                Options.SetVerboseState(State.Disabled);
                            }
                        }
                    
                        verboseStateStr = 
                            Options.VerboseState == State.Enabled?
                                "включены" :
                                "отключены";
                        comment =
                            Options.VerboseState == State.Enabled ?
                                "Shiza":
                                "ZACHTO";
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Дополнительные логи теперь {verboseStateStr} {comment}");
                        break;
                    }
                    case "req": {
                        var reqsStateStr = 
                            Options.LevelRequestsService.GetServiceState() == State.Enabled?
                                "включены" :
                                "отключены";
                        var reqsState = Options.LevelRequestsService.GetServiceState();
                        var comment = reqsState == State.Enabled ? "PIZDEC" : "RIZZ";
                        if (!RestrictionHandler.Handle(Restriction.DevMod, chatMessage) || commandArgs.Count < 1) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Реквесты {reqsStateStr} {comment}");
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
                    
                        reqsStateStr = 
                            Options.LevelRequestsService.GetServiceState() == State.Enabled? 
                                "включены" : 
                                "отключены";
                        comment = reqsState == State.Enabled ? "PIZDEC" : "RIZZ";
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Реквесты теперь {reqsStateStr} {comment}");
                        break;
                    }

                    case "title": {
                        var channelInfo = await HelixUtils.GetChannelInfo(_bot.Options);
                    
                        if (!RestrictionHandler.Handle(Restriction.DevMod, chatMessage) || commandArgs.Count < 1) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Название стрима - {channelInfo!.Title}");
                            return;
                        }
                    
                        var titleSb = new StringBuilder();

                        foreach (var arg in commandArgs) {
                            titleSb.Append($"{arg} ");
                        }
                    
                        var result = await HelixUtils.UpdateChannelInfo(_bot.Options, titleSb.ToString(), channelInfo!.GameId);
                        if (!result) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Не удалось изменить название");
                            return;
                        }
                        channelInfo = await HelixUtils.GetChannelInfo(_bot.Options);
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Название стрима изменено на {channelInfo!.Title}");
                        break;
                    }
                
                    case "game": {
                        var channelInfo = await HelixUtils.GetChannelInfo(_bot.Options);
                    
                        if (!RestrictionHandler.Handle(Restriction.DevMod, chatMessage)) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Текущая категория - {channelInfo!.GameName}");
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
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Не удалось изменить категорию");
                            return;
                        }
                        channelInfo = await HelixUtils.GetChannelInfo(_bot.Options);
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Категория изменена на {channelInfo!.GameName}");
                        break;
                    }
                
                    case "delay": {
                        var channelInfo = await HelixUtils.GetChannelInfo(_bot.Options);
                    
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Текущая задержка - {channelInfo!.Delay} {Declensioner.Secs(channelInfo.Delay)}");
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
                            Client?.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, "Уже отгадано.");
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
                            Client?.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, "Неправильно.");
                            _logger.Log(LogLevel.Info, $"'{args.Command.ChatMessage.Username}' guessed wrong.");
                        } else {
                            Client?.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id,
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

                        Client?.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, $"Это было сообщение от '{message!.Username}'");
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
                        Client?.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, message!.Msg);
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
                    #region DemonList

                    case "top": {
                        if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                            await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                            return;
                        }
                        if (Options.DemonListService.GetServiceState() == State.Disabled) {
                            errorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage);
                            return;
                        }
                    
                        var index = int.Parse(string.IsNullOrWhiteSpace(commandArgs[0])? "-1" : commandArgs[0]);
                        var levelInfo =  await Options.DemonListService.GetLevelByPlacement(index);
                        if (levelInfo == null) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, "Позиция не найдена.");
                            return;
                        }
                        var verificationLink = await Options.DemonListService.GetLevelVerificationLink(levelInfo.id);
                        if (verificationLink != null) {
                            verificationLink = $"| {verificationLink}";
                        }
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{index} {levelInfo.name} {verificationLink}");
                        break;
                    }
                    case "place": {
                        if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                            await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                            return;
                        }
                        if (Options.DemonListService.GetServiceState() == State.Disabled) {
                            errorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage);
                            return;
                        }
                    
                        string creator = null!;
                        if (commandArgs.Count < 1) {
                            errorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage);
                            return;
                        }

                        var levelName = new StringBuilder();
                        var levelNameEndIndex = 0;
                        for (var i = 0; i < commandArgs.Count; i++) {
                            if (commandArgs[i] is "by" or "от") break;
                            levelName.Append($"{commandArgs[i]} ");
                            levelNameEndIndex = i;
                        }
                    
                        if (levelNameEndIndex+2 < commandArgs.Count) {
                            creator = commandArgs[levelNameEndIndex+2];
                        }
                    
                        var levelInfo =  await Options.DemonListService.GetLevelInfoByName(levelName.ToString().Trim(), creator);
                        if (levelInfo == null) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, "Уровень не найден.");
                            return;
                        }
                        var verificationLink = await Options.DemonListService.GetLevelVerificationLink(levelInfo.id);
                        if (verificationLink != null) {
                            verificationLink = $"| {verificationLink}";
                        }
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{levelInfo.position} {levelInfo.name} {verificationLink}");
                        break;
                    }

                    case "ptop": {
                        if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                            await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                            return;
                        }
                        if (Options.DemonListService.GetServiceState() == State.Disabled) {
                            errorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage);
                            return;
                        }
                    
                        var index = int.Parse(string.IsNullOrWhiteSpace(commandArgs[0])? "-1" : commandArgs[0]);
                        var levelInfo =  await Options.DemonListService.GetPlatformerLevelByPlacement(index);
                        if (levelInfo == null) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, "Позиция не найдена.");
                            return;
                        }
                        var verificationLink = await Options.DemonListService.GetPlatformerLevelVerificationLink(levelInfo.id);
                        if (verificationLink != null) {
                            verificationLink = $"| {verificationLink}";
                        }
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{index} {levelInfo.name} {verificationLink}");
                        break;
                    }
                    case "pplace": {
                        if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                            await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                            return;
                        }
                        if (Options.DemonListService.GetServiceState() == State.Disabled) {
                            errorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage);
                            return;
                        }
                    
                        string creator = null!;
                        if (commandArgs.Count < 1) {
                            errorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage);
                            return;
                        }

                        var levelName = new StringBuilder();
                        var levelNameEndIndex = 0;
                        for (var i = 0; i < commandArgs.Count; i++) {
                            if (commandArgs[i] is "by" or "от") break;
                            levelName.Append($"{commandArgs[i]} ");
                            levelNameEndIndex = i;
                        }
                    
                        if (levelNameEndIndex+2 < commandArgs.Count) {
                            creator = commandArgs[levelNameEndIndex+2];
                        }
                    
                        var levelInfo = await Options.DemonListService.GetPlatformerLevelInfoByName(levelName.ToString().Trim(), creator);
                        if (levelInfo == null) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, "Уровень не найден.");
                            return;
                        }
                        var verificationLink = await Options.DemonListService.GetPlatformerLevelVerificationLink(levelInfo.id);
                        if (verificationLink != null) {
                            verificationLink = $"| {verificationLink}";
                        }
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{levelInfo.position} {levelInfo.name} {verificationLink}");
                        break;
                    }
                    case "hardest": {
                        if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                            await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex); 
                            return;
                        }
                        if (Options.DemonListService.GetServiceState() == State.Disabled) {
                            errorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage);
                            return;
                        }
                    
                        if (commandArgs.Count < 1) {
                            var levels = await AredlUtils.ListLevels();
                            if (levels == null || levels.data?.Count < 1) {
                                if (Options.VerboseState == State.Enabled) {
                                    Client?.SendReply(chatMessage.Channel, chatMessage.Id, "Что-то пошло не так.");
                                }
                                return;
                            }
                            var level = levels.data?[0];
                            var details = await Options.DemonListService.GetLevelDetails(level?.id!);
                            if (details == null || details.verifications.Count < 1) {
                                if (Options.VerboseState == State.Enabled) {
                                    Client?.SendReply(chatMessage.Channel, chatMessage.Id, "Что-то пошло не так.");
                                }
                                return;
                            }
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{level?.position} {level?.name} | {details.verifications[0].videoUrl}");
                            return;
                        }
                 
                        var argSb = new StringBuilder();
                        foreach (var arg in commandArgs) {
                            argSb.Append($"{arg} ");
                        }
                        var username = argSb.ToString();
                    
                        var profile = await Options.DemonListService.GetProfile(username.Trim());
                        if (profile == null) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, "Пользователь не найден.");
                            return;
                        }
                        var hardest = await Options.DemonListService.GetHardest(profile);
                        if (hardest == null) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, "Хардест не найден.");
                            return;
                        }
                        
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{hardest.level.position} {profile.hardest?.name} | {hardest.videoUrl}");
                        break;
                    }
                    case "lowest":
                    case "easiest": {
                        if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                            await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                            return;
                        }
                        if (Options.DemonListService.GetServiceState() == State.Disabled) {
                            errorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage);
                            return;
                        }
                    
                        if (commandArgs.Count < 1) {
                            var levels = await AredlUtils.ListLevels();
                            if (levels == null || levels.data?.Count < 1) {
                                if (Options.VerboseState == State.Enabled) {
                                    Client?.SendReply(chatMessage.Channel, chatMessage.Id, "Что-то пошло не так.");
                                }
                                return;
                            }
                            var level = levels.data?[^1];
                            var i = 2;
                            while (level!.legacy) {
                                level = levels.data?[^i++];
                            }
                            var details = await Options.DemonListService.GetLevelDetails(level.id);
                            if (details == null || details.verifications.Count < 1) {
                                if (Options.VerboseState == State.Enabled) {
                                    Client?.SendReply(chatMessage.Channel, chatMessage.Id, "Не найдено.");
                                }
                                return;
                            }
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{level.position} {level.name} | {details.verifications[0].videoUrl}");
                            return;
                        }

                        var argSb = new StringBuilder();
                        foreach (var arg in commandArgs) {
                            argSb.Append($"{arg} ");
                        }
                        var username = argSb.ToString();
                    
                        var profile = await Options.DemonListService.GetProfile(username.Trim());
                        if (profile == null) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, "Пользователь не найден.");
                            return;
                        }
                        var easiest = await Options.DemonListService.GetEasiest(profile);
                        if (easiest == null) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, "Не найдено.");
                            return;
                        }
                        
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{easiest.level.position} {easiest.level.name} | {easiest.videoUrl}");
                        break;
                    }
                    case "rulet":
                    case "roulette": {
                        var from = -1;
                        var to = -1;
                        
                        if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                            await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                            return;
                        }
                        if (Options.DemonListService.GetServiceState() == State.Disabled) {
                            errorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage);
                            return;
                        }

                        if (commandArgs.Count > 0) {
                            from = int.Parse(commandArgs[0]);
                            if (commandArgs.Count > 1) {
                                to = int.Parse(commandArgs[1]);
                            }
                        }
                        
                        var levelInfo = await Options.DemonListService.GetRandomLevel(from, to);
                        if (levelInfo == null) {
                            errorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage);
                            return;
                        }
                        var verificationLink = await Options.DemonListService.GetLevelVerificationLink(levelInfo.id);
                        if (verificationLink != null) {
                            verificationLink = $"| {verificationLink}";
                        }
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{levelInfo.position} {levelInfo.name} {verificationLink}");
                        break;
                    }
                    case "clan-hardest": {
                        if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                            await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                            return;
                        }
                        if (Options.DemonListService.GetServiceState() == State.Disabled) {
                            errorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage);
                            return;
                        }

                        if (commandArgs.Count < 1) {
                            errorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage);
                            return;
                        }
                    
                        var clanInfo = await Options.DemonListService.GetClanInfo(commandArgs[0]);
                        if (clanInfo == null) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Клана не существует.");
                            return;
                        }
                        var hardest = await Options.DemonListService.GetLevelDetails(clanInfo.hardest.id);
                        var verificationLink = string.Empty;
                        if (hardest?.verifications.Count > 0) {
                            verificationLink = $"| {hardest.verifications[0].videoUrl}";
                        }
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{hardest?.position} {clanInfo.hardest.name} {verificationLink}");
                        break;
                    }
                    case "clan-rulet":
                    case "clan-roulette": {
                        if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                            await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                            return;
                        }
                        if (Options.DemonListService.GetServiceState() == State.Disabled) {
                            errorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage);
                            return;
                        }

                        if (commandArgs.Count < 1) {
                            errorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage);
                            return;
                        }
                    
                        var clanInfo = await Options.DemonListService.GetClanInfo(commandArgs[0]);
                        if (clanInfo == null) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Клана не существует.");
                            return;
                        }
                        var levelInfo = await Options.DemonListService.GetRandomClanSubmission(clanInfo.clan.id)!;
                        if (levelInfo == null) {
                            errorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage);
                            return;
                        }
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{levelInfo.level?.position} {levelInfo.level?.name} | {levelInfo.videoUrl}");
                        break;
                    }
                    case "clan": {
                        if (!RestrictionHandler.Handle(Options.RequiredRole, chatMessage)) {
                            await Options.ModerationService.WarnUser(chatMessage, Options.ModActionIndex);
                            return;
                        }
                        if (Options.DemonListService.GetServiceState() == State.Disabled) {
                            errorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage);
                            return;
                        }
                        if (commandArgs.Count < 1) {
                            errorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage);
                            return;
                        }
                        
                        var clanInfo = await Options.DemonListService.GetClanInfo(commandArgs[0]);
                        if (clanInfo == null) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Клана не существует.");
                            return;
                        }
                        
                        Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"[{clanInfo.clan.tag}] {clanInfo.clan.globalName} | https://aredl.net/clans/{clanInfo.clan.id}");
                        break;
                    }
                    #endregion
                    default: {
                        if (Options.VerboseState == State.Enabled) {
                            Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Неизвестная комманда: {Options.CommandIdentifier}{args.Command.CommandText}");
                        }
                        break;
                    }
                }
            } catch (Exception e) {
                if (Options.VerboseState == State.Enabled) {
                    Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Ошибка при обработке команды {Options.CommandIdentifier}{args.Command.CommandText}.");
                }
                _logger.Log(LogLevel.Error, $"Error while handling the command: {e.Message}");
            }
        } catch (Exception) {
            _logger.Log(LogLevel.Error, $"An exception has been caught while handling a command: {args.Command.CommandIdentifier}{args.Command.CommandText}");
        }
    }

    private void SendUsage(OnChatCommandReceivedArgs args) {
        var usage = $"{Options.CommandIdentifier}<комманда> \"аргумент1\" \"аргумент2\" ... | {Options.CommandIdentifier}cmds для списка комманд";
        Client?.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, usage);
    }

    private void SendEveryonesCmds(OnChatCommandReceivedArgs args, List<string> commandArgs) {
        var cmdId = Options.CommandIdentifier;
        var cmds = new[] {
                             $"1. {cmdId}cmds - список комманд.",
                             $"2. {cmdId}help - использование комманд.",
                             Page.PageTerminator,
                             $"1. {cmdId}followage [username] - время, которое пользователь отслеживает канал",
                             $"2. {cmdId}title - название стрима",
                             $"3. {cmdId}game - категория стрима",
                             Page.PageTerminator,
                             $"1. {cmdId}req - включены ли реквесты",
                             $"2. {cmdId}hardest <aredl_username> - хардест пользователя по AREDL",
                             $"3. {cmdId}easiest/lowest [ared_username] - легчайший экстрим пользователя по AREDL",
                             $"4. {cmdId}clan-hardest - хардест клана",
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
                             $"1. {cmdId}rizz [message] - RIZZ",
                             $"2. {cmdId}potato - сгенерировать новое сообщение",
                             $"3. {cmdId}ai - задать вопрос ии",
                             Page.PageTerminator,
                             $"1. {cmdId}guess <username> - угадать ник написавшего",
                             $"2. {cmdId}whose - вывести ник написавшего",
                             $"3. {cmdId}carrot - зарандомить новое сообщение",
                             $"4. {cmdId}repeat - повторить сообщение",
                             Page.PageTerminator,
                             $"1. {cmdId}top <placement> - уровень стоящий на данной позиции по AREDL",
                             $"2. {cmdId}place <level_name> [by creator_name] - позиция уровня по AREDL",
                             $"3. {cmdId}hardest [aredl_username] - хардест пользователя по AREDL",
                             $"4. {cmdId}easiest/lowest [ared_username] - легчайший экстрим пользователя по AREDL",
                             Page.PageTerminator,
                             $"1. {cmdId}ptop <placement> - уровень стоящий на данной позиции по Pemon List",
                             $"2. {cmdId}pplace <level_name> [by creator_name] - позиция уровня по Pemon List",
                             Page.PageTerminator,
                             $"1. {cmdId}clan-hardest - хардест клана",
                             $"2. {cmdId}clan-roulette/clan-rulet - случайный пройденный кланом уровень",
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
                             $"1. {cmdId}rizz [message] - RIZZ",
                             $"2. {cmdId}potato - сгенерировать новое сообщение",
                             $"3. {cmdId}ai - задать вопрос ии",
                             Page.PageTerminator,
                             $"1. {cmdId}req [on/off] - включить/выключить реквесты",
                             $"2. {cmdId}title [new_title] - посмотреть/изменить название стрима",
                             $"3. {cmdId}game [new_game] - посмотреть/изменить категорию стрима",
                             $"4. {cmdId}verbose [on/off] - включить/выключить дополнительные логи",
                             Page.PageTerminator,
                             $"1. {cmdId}guess <nick_name> - угадать ник написавшего",
                             $"2. {cmdId}whose - вывести ник написавшего",
                             $"3. {cmdId}carrot - зарандомить новое сообщение",
                             $"4. {cmdId}repeat - повторить сообщение",
                             Page.PageTerminator,
                             $"1. {cmdId}top <placement> - уровень стоящий на данной позиции по AREDL",
                             $"2. {cmdId}place <level_name> [by creator_name] - позиция уровня по AREDL",
                             $"3. {cmdId}hardest <aredl_username> - хардест пользователя по AREDL",
                             $"4. {cmdId}easiest/lowest [ared_username] - легчайший экстрим пользователя по AREDL",
                             Page.PageTerminator,
                             $"1. {cmdId}ptop <placement> - уровень стоящий на данной позиции по Pemon List",
                             $"2. {cmdId}pplace <level_name> [by creator_name] - позиция уровня по Pemon List",
                             Page.PageTerminator,
                             $"1. {cmdId}clan-hardest - хардест клана",
                             $"2. {cmdId}clan-roulette/clan-rulet - случайный пройденный кланом уровень",
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
        
        Client?.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, message.ToString());
    }
}