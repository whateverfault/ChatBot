using System.Text;
using ChatBot.bot.interfaces;
using ChatBot.Services.ai;
using ChatBot.Services.demon_list;
using ChatBot.Services.level_requests;
using ChatBot.Services.logger;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.Static;
using ChatBot.Services.text_generator;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;
using ChatBot.utils.GD.AREDL;
using ChatBot.utils.Helix;
using MessageState = ChatBot.Services.message_randomizer.MessageState;

namespace ChatBot.Services.chat_commands;

public static class CommandsList {
    private static readonly ChatCommandsService _chatCmds = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
    
    public static List<DefaultChatCommand> DefaultsCommands { get; }


    static CommandsList() {
        DefaultsCommands = [
                               new DefaultChatCommand(
                                                      "cmds",
                                                      "[page]",
                                                      "список комманд.",
                                                      Cmds,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      "help",
                                                      string.Empty,
                                                      "использование комманд.",
                                                      Help,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      "followage",
                                                      "[username]",
                                                      "время, которое пользователь отслеживает канал.",
                                                      Followage,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      "title",
                                                      "[title]",
                                                      "посмотреть/изменить название стрима.",
                                                      Title,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      "game",
                                                      "[game]",
                                                      "посмотреть/изменить категорию стрима.",
                                                      Game,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      "clip",
                                                      string.Empty,
                                                      "создать клип.",
                                                      Clip,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      "delay",
                                                      string.Empty,
                                                      "узнать задержку",
                                                      Delay,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      "rizz",
                                                      "[text]",
                                                      "RIZZ",
                                                      Rizz,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      "when",
                                                      "[text]",
                                                      "Waiting",
                                                      When,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      "ban",
                                                      "[text]",
                                                      "SillyJail",
                                                      Ban,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      "ai",
                                                      "[prompt]",
                                                      "задать вопрос ии.",
                                                      Ai,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      "potato",
                                                      string.Empty,
                                                      "сгенерировать сообщение",
                                                      Potato,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      "carrot",
                                                      string.Empty,
                                                      "зарандомить новое сообщение",
                                                      Carrot,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      "req",
                                                      "[on/off]",
                                                      "включить/выключить/узнать включены ли реквесты",
                                                      Req,
                                                      Restriction.DevMod
                                                     ),
                               new DefaultChatCommand(
                                                      "verbose",
                                                      "[on/off]",
                                                      "включить/выключить/узнать включены ли дополнительные логи",
                                                      Verbose,
                                                      Restriction.DevMod
                                                     ),
                               new DefaultChatCommand(
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      "guess",
                                                      "[username]",
                                                      "угадать ник написавшего.",
                                                      Guess,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      "whose",
                                                      string.Empty,
                                                      "узнать ник написавшего.",
                                                      Whose,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      "repeat",
                                                      string.Empty,
                                                      "повторить сообщение.",
                                                      Repeat,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      "top",
                                                      "<position>",
                                                      "уровень стоящий на данной позиции по AREDL.",
                                                      Top,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      "place",
                                                      "<level_name> [by creator_name] [--page page_count]",
                                                      "позиция уровня по AREDL.",
                                                      Place,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      "roulette",
                                                      "<from> <to>",
                                                      "зарандомить экстрим.",
                                                      Roulette,
                                                      Restriction.Vip,
                                                      aliases: ["rulet"]
                                                     ),
                               new DefaultChatCommand(
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      "hardest",
                                                      "[username]",
                                                      "хардест пользователя по AREDL.",
                                                      Hardest,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      "easiest",
                                                      "[username]",
                                                      "легчайший экстрим пользователя по AREDL.",
                                                      Easiest,
                                                      Restriction.Vip,
                                                      aliases: ["lowest"]
                                                     ),
                               new DefaultChatCommand(
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      "ptop",
                                                      "<position>",
                                                      "уровень стоящий на данной позиции по Pemon List.",
                                                      Ptop,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      "pplace",
                                                      "<level> [by creator_name] [--page page_count]",
                                                      "позиция уровня по Pemon List.",
                                                      Pplace,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      "clan",
                                                      "<clan_tag>",
                                                      "Информация о клане.",
                                                      Clan,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      "clan-hardest",
                                                      "<clan_tag>",
                                                      "хардест клана.",
                                                      ClanHardest,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      "clan-roulette",
                                                      "<clan_tag>",
                                                      "случайный пройденный кланом уровень.",
                                                      ClanRoulette,
                                                      Restriction.Vip,
                                                      aliases: ["clan-rulet"]
                                                     ),
                           ];
    }
    
    public static void SetDefaults() {
        _chatCmds.Options.SetDefaultCmds(DefaultsCommands);
    }

    private static Task Cmds(ChatCmdArgs cmdArgs) {
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var cmdId = cmdArgs.Args.Command.CommandIdentifier;
        var index = 1;
        var cmds = new List<string>();

        for (var i = 0; i < _chatCmds.Options.DefaultCmds.Count; i++, index++) {
            if (!RestrictionHandler.Handle(_chatCmds.Options.DefaultCmds[i].Restriction, chatMessage)) continue;
            
            if (string.IsNullOrEmpty(_chatCmds.Options.DefaultCmds[i].Name)) {
                if (cmds[^1].Equals(Page.pageTerminator)) continue;
                cmds.Add(Page.pageTerminator);
                index = 0;
                continue;
            }
            
            cmds.Add(string.IsNullOrEmpty(_chatCmds.Options.DefaultCmds[i].Args)?
                         $"{index}. {cmdId}{_chatCmds.Options.DefaultCmds[i].Name} - {_chatCmds.Options.DefaultCmds[i].Description} " 
                         : $"{index}. {cmdId}{_chatCmds.Options.DefaultCmds[i].Name} {_chatCmds.Options.DefaultCmds[i].Args} - {_chatCmds.Options.DefaultCmds[i].Description} "
                     );
        }

        SendPagedReply(cmds, cmdArgs);
        return Task.CompletedTask;
    }
    
    private static Task Help(ChatCmdArgs cmdArgs) {
        var client = cmdArgs.Bot.GetClient();
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var cmdId = cmdArgs.Args.Command.CommandIdentifier;
        
        var usage = $"{cmdId}<комманда> \"аргумент1\" \"аргумент2\" ... | {cmdId}{_chatCmds.Options.DefaultCmds[0].Name} для списка комманд";
        client?.SendReply(chatMessage.Channel, chatMessage.Id, usage);
        
        return Task.CompletedTask;
    }

        private static Task When(ChatCmdArgs cmdArgs) {
        var client = cmdArgs.Bot.GetClient();
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        
        if (cmdArgs.Parsed.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return Task.CompletedTask;
        }

        var argSb = new StringBuilder();
                
        foreach (var arg in cmdArgs.Parsed) {
            argSb.Append($"{arg} ");
        }

        var random = Random.Shared.Next(0, 2);
        var randomizedMessage = 
            random == 0 ? 
                $"{argSb} уже завтра! PewPewPew PewPewPew PewPewPew" : 
                $"{argSb} никогда GAGAGA GAGAGA GAGAGA";
        client?.SendReply(chatMessage.Channel, chatMessage.Id, randomizedMessage);
        return Task.CompletedTask;
    }

    private static Task Ban(ChatCmdArgs cmdArgs) {
        var client = cmdArgs.Bot.GetClient();
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        
        if (cmdArgs.Parsed.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return Task.CompletedTask;
        }

        var argSb = new StringBuilder();
                
        foreach (var arg in cmdArgs.Parsed) {
            argSb.Append($"{arg} ");
        }
                
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"{argSb} отправлен в бан sillyJAIL sillyJAIL sillyJAIL");
        return Task.CompletedTask;
    }

    private static async Task Ai(ChatCmdArgs cmdArgs) {
        var ai = (AiService)ServiceManager.GetService(ServiceName.Ai);
        var client = cmdArgs.Bot.GetClient();
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        
        if (cmdArgs.Parsed.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var prompt = new StringBuilder();
        foreach (var word in cmdArgs.Parsed) {
            prompt.Append($"{word} ");
        }

        var response = await ai.GenerateText(prompt.ToString().Trim());
        
        if (string.IsNullOrEmpty(response)) {
            ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
            return;
        }
        client?.SendReply(chatMessage.Channel, chatMessage.Id, response);
    }

    private static Task Verbose(ChatCmdArgs cmdArgs) {
        var chatCommands = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
        var client = cmdArgs.Bot.GetClient();
        var chatMessage = cmdArgs.Args.Command.ChatMessage;

        var verboseStateStr = 
            chatCommands.Options.VerboseState == State.Enabled?
                "включены" :
                "отключены";
        var comment =
            chatCommands.Options.VerboseState == State.Enabled ?
                "Shiza":
                "ZACHTO";
        if (!RestrictionHandler.Handle(Restriction.DevMod, chatMessage) || cmdArgs.Parsed.Count < 1) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Дополнительные логи {verboseStateStr} {comment}");
            return Task.CompletedTask;
        }

        if (cmdArgs.Parsed.Count > 0) {
            if (cmdArgs.Parsed[0] == "on") {
                chatCommands.Options.SetVerboseState(State.Enabled);
            }
            if (cmdArgs.Parsed[0] == "off") {
                chatCommands.Options.SetVerboseState(State.Disabled);
            }
        }

        verboseStateStr = 
            chatCommands.Options.VerboseState == State.Enabled?
                "включены" :
                "отключены";
        comment =
            chatCommands.Options.VerboseState == State.Enabled ?
                "Shiza":
                "ZACHTO";
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Дополнительные логи теперь {verboseStateStr} {comment}");
        return Task.CompletedTask;
    }

    private static Task Req(ChatCmdArgs cmdArgs) {
        var levelRequests = (LevelRequestsService)ServiceManager.GetService(ServiceName.LevelRequests);
        var client = cmdArgs.Bot.GetClient();
        var chatMessage = cmdArgs.Args.Command.ChatMessage;

        var reqsStateStr = 
            levelRequests.GetServiceState() == State.Enabled?
                "включены" :
                "отключены";
        var reqsState = levelRequests.GetServiceState();
        var comment = reqsState == State.Enabled ? "PIZDEC" : "RIZZ";
        
        if (!RestrictionHandler.Handle(Restriction.DevMod, chatMessage) || cmdArgs.Parsed.Count < 1) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Реквесты {reqsStateStr} {comment}");
            return Task.CompletedTask;
        }

        if (cmdArgs.Parsed.Count > 0) {
            if (cmdArgs.Parsed[0] == "on") {
                reqsState = State.Enabled;
            }
            if (cmdArgs.Parsed[0] == "off") {
                reqsState = State.Disabled;
            }
        }

        levelRequests.Options.SetState(reqsState);

        reqsStateStr = 
            levelRequests.GetServiceState() == State.Enabled? 
                "включены" : 
                "отключены";
        comment = reqsState == State.Enabled ? "PIZDEC" : "RIZZ";
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Реквесты теперь {reqsStateStr} {comment}");
        return Task.CompletedTask;
    }

    private static Task Potato(ChatCmdArgs chatCmdArgs) {
        var textGenerator = (TextGeneratorService)ServiceManager.GetService(ServiceName.TextGenerator);
        
        textGenerator.GenerateAndSend();
        return Task.CompletedTask;
    }

    private static async Task Clip(ChatCmdArgs cmdArgs) {
        var client = cmdArgs.Bot.GetClient();
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        
        var clipId = await HelixUtils.CreateClipHelix(cmdArgs.Bot.Options);

        if (clipId == null) {
            ErrorHandler.ReplyWithError(ErrorCode.ClipCreationFailed, chatMessage, client);
            return;
        }
        client?.SendReply(cmdArgs.Bot.Options.Channel!, chatMessage.Id, $"Клип создан - https://www.twitch.tv/{cmdArgs.Bot.Options.Channel}/clip/{clipId}");
    }
    
    private static Task Rizz(ChatCmdArgs cmdArgs) {
        var client = cmdArgs.Bot.GetClient();
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        
        var argSb = new StringBuilder();

        foreach (var arg in cmdArgs.Parsed) {
            argSb.Append($"{arg} ");
        }
                
        var message =
            cmdArgs.Parsed.Count == 0 ?
                "КШЯЯ" :
                argSb.ToString();
                
        client?.SendMessage(chatMessage.Channel, $"{message} RIZZ RIZZ RIZZ");
        return Task.CompletedTask;
    }

    private static async Task Title(ChatCmdArgs cmdArgs) {
        var client = cmdArgs.Bot.GetClient();
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var channelInfo = await HelixUtils.GetChannelInfo(cmdArgs.Bot.Options);
                    
        if (!RestrictionHandler.Handle(Restriction.DevMod, chatMessage) || cmdArgs.Parsed.Count < 1) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Название стрима - {channelInfo!.Title}");
            return;
        }
                    
        var titleSb = new StringBuilder();

        foreach (var arg in cmdArgs.Parsed) {
            titleSb.Append($"{arg} ");
        }
                    
        var result = await HelixUtils.UpdateChannelInfo(cmdArgs.Bot.Options, titleSb.ToString(), channelInfo!.GameId);
        if (!result) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Не удалось изменить название");
            return;
        }
        channelInfo = await HelixUtils.GetChannelInfo(cmdArgs.Bot.Options);
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Название стрима изменено на {channelInfo!.Title}");
    }
    
    private static async Task Followage(ChatCmdArgs cmdArgs) {
        var client = cmdArgs.Bot.GetClient(); 
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        
        string message;
        var username = chatMessage.Username;
        if (cmdArgs.Parsed.Count > 0) {
            username = cmdArgs.Parsed[0];
        }

        var followage = await HelixUtils.GetFollowageHelix(cmdArgs.Bot.Options, username!);
        if (followage == null) {
            if (cmdArgs.Parsed.Count > 0) {
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

            client?.SendReply(chatMessage.Channel, chatMessage.Id, message);
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
            cmdArgs.Parsed.Count > 0 ? 
                $"{username} фолловнут на {chatMessage.Channel} {years} {months} {days}" :
                $"Вы фолловнуты на {chatMessage.Channel} {years} {months} {days}";
        client?.SendReply(chatMessage.Channel, chatMessage.Id, message);
    }

    private static async Task Game(ChatCmdArgs cmdArgs) {
        var client = cmdArgs.Bot.GetClient();
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var channelInfo = await HelixUtils.GetChannelInfo(cmdArgs.Bot.Options);
                    
        if (!RestrictionHandler.Handle(Restriction.DevMod, chatMessage)) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Текущая категория - {channelInfo!.GameName}");
            return;
        }

        var gameSb = new StringBuilder();

        for (var i = 0; i < cmdArgs.Parsed.Count; i++) {
            if (i == cmdArgs.Parsed.Count-1) {
                gameSb.Append($"{cmdArgs.Parsed[i]}");
                break;
            }
            gameSb.Append($"{cmdArgs.Parsed[i]} ");
        }

        var gameId = await HelixUtils.FindGameId(cmdArgs.Bot.Options, gameSb.ToString());
        var result = await HelixUtils.UpdateChannelInfo(cmdArgs.Bot.Options, channelInfo!.Title, gameId!);
        if (!result || gameId == null) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Не удалось изменить категорию");
            return;
        }
        channelInfo = await HelixUtils.GetChannelInfo(cmdArgs.Bot.Options);
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Категория изменена на {channelInfo!.GameName}");
    }

    private static async Task Delay(ChatCmdArgs cmdArgs) {
        var client = cmdArgs.Bot.GetClient();
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var channelInfo = await HelixUtils.GetChannelInfo(cmdArgs.Bot.Options);
                    
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Текущая задержка - {channelInfo!.Delay} {Declensioner.Secs(channelInfo.Delay)}");
    }
    
    private static Task Guess(ChatCmdArgs cmdArgs) {
        var messageRandomizer = (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var client = cmdArgs.Bot.GetClient();
        
        if (messageRandomizer.Options.MessageState == MessageState.Guessed) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, "Уже отгадано.");
            return Task.CompletedTask;
        }

        if (cmdArgs.Parsed.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            logger.Log(LogLevel.Error, $"Too few arguments for '{cmdArgs.Args.Command.CommandText}' command");
            return Task.CompletedTask;
        }

        var err = messageRandomizer.GetLastGeneratedMessage(out var message);
        if (ErrorHandler.ReplyWithError(err, chatMessage, client)) {
            logger.Log(LogLevel.Info, "Tried to access last random message while there is no such.");
            return Task.CompletedTask;
        }

        if (cmdArgs.Parsed[0] != message!.Username) {
            client?.SendReply(chatMessage.Channel,chatMessage.Id, "Неправильно.");
        } else {
            client?.SendReply(chatMessage.Channel, chatMessage.Id,
                              $"Правильно, это было сообщение от {message.Username}.");
            messageRandomizer.Options.SetMessageState(MessageState.Guessed);
        }
        
        return Task.CompletedTask;
    }

    private static Task Whose(ChatCmdArgs cmdArgs) {
        var messageRandomizer = (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var client = cmdArgs.Bot.GetClient();
        
        var err = messageRandomizer.GetLastGeneratedMessage(out var message);
        if (ErrorHandler.ReplyWithError(err, chatMessage, client)) {
            ErrorHandler.ReplyWithError(err, chatMessage, client);
            logger.Log(LogLevel.Info, "Tried to access last random message while there are no such.");
            return Task.CompletedTask;
        }

        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Это было сообщение от '{message!.Username}'");
        return Task.CompletedTask;
    }

    private static Task Repeat(ChatCmdArgs cmdArgs) {
        var messageRandomizer = (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var client = cmdArgs.Bot.GetClient();
        
        var err = messageRandomizer.GetLastGeneratedMessage(out var message);
        if (ErrorHandler.ReplyWithError(err, chatMessage, client)) {
            ErrorHandler.ReplyWithError(err, chatMessage, client);
            logger.Log(LogLevel.Info, "Tried to access last random message while there are no such.");
            return Task.CompletedTask;
        }
        
        client?.SendReply(chatMessage.Channel, chatMessage.Id, message!.Msg);
        logger.Log(LogLevel.Info, $"Repeated last message for {chatMessage.Username}.");
        return Task.CompletedTask;
    }

    private static Task Carrot(ChatCmdArgs cmdArgs) {
        var messageRandomizer = (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var client = cmdArgs.Bot.GetClient();
        
        if (messageRandomizer.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            logger.Log(LogLevel.Error, "Message Randomizer service is disabled");
            return Task.CompletedTask;
        }
                    
        messageRandomizer.GenerateAndSend(client, chatMessage.Channel);
        return Task.CompletedTask;
    }

    private static async Task Top(ChatCmdArgs cmdArgs) {
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var client = cmdArgs.Bot.GetClient();
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            logger.Log(LogLevel.Error, "Demon List service is disabled");
            return;
        }
                    
        var index = int.Parse(string.IsNullOrWhiteSpace(cmdArgs.Parsed[0])? "-1" : cmdArgs.Parsed[0]);
        var levelInfo =  await demonList.GetLevelByPlacement(index);
        if (levelInfo == null) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, "Позиция не найдена.");
            return;
        }
        var verificationLink = await demonList.GetLevelVerificationLink(levelInfo.id);
        if (verificationLink != null) {
            verificationLink = $"| {verificationLink}";
        }
        var tier = 
            levelInfo.nlwTier == null? 
                "(List tier)":
                $"({levelInfo.nlwTier} tier)";
        
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{index} {levelInfo.name} {tier} {verificationLink}");
    }

    private static async Task Place(ChatCmdArgs cmdArgs) {
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var client = cmdArgs.Bot.GetClient();

        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (cmdArgs.Parsed.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var levelName = new StringBuilder();
        foreach (var arg in cmdArgs.Parsed) {
            if (arg is "by" or "от" or "--page") break;
            levelName.Append($"{arg} ");
        }

        var index = cmdArgs.Parsed.IndexOf("by");
        if (index != -1 && index+1 < cmdArgs.Parsed.Count) {
            var creator = cmdArgs.Parsed[index+1];
            levelName.Append($" ({creator})");
        }

        var levelsInfo =  await demonList.GetLevelsInfoByName(levelName.ToString().Trim());

        if (levelsInfo == null) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, "Уровень не найден.");
            return;
        }
        
        var page = 0;
        index = cmdArgs.Parsed.IndexOf("--page");
        if (index != -1 && cmdArgs.Parsed.Count > index+1) {
            page = int.Parse(cmdArgs.Parsed[index+1])-1;
        }

        if (page < 0) page = 0;
        if (page >= levelsInfo.Count) page = levelsInfo.Count-1;
        
        var levelInfo = levelsInfo[page];
        var verificationLink = await demonList.GetLevelVerificationLink(levelInfo.id);
        if (verificationLink != null) {
            verificationLink = $"| {verificationLink}";
        }
        
        var tier = 
            levelInfo.nlwTier == null? 
                string.Empty:
                $"({levelInfo.nlwTier} tier)";

        var pages = 
            levelsInfo.Count <= 1 ? 
                string.Empty : 
                $"Страница {page+1} из {levelsInfo.Count} |";
        
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"{pages} #{levelInfo.position} {levelInfo.name} {tier} {verificationLink}");
    }

    private static async Task Ptop(ChatCmdArgs cmdArgs) {
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var client = cmdArgs.Bot.GetClient();
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            logger.Log(LogLevel.Error, "Demon List service is disabled");
            return;
        }
                    
        var index = int.Parse(string.IsNullOrWhiteSpace(cmdArgs.Parsed[0])? "-1" : cmdArgs.Parsed[0]);
        var levelInfo =  await demonList.GetPlatformerLevelByPlacement(index);
        if (levelInfo == null) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, "Позиция не найдена.");
            return;
        }
        var verificationLink = await demonList.GetPlatformerLevelVerificationLink(levelInfo.id);
        if (verificationLink != null) {
            verificationLink = $"| {verificationLink}";
        }
        
        var tier = 
            levelInfo.nlwTier == null? 
                "(List tier)":
                $"({levelInfo.nlwTier} tier)";
        
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{index} {levelInfo.name} {tier} {verificationLink}");
    }
    
    private static async Task Pplace(ChatCmdArgs cmdArgs) {
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var client = cmdArgs.Bot.GetClient();

        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (cmdArgs.Parsed.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var levelName = new StringBuilder();
        foreach (var arg in cmdArgs.Parsed) {
            if (arg is "by" or "от" or "--page") break;
            levelName.Append($"{arg} ");
        }

        var index = cmdArgs.Parsed.IndexOf("by");
        if (index != -1 && index+1 < cmdArgs.Parsed.Count) {
            var creator = cmdArgs.Parsed[index+1];
            levelName.Append($" ({creator})");
        }

        var levelsInfo =  await demonList.GetPlatformerLevelsInfoByName(levelName.ToString().Trim());

        if (levelsInfo == null) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, "Уровень не найден.");
            return;
        }
        
        var page = 0;
        index = cmdArgs.Parsed.IndexOf("--page");
        if (index != -1 && cmdArgs.Parsed.Count > index+1) {
            page = int.Parse(cmdArgs.Parsed[index+1])-1;
        }

        if (page < 0) page = 0;
        if (page >= levelsInfo.Count) page = levelsInfo.Count-1;
        
        var levelInfo = levelsInfo[page];
        var verificationLink = await demonList.GetLevelVerificationLink(levelInfo.id);
        if (verificationLink != null) {
            verificationLink = $"| {verificationLink}";
        }
        
        var tier = 
            levelInfo.nlwTier == null? 
                "(List tier)":
                $"({levelInfo.nlwTier} tier)";
        
        var pages = 
            levelsInfo.Count <= 1 ? 
                string.Empty : 
                $"Страница {page+1} из {levelsInfo.Count} |";
        
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"{pages} #{levelInfo.position} {levelInfo.name} {tier} {verificationLink}");
    }

    private static async Task Hardest(ChatCmdArgs cmdArgs) {
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var chatCommands = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var client = cmdArgs.Bot.GetClient();
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }
                    
        if (cmdArgs.Parsed.Count < 1) {
            var levels = await AredlUtils.ListLevels();
            if (levels == null || levels.data?.Count < 1) {
                if (chatCommands.Options.VerboseState == State.Enabled) {
                    ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
                }
                return;
            }
            var level = levels.data?[0];
            var details = await demonList.GetLevelDetails(level?.id!);
            if (details == null || details.verifications.Count < 1) {
                if (chatCommands.Options.VerboseState == State.Enabled) {
                    ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
                }
                return;
            }
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{level?.position} {level?.name} | {details.verifications[0].videoUrl}");
            return;
        }
        
        var argSb = new StringBuilder();
        foreach (var arg in cmdArgs.Parsed) {
            argSb.Append($"{arg} ");
        }
        var username = argSb.ToString();
                    
        var profile = await demonList.GetProfile(username.Trim());
        if (profile == null) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, "Пользователь не найден.");
            return;
        }
        var hardest = await demonList.GetHardest(profile);
        if (hardest == null) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, "Хардест не найден.");
            return;
        }
                        
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{hardest.level.position} {profile.hardest?.name} | {hardest.videoUrl}");
    }

    private static async Task Easiest(ChatCmdArgs cmdArgs) {
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var chatCommands = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var client = cmdArgs.Bot.GetClient();
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client); 
            return;
        }
                    
        if (cmdArgs.Parsed.Count < 1) {
            var levels = await AredlUtils.ListLevels();
            if (levels == null || levels.data?.Count < 1) {
                if (chatCommands.Options.VerboseState == State.Enabled) {
                    client?.SendReply(chatMessage.Channel, chatMessage.Id, "Что-то пошло не так.");
                }
                return;
            }
            var level = levels.data?[^1];
            var i = 2;
            while (level!.legacy) {
                level = levels.data?[^i++];
            }
            var details = await demonList.GetLevelDetails(level.id);
            if (details == null || details.verifications.Count < 1) {
                if (chatCommands.Options.VerboseState == State.Enabled) {
                    client?.SendReply(chatMessage.Channel, chatMessage.Id, "Не найдено.");
                }
                return;
            }
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{level.position} {level.name} | {details.verifications[0].videoUrl}");
            return;
        }

        var argSb = new StringBuilder();
        foreach (var arg in cmdArgs.Parsed) {
            argSb.Append($"{arg} ");
        }
        var username = argSb.ToString();

        var profile = await demonList.GetProfile(username.Trim());
        if (profile == null) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, "Пользователь не найден.");
            return;
        }
        var easiest = await demonList.GetEasiest(profile);
        if (easiest == null) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, "Не найдено.");
            return;
        }

        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{easiest.level.position} {easiest.level.name} | {easiest.videoUrl}");
    }

    private static async Task Roulette(ChatCmdArgs cmdArgs) {
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var client = cmdArgs.Bot.GetClient();
        
        var from = -1;
        var to = -1;

        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (cmdArgs.Parsed.Count > 0) {
            from = int.Parse(cmdArgs.Parsed[0]);
            if (cmdArgs.Parsed.Count > 1) {
                to = int.Parse(cmdArgs.Parsed[1]);
            }
        }
                        
        var levelInfo = await demonList.GetRandomLevel(from, to);
        if (levelInfo == null) {
            ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
            return;
        }
        var verificationLink = await demonList.GetLevelVerificationLink(levelInfo.id);
        if (verificationLink != null) {
            verificationLink = $"| {verificationLink}";
        }
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{levelInfo.position} {levelInfo.name} {verificationLink}");
    }

    private static async Task Clan(ChatCmdArgs cmdArgs) {
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var client = cmdArgs.Bot.GetClient();
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }
        if (cmdArgs.Parsed.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
                        
        var clanInfo = await demonList.GetClanInfo(cmdArgs.Parsed[0]);
        if (clanInfo == null) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Клана не существует.");
            return;
        }
                        
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"[{clanInfo.clan.tag}] {clanInfo.clan.globalName} | https://aredl.net/clans/{clanInfo.clan.id}");
    }
    
    private static async Task ClanHardest(ChatCmdArgs cmdArgs) {
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var client = cmdArgs.Bot.GetClient();

        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (cmdArgs.Parsed.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
                    
        var clanInfo = await demonList.GetClanInfo(cmdArgs.Parsed[0]);
        if (clanInfo == null) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Клана не существует.");
            return;
        }
        var hardest = await demonList.GetLevelDetails(clanInfo.hardest.id);
        var verificationLink = string.Empty;
        if (hardest?.verifications.Count > 0) {
            verificationLink = $"| {hardest.verifications[0].videoUrl}";
        }
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{hardest?.position} {clanInfo.hardest.name} {verificationLink}");
    }

    private static async Task ClanRoulette(ChatCmdArgs cmdArgs) {
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var chatMessage = cmdArgs.Args.Command.ChatMessage;
        var client = cmdArgs.Bot.GetClient();
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (cmdArgs.Parsed.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
                    
        var clanInfo = await demonList.GetClanInfo(cmdArgs.Parsed[0]);
        if (clanInfo == null) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Клана не существует.");
            return;
        }
        var levelInfo = await demonList.GetRandomClanSubmission(clanInfo.clan.id)!;
        if (levelInfo == null) {
            ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
            return;
        }
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{levelInfo.level?.position} {levelInfo.level?.name} | {levelInfo.videoUrl}");
    }
    
    private static Task PageTerminator(ChatCmdArgs cmdArgs) {
        return Task.CompletedTask;
    }
    
    private static void SendPagedReply(List<string> reply, ChatCmdArgs cmdArgs) {
        var client = cmdArgs.Bot.GetClient();
        
        var page = 0;
        if (cmdArgs.Parsed.Count > 0) {
            int.TryParse(cmdArgs.Parsed[0], out page);
        }

        var pages = Page.CalculatePages(reply);

        if (page < pages[0]) {
            page = pages[0];
        } else if (page > pages[^1]) {
            page = pages[^1];
        }

        var message = new StringBuilder();
        var pageTerminatorsCount = 0;
        
        message.Append($"Страница {page} из {pages[^1]} | ");
        
        for (var i = 0; i < reply.Count; i++) {
            if (reply[i] == Page.pageTerminator) {
                pageTerminatorsCount++;
                continue;
            }
            if (pages[i-pageTerminatorsCount] == page) {
                message.Append($"{reply[i]} ");
            }
        }
        
        client?.SendReply(cmdArgs.Args.Command.ChatMessage.Channel, cmdArgs.Args.Command.ChatMessage.Id, message.ToString());
    }
}