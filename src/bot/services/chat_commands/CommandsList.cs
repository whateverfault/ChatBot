﻿using System.Text;
using ChatBot.api.twitch.client;
using ChatBot.api.twitch.helix;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.ai;
using ChatBot.bot.services.chat_ads;
using ChatBot.bot.services.chat_ads.Data;
using ChatBot.bot.services.chat_commands.Data;
using ChatBot.bot.services.demon_list;
using ChatBot.bot.services.game_requests;
using ChatBot.bot.services.level_requests;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.message_randomizer;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.telegram;
using ChatBot.bot.services.text_generator;
using ChatBot.bot.services.translator;
using ChatBot.bot.shared;
using ChatBot.bot.shared.handlers;
using MessageState = ChatBot.bot.services.message_randomizer.MessageState;

namespace ChatBot.bot.services.chat_commands;

public static class CommandsList {
    private static readonly ChatCommandsService _chatCmds = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private static readonly TwitchChatBot _bot = TwitchChatBot.Instance; 
    
    public static List<DefaultChatCommand> DefaultsCommands { get; }


    static CommandsList() {
        // 57
        DefaultsCommands = [
                               new DefaultChatCommand(
                                                   1,
                                                   "help",
                                                   string.Empty,
                                                   "использование комманд.",
                                                   Help,
                                                   Restriction.Everyone
                                                  ),
                               new DefaultChatCommand(
                                                      0,
                                                   "cmds",
                                                   "[page]",
                                                   "список комманд.",
                                                   Cmds,
                                                   Restriction.Everyone
                                                  ),
                               new DefaultChatCommand(
                                                      47,
                                                      "more",
                                                      "[page]",
                                                      "список дополнительных комманд.",
                                                      More,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      2,
                                                      "followage",
                                                      "[username]",
                                                      "время, которое пользователь отслеживает канал.",
                                                      Followage,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      -1,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      3,
                                                      "title",
                                                      string.Empty, 
                                                      "посмотреть название стрима.",
                                                      TitleEveryone,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      4,
                                                      "title",
                                                      "[title]",
                                                      "посмотреть/изменить название стрима.",
                                                      Title,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      5,
                                                      "game",
                                                      string.Empty,
                                                      "посмотреть категорию стрима.",
                                                      GameEveryone,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      6,
                                                      "game",
                                                      "[game]",
                                                      "изменить категорию стрима.",
                                                      Game,
                                                      Restriction.DevMod
                                                     ),
                               new DefaultChatCommand(
                                                      7,
                                                      "clip",
                                                      string.Empty,
                                                      "создать клип.",
                                                      Clip,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      8,
                                                      "req",
                                                      string.Empty, 
                                                      "узнать включены ли реквесты",
                                                      ReqEveryone,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      -1,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      9,
                                                      "rizz",
                                                      "[text]",
                                                      "RIZZ",
                                                      Rizz,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      10,
                                                      "when",
                                                      "[text]",
                                                      "Waiting",
                                                      When,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      11,
                                                      "ban",
                                                      "[text]",
                                                      "SillyJail",
                                                      Ban,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      -1,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      12,
                                                      "ai",
                                                      "[prompt]",
                                                      "задать вопрос ии.",
                                                      Ai,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      13,
                                                      "potato",
                                                      string.Empty,
                                                      "сгенерировать сообщение",
                                                      Potato,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      14,
                                                      "carrot",
                                                      string.Empty,
                                                      "зарандомить новое сообщение",
                                                      Carrot,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      -1,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      15,
                                                      "req",
                                                      "[on/points/off]",
                                                      "включить/выключить реквесты",
                                                      Req,
                                                      Restriction.DevMod
                                                     ),
                               new DefaultChatCommand(
                                                      16,
                                                      "set-req-reward",
                                                      string.Empty, 
                                                      "установить награду для реквестов.",
                                                      SetReqReward,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      17,
                                                      "verbose",
                                                      "[on/off]",
                                                      "включить/выключить/узнать включены ли дополнительные логи",
                                                      Verbose,
                                                      Restriction.DevMod
                                                     ),
                               new DefaultChatCommand(
                                                      -1,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      18,
                                                      "guess",
                                                      "[username]",
                                                      "угадать ник написавшего.",
                                                      Guess,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      19,
                                                      "whose",
                                                      string.Empty,
                                                      "узнать ник написавшего.",
                                                      Whose,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      20,
                                                      "repeat",
                                                      string.Empty,
                                                      "повторить сообщение.",
                                                      Repeat,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      -1,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      21,
                                                      "translate",
                                                      "<text> [--lang lang_code --source_lang lang_code]",
                                                      "перевести текст.",
                                                      Translate,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      22,
                                                      "detect-lang",
                                                      "<text>",
                                                      "узнать язык, на котором написан текст.",
                                                      DetectLang,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      23,
                                                      "lang",
                                                      "[lang_code]",
                                                      "установить дефолтный язык переводчика.",
                                                      Lang,
                                                      Restriction.Vip
                                                     ),
                               new DefaultChatCommand(
                                                      24,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      25,
                                                      "top",
                                                      "<position>",
                                                      "уровень стоящий на данной позиции по AREDL.",
                                                      Top,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      26,
                                                      "place",
                                                      "<level_name> [by creator_name] [--page page_count]",
                                                      "позиция уровня по AREDL.",
                                                      Place,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      27,
                                                      "roulette",
                                                      "<from> <to>",
                                                      "зарандомить экстрим.",
                                                      Roulette,
                                                      Restriction.Everyone,
                                                      aliases: ["rulet",]
                                                     ),
                               new DefaultChatCommand(
                                                      -1,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      28,
                                                      "hardest",
                                                      "[username]",
                                                      "хардест пользователя по AREDL.",
                                                      Hardest,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      29,
                                                      "easiest",
                                                      "[username]",
                                                      "легчайший экстрим пользователя по AREDL.",
                                                      Easiest,
                                                      Restriction.Everyone,
                                                      aliases: ["lowest",]
                                                     ),
                               new DefaultChatCommand(
                                                      30,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      31,
                                                      "ptop",
                                                      "<position>",
                                                      "уровень стоящий на данной позиции по Pemon List.",
                                                      Ptop,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      32,
                                                      "pplace",
                                                      "<level> [by creator_name] [--page page_count]",
                                                      "позиция уровня по Pemon List.",
                                                      Pplace,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      -1,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      33,
                                                      "clan",
                                                      "<clan_tag>",
                                                      "Информация о клане.",
                                                      Clan,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      34,
                                                      "clan-hardest",
                                                      "<clan_tag>",
                                                      "хардест клана.",
                                                      ClanHardest,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      35,
                                                      "clan-roulette",
                                                      "<clan_tag>",
                                                      "случайный пройденный кланом уровень.",
                                                      ClanRoulette,
                                                      Restriction.Everyone,
                                                      aliases: ["clan-rulet",]
                                                     ),
                               new DefaultChatCommand(
                                                      -1,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      36,
                                                      "games",
                                                      "[page]",
                                                      "список заказов игр.",
                                                      Games,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      37,
                                                      "add-game",
                                                      "<game_name> [--position number] [--user username]",
                                                      "добавить игру в очередь.",
                                                      AddGame,
                                                      Restriction.Broadcaster
                                                     ),
                               new DefaultChatCommand(
                                                      38,
                                                      "complete",
                                                      "<game_index>",
                                                      "удалить игру из очереди.",
                                                      CompleteGame,
                                                      Restriction.Broadcaster
                                                     ),
                               new DefaultChatCommand(
                                                      39,
                                                      "reset-games",
                                                      string.Empty,
                                                      "очистить очередь заказов игр.",
                                                      ResetGames,
                                                      Restriction.Broadcaster,
                                                      aliases: ["nuke-games",]
                                                     ),
                               new DefaultChatCommand(
                                                      40,
                                                      "add-game-reqs-reward",
                                                      string.Empty,
                                                      "добавить награду для заказа игр.",
                                                      AddGameRequestsReward,
                                                      Restriction.DevBroad
                                                      ),
                               new DefaultChatCommand(
                                                      41,
                                                      "reset-game-reqs-rewards",
                                                      string.Empty,
                                                      "очистить список наград для заказа игр.",
                                                      ResetGameRequestsRewards,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      -1,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      42,
                                                      "create-reward",
                                                      "<title;cost;> [is_input_required (true/false)]",
                                                      "создать награду.",
                                                      CreateReward,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      43,
                                                      "delete-reward",
                                                      "<reward_id>",
                                                      "удалить награду.",
                                                      DeleteReward,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      -1,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      44,
                                                      "tg-notify",
                                                      string.Empty,
                                                      "узнать включены ли уведомления.",
                                                      TgNotifyEveryone,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      45,
                                                      "tg-notify",
                                                      "[enable/disable]",
                                                      "включить/выключить уведомления.",
                                                      TgNotify,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      46,
                                                      "tg-notify-prompt",
                                                      "<notification_prompt>",
                                                      "изменить текст уведомления.",
                                                      TgNotifyPrompt,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      -1,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      48,
                                                      "list-cmds",
                                                      "[page]",
                                                      "список команд с их ID.",
                                                      ListCustomCmds,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      49,
                                                      "add-cmd",
                                                      "<name>;<output>;[has_identifier true/false]",
                                                      "добавить команду.",
                                                      AddCmd,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      50,
                                                      "remove-cmd",
                                                      "<id>",
                                                      "удалить команду.",
                                                      RemoveCmd,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      51,
                                                      "change-cmd-desc",
                                                      "<id>;<new_desc>",
                                                      "изменить описание команды.",
                                                      ChangeCmdDescription,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      52,
                                                      "change-cmd-output",
                                                      "<id>;<new_output>",
                                                      "изменить вывод команды.",
                                                      ChangeCmdOutput,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      -1,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      53,
                                                      "list-chat-ads",
                                                      "[--page number]",
                                                      "список чат-реклам.",
                                                      ListChatAds,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      54,
                                                      "add-chat-ad",
                                                      "<name>;<output>;<cooldown>(in secs)",
                                                      "добавить чат-рекламу.",
                                                      AddChatAd,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      55,
                                                      "remove-chat-ad",
                                                      "<id>",
                                                      "удалить чат-рекламу.",
                                                      RemoveChatAd,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      56,
                                                      "change-chat-ad-name",
                                                      "<id>;<new_name>",
                                                      "изменить название чат-рекламы.",
                                                      ChangeChatAdName,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      57,
                                                      "change-chat-ad-output",
                                                      "<id>;<new_output>",
                                                      "изменить описание чат-рекламы.",
                                                      ChangeChatAdOutput,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      58,
                                                      "change-chat-ad-cooldown",
                                                      "<id>;<new_cooldown>(in secs)",
                                                      "изменить перезарядку чат-рекламы.",
                                                      ChangeChatAdCooldown,
                                                      Restriction.DevBroad
                                                     ),
                           ];
    }
    
    public static void SetDefaults() {
        _chatCmds.Options.SetDefaultCmds(DefaultsCommands);
    }

    private static async Task Cmds(ChatCmdArgs cmdArgs) {
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var cmdId = cmdArgs.Parsed.CommandIdentifier;
        var index = 1;
        var cmds = new List<string>();

        foreach (var cmd in _chatCmds.Options.DefaultCmds) {
            if (!chatMessage.Fits(cmd.Restriction)) continue;
            if (_chatCmds.Options.DefaultCmds
                         .Any(defaultCmd =>
                                  defaultCmd.Name.Equals(cmd.Name)
                               && defaultCmd.Restriction < cmd.Restriction
                               && chatMessage.Fits(defaultCmd.Restriction))){
                continue;
            }
            
            if (string.IsNullOrEmpty(cmd.Name)) {
                if (cmds[^1].Equals(Page.PageTerminator)) continue;
                cmds.Add(Page.PageTerminator);
                index = 1;
                continue;
            }
            
            var desc = string.IsNullOrEmpty(cmd.Args) || cmd.Description.Equals("--") ? 
                           string.Empty :
                           $"- {cmd.Description}";

            var args = string.IsNullOrEmpty(cmd.Args) || cmd.Args.Equals("--") ?
                           string.Empty :
                           cmd.Args;
            
            cmds.Add($"{index}. {cmdId}{cmd.Name} {args} {desc} ");
            index++;
        }
        
        
        await SendPagedReply(cmds, cmdArgs, _chatCmds.Options.SendWhisperIfPossible == State.Enabled);
    }
    
    private static async Task More(ChatCmdArgs cmdArgs) {
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var cmdId = cmdArgs.Parsed.CommandIdentifier;
        
        var index = 1;
        var cmds = new List<string>();
        
        foreach (var cmd in _chatCmds.Options.CustomCmds) {
            if (!chatMessage.Fits(cmd.Restriction)) continue;
            if (_chatCmds.Options.CustomCmds
                         .Any(customCmd =>
                                  customCmd.Name.Equals(cmd.Name)
                               && customCmd.Restriction < cmd.Restriction
                               && chatMessage.Fits(customCmd.Restriction))){
                continue;
            }
            
            if (string.IsNullOrEmpty(cmd.Name)) {
                if (cmds[^1].Equals(Page.PageTerminator)) continue;
                cmds.Add(Page.PageTerminator);
                index = 1;
                continue;
            }

            if (!cmd.HasIdentifier) {
                continue;
            }
            
            var desc = string.IsNullOrEmpty(cmd.Args) || cmd.Description.Equals("--") ? 
                           string.Empty :
                           $"- {cmd.Description}";

            var args = string.IsNullOrEmpty(cmd.Args) || cmd.Args.Equals("--") ?
                           string.Empty :
                           cmd.Args;
            
            cmds.Add($"{index}. {cmdId}{cmd.Name} {args} {desc} ");
            index++;
        }
        
        await SendPagedReply(cmds, cmdArgs, _chatCmds.Options.SendWhisperIfPossible == State.Enabled);
    }
    
    private static async Task Help(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatCommands = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var cmdId = cmdArgs.Parsed.CommandIdentifier;
        
        var usage = $"{cmdId}<комманда> \"аргумент1\" \"аргумент2\" ... | {cmdId}{_chatCmds.Options.DefaultCmds[0].Name} для списка комманд";
        
        switch (chatCommands.Options.SendWhisperIfPossible) {
            case State.Disabled: {
                await client.SendReply(chatMessage.Id, usage);
                break;
            }
            case State.Enabled: {
                var result = await Helix.SendWhisper(cmdArgs.Parsed.ChatMessage.UserId, usage, client.Credentials,
                                                          (_, message) => {
                                                              _logger.Log(LogLevel.Error, message);
                                                          });
                if (!result) {
                    ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
                }
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

        private static async Task When(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        
        if (args.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var argSb = new StringBuilder();
                
        foreach (var arg in args) {
            argSb.Append($"{arg} ");
        }

        var random = Random.Shared.Next(0, 2);
        var randomizedMessage = 
            random == 0 ? 
                $"{argSb} уже завтра! PewPewPew PewPewPew PewPewPew" : 
                $"{argSb} никогда GAGAGA GAGAGA GAGAGA";
        await client.SendReply(chatMessage.Id, randomizedMessage);
    }

    private static async Task Ban(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        
        if (args.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var argSb = new StringBuilder();
                
        foreach (var arg in args) {
            argSb.Append($"{arg} ");
        }
                
        await client.SendReply(chatMessage.Id, $"{argSb} отправлен в бан sillyJAIL sillyJAIL sillyJAIL");
    }

    private static async Task Ai(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var ai = (AiService)ServiceManager.GetService(ServiceName.Ai);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        
        if (args.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var prompt = new StringBuilder();
        foreach (var word in args) {
            prompt.Append($"{word} ");
        }

        var response = await ai.GetResponse(prompt.ToString().Trim());
        
        if (string.IsNullOrEmpty(response)) {
            ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
            return;
        }

        const int chunkSize = 450;
        var remainingLenght = response.Length;
        var position = 0;
        var iteration = 0;
        var messages = new List<string>();
        var sb = new StringBuilder();
        
        do {
            sb.Clear();
            if (iteration++ > 0) sb.Append("...");
            var start = position;
            var end = Math.Clamp(position+chunkSize, 0, response.Length);
            sb.Append(response[start..end]);
            
            remainingLenght -= sb.Length;
            position += sb.Length;
            
            if (remainingLenght > 0) {
                sb.Append("...");
            }
            
            messages.Add(sb.ToString());
        } while (remainingLenght > 0);

        foreach (var message in messages) {
            await client.SendReply(chatMessage.Id, message);
        }
    }

    private static async Task Verbose(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatCommands = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        
        var verboseStateStr = 
            chatCommands.Options.VerboseState == State.Enabled?
                "включены" :
                "отключены";
        var comment =
            chatCommands.Options.VerboseState == State.Enabled ?
                "Shiza":
                "ZACHTO";
        
        switch (args.Count) {
            case < 1:
                await client.SendReply(chatMessage.Id, $"Дополнительные логи {verboseStateStr} {comment}");
                return;
            case > 0: {
                if (args[0] == "on") {
                    chatCommands.Options.SetVerboseState(State.Enabled);
                }
                if (args[0] == "off") {
                    chatCommands.Options.SetVerboseState(State.Disabled);
                }

                break;
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
        await client.SendReply(chatMessage.Id, $"Дополнительные логи теперь {verboseStateStr} {comment}");
    }

    private static async Task ReqEveryone(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var levelRequests = (LevelRequestsService)ServiceManager.GetService(ServiceName.LevelRequests);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var reqState = levelRequests.GetReqState();
        await client.SendReply(chatMessage.Id, $"Реквесты {levelRequests.GetReqStateStr(reqState)}"); 
    }
    
    private static async Task Req(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var levelRequests = (LevelRequestsService)ServiceManager.GetService(ServiceName.LevelRequests);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (levelRequests.GetServiceState() == State.Disabled) return;
        
        var reqState = levelRequests.GetReqState();
        switch (args.Count) {
            case <= 0:
                if (_chatCmds.Options.VerboseState == State.Enabled) {
                    await client.SendReply(chatMessage.Id, "Недостаточно аргументов для изменения состояния.");
                }
                await ReqEveryone(cmdArgs);
                return;
            case > 0:
                switch (args[0]) {
                    case "off": {
                        reqState = ReqState.Off;
                        await Helix.SetChannelRewardState(levelRequests.GetRewardId(), false, client.Credentials, 
                                                          (_, message) => { 
                                                              _logger.Log(LogLevel.Error, message);
                                                          });
                        break;
                    }
                    case "points": {
                        reqState = ReqState.Points;
                        var result = await Helix.SetChannelRewardState(levelRequests.GetRewardId(), true, client.Credentials,
                                                                            (_, message) => {
                                                                                _logger.Log(LogLevel.Error, message);
                                                                            });
                        if (!result) {
                            ErrorHandler.ReplyWithError(ErrorCode.NoRewardSet, chatMessage, client);
                            return;
                        }
                        break;
                    }
                    case "on": {
                        reqState = ReqState.On;
                        await Helix.SetChannelRewardState(levelRequests.GetRewardId(), false, client.Credentials,
                                                          (_, message) => {
                                                              _logger.Log(LogLevel.Error, message);
                                                          });
                        break;
                    }
                }
                break;
        }

        levelRequests.Options.SetReqState(reqState);
        await client.SendReply(chatMessage.Id, $"Реквесты теперь {levelRequests.GetReqStateStr(reqState)}");
    }

    private static async Task SetReqReward(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var levelRequests = (LevelRequestsService)ServiceManager.GetService(ServiceName.LevelRequests);
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (chatMessage.RewardId == null) {
            await client.SendReply(chatMessage.Id, $"Используйте эту комманду внутри награды.");
            return;
        }
        
        levelRequests.SetRewardId(chatMessage.RewardId);
        await client.SendReply(chatMessage.Id, $"Награда для реквестов успешно установлена.");
    }
    
    private static Task Potato(ChatCmdArgs chatCmdArgs) {
        var textGenerator = (TextGeneratorService)ServiceManager.GetService(ServiceName.TextGenerator);
        
        textGenerator.GenerateAndSend();
        return Task.CompletedTask;
    }

    private static async Task Clip(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var clipId = await Helix.CreateClip(client.Credentials, (_, message) => {
                                                                         _logger.Log(LogLevel.Error, message);
                                                                     });

        if (clipId == null) {
            ErrorHandler.ReplyWithError(ErrorCode.ClipCreationFailed, chatMessage, client);
            return;
        }
        await client.SendReply(chatMessage.Id, $"Клип создан - https://www.twitch.tv/{client.Credentials.Channel}/clip/{clipId}");
    }
    
    private static Task Rizz(ChatCmdArgs cmdArgs) {
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var client = _bot.GetClient();
        
        var argSb = new StringBuilder();

        foreach (var arg in args) {
            argSb.Append($"{arg} ");
        }
                
        var message =
            args.Count == 0 ?
                "КШЯЯ" :
                argSb.ToString();
                
        client?.SendMessage($"{message} RIZZ RIZZ RIZZ");
        return Task.CompletedTask;
    }

    private static async Task TitleEveryone(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var channelInfo = await Helix.GetChannelInfo(client.Credentials, (_, message) => {
                                                                                  _logger.Log(LogLevel.Error, message);
                                                                              });
        
        await client.SendReply(chatMessage.Id, $"Название стрима - {channelInfo!.Title}");
    }
    
    private static async Task Title(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var baseTitle = ((ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands)).GetBaseTitle();
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
                    
        if (args.Count < 1) {
            await TitleEveryone(cmdArgs);
            return;
        }

        var channelInfo = await Helix.GetChannelInfo(client.Credentials, (_, message) => {
                                                                                  _logger.Log(LogLevel.Error, message);
                                                                              });
        if (channelInfo == null) return;
        
        var titleSb = new StringBuilder();
        titleSb.Append($"{baseTitle} ");
        foreach (var arg in args) {
            titleSb.Append($"{arg} ");
        }

        var result = await Helix.UpdateChannelInfo(titleSb.ToString(), channelInfo.GameId, client.Credentials, (_, message) => {
                                                            _logger.Log(LogLevel.Error, message);
                                                        });
        if (!result) {
            await client.SendReply(chatMessage.Id, $"Не удалось изменить название");
            return;
        }
        await client.SendReply(chatMessage.Id, $"Название стрима изменено на {titleSb}");
    }
    
    private static async Task Followage(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        string result;
        var username = chatMessage.Username;
        if (args.Count > 0) {
            username = args[0];
        }

        var followage = await Helix.GetFollowage(username, client.Credentials, (_, message) => {
                                                               _logger.Log(LogLevel.Error, message);
                                                           });
        if (followage == null) {
            if (args.Count > 0) {
                result = 
                    username == chatMessage.Channel ?
                             $"{username} это владелец канала RIZZ" :
                             $"{username} не фолловнут на {chatMessage.Channel} Sadding";
            } else {
                result = 
                    username == chatMessage.Channel ?
                        "Вы владелец канала RIZZ" :
                        $"Вы не фолловнуты на {chatMessage.Channel} Sadding";
            }

            await client.SendReply(chatMessage.Id, result);
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
        result = 
            args.Count > 0 ? 
                $"{username} фолловнут на {chatMessage.Channel} {years} {months} {days}" : 
                $"Вы фолловнуты на {chatMessage.Channel} {years} {months} {days}";
        await client.SendReply(chatMessage.Id, result);
    }

    private static async Task GameEveryone(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var channelInfo = await Helix.GetChannelInfo(client.Credentials, (_, message) => {
                                                                                  _logger.Log(LogLevel.Error, message);
                                                                              });
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        await client.SendReply(chatMessage.Id, $"Текущая категория - {channelInfo!.GameName}");
    }
    
    private static async Task Game(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (args.Count < 1) {
            await GameEveryone(cmdArgs);
            return;
        }
        
        var channelInfo = await Helix.GetChannelInfo(client.Credentials, (_, message) => {
                                                                                  _logger.Log(LogLevel.Error, message);
                                                                              });
        if (channelInfo == null) return;
        
        var gameSb = new StringBuilder();
        for (var i = 0; i < args.Count; i++) {
            if (i == args.Count-1) {
                gameSb.Append($"{args[i]}");
                break;
            }
            gameSb.Append($"{args[i]} ");
        }

        var gameId = await Helix.FindGameId(gameSb.ToString(), client.Credentials, (_, message) => {
                                                     _logger.Log(LogLevel.Error, message);
                                                 });
        if (gameId == null) return;
        
        var result = await Helix.UpdateChannelInfo(channelInfo.Title, gameId, client.Credentials, (_, message) => {
                                                            _logger.Log(LogLevel.Error, message);
                                                        });
        if (!result) {
            await client.SendReply(chatMessage.Id, "Не удалось изменить категорию");
            return;
        }
        channelInfo = await Helix.GetChannelInfo(client.Credentials, (_, message) => {
                                                                              _logger.Log(LogLevel.Error, message);
                                                                          });
        await client.SendReply(chatMessage.Id, $"Категория изменена на {channelInfo!.GameName}");
    }
    
    private static async Task Guess(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var messageRandomizer = (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (messageRandomizer.Options.MessageState == MessageState.Guessed) {
            await client.SendReply(chatMessage.Id, "Уже отгадано.");
            return;
        }

        if (args.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            logger.Log(LogLevel.Error, $"Too few arguments for '{cmdArgs.Parsed.CommandText}' command");
            return;
        }

        var err = messageRandomizer.GetLastGeneratedMessage(out var message);
        if (ErrorHandler.ReplyWithError(err, chatMessage, client)
         || message == null) {
            logger.Log(LogLevel.Info, "Tried to access last random message while there is no such.");
            return;
        }

        var userId = await Helix.GetUserId(args[0], client.Credentials, (_, msg) => {
                                               _logger.Log(LogLevel.Error, msg);
                                           });
        if (userId == null) return;
        
        if (userId != message.UserId) {
            await client.SendReply(chatMessage.Id, "Неправильно.");
        } else {
            await client.SendReply(chatMessage.Id,
                              $"Правильно, это было сообщение от {args[0]}.");
            messageRandomizer.Options.SetMessageState(MessageState.Guessed);
        }
    }

    private static async Task Whose(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var messageRandomizer = (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var err = messageRandomizer.GetLastGeneratedMessage(out var message);
        if (ErrorHandler.ReplyWithError(err, chatMessage, client)
         || message == null) {
            logger.Log(LogLevel.Info, "Tried to access last random message while there are no such.");
            return;
        }

        var username = await Helix.GetUsername(message.UserId, client.Credentials, true, (_, msg) => {
                                                   _logger.Log(LogLevel.Error, msg);
                                               });
        await client.SendReply(chatMessage.Id, $"Это было сообщение от '{username}'");
    }

    private static async Task Repeat(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var messageRandomizer = (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var err = messageRandomizer.GetLastGeneratedMessage(out var message);
        if (ErrorHandler.ReplyWithError(err, chatMessage, client)) {
            ErrorHandler.ReplyWithError(err, chatMessage, client);
            logger.Log(LogLevel.Info, "Tried to access last random message while there are no such.");
            return;
        }
        
        await client.SendReply(chatMessage.Id, message!.Text);
        logger.Log(LogLevel.Info, $"Repeated last message for {chatMessage.Username}.");
    }

    private static async Task Translate(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var translator = (TranslatorService)ServiceManager.GetService(ServiceName.Translator);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (translator.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }
        
        if (args.Count <= 0) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
        
        var textSb = new StringBuilder();
        foreach (var arg in args) {
            if (arg.Contains("--")) break;
            textSb.Append($"{arg} ");
        }

        var targetLang = string.Empty;
        var index = args.IndexOf("--lang");
        if (index >= 0 && index < args.Count-1) {
            targetLang = args[index+1];
        }
        
        var sourceLang = string.Empty;
        index = args.IndexOf("--source_lang");
        if (index >= 0 && index < args.Count-1) {
            sourceLang = args[index+1];
        }
        
        var translated = await translator.Translate(textSb.ToString(), targetLang, sourceLang);
        if (translated == null) {
            ErrorHandler.ReplyWithError(ErrorCode.TranslationFailed, chatMessage, client);
            return;
        }
        await client.SendReply(chatMessage.Id, $"Перевод: {translated}");
    }

    private static async Task DetectLang(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var translator = (TranslatorService)ServiceManager.GetService(ServiceName.Translator);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (translator.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }
        
        if (args.Count <= 0) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
        
        var textSb = new StringBuilder();
        foreach (var arg in args) {
            if (arg is null or "--") break;
            textSb.Append($"{arg} ");
        }
        
        var lang = await translator.DetectLanguage(textSb.ToString());
        if (lang == null) {
            ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
            return;
        }
        await client.SendReply(chatMessage.Id, $"Самый близкий язык: {lang.LanguageCode} с вероятностью {(int)(lang.Confidence*100)}%");
    }

    private static async Task Lang(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var translator = (TranslatorService)ServiceManager.GetService(ServiceName.Translator);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (translator.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }
        
        if (args.Count <= 0) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
        
        translator.SetTargetLanguage(args[0]);
        await client.SendReply(chatMessage.Id, $"Язык установлен на '{args[0]}'");
    }
    
    private static Task Carrot(ChatCmdArgs cmdArgs) {
        var messageRandomizer = (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        if (messageRandomizer.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            logger.Log(LogLevel.Error, "Message Randomizer service is disabled");
            return Task.CompletedTask;
        }
                    
        var err = messageRandomizer.Generate(out var message);
        if (ErrorHandler.ReplyWithError(err, chatMessage, client)
         || message == null) return Task.CompletedTask;
        
        client?.SendMessage(message.Text);
        return Task.CompletedTask;
    }

    private static async Task Top(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            logger.Log(LogLevel.Error, "Demon List service is disabled");
            return;
        }
                    
        var index = int.Parse(string.IsNullOrWhiteSpace(args[0])? "-1" : args[0]);
        var levelInfo =  await demonList.GetLevelByPlacement(index);
        if (levelInfo == null) {
            await client.SendReply(chatMessage.Id, "Позиция не найдена.");
            return;
        }
        var verificationLink = await demonList.GetLevelVerificationLink(levelInfo.Id);
        if (verificationLink != null) {
            verificationLink = $"| {verificationLink}";
        }
        
        var tier = 
            levelInfo.NlwTier == null? 
                string.Empty:
                $"({levelInfo.NlwTier} tier";
        
        var enjoyment = (levelInfo.EdelEnjoyment == null) switch {
                            true  => string.IsNullOrEmpty(tier) ? string.Empty : ")",
                            false => string.IsNullOrEmpty(tier) ? $"(EDL: {(int)levelInfo.EdelEnjoyment})" : $"; EDL: {(int)levelInfo.EdelEnjoyment})",
                        };
        
        await client.SendReply(chatMessage.Id, $"#{index} {levelInfo.Name} {tier}{enjoyment} {verificationLink}");
    }

    private static async Task Place(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (args.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var levelName = new StringBuilder();
        foreach (var arg in args) {
            if (arg is "by" or "от" or "--page") break;
            levelName.Append($"{arg} ");
        }

        var index = args.IndexOf("by");
        if (index != -1 && index+1 < args.Count) {
            var creator = args[index+1];
            levelName.Append($" ({creator})");
        }

        var levelsInfo =  await demonList.GetLevelsInfoByName(levelName.ToString().Trim());

        if (levelsInfo == null || levelsInfo.Count == 0) {
            await client.SendReply(chatMessage.Id, "Уровень не найден.");
            return;
        }
        
        var page = 0;
        index = args.IndexOf("--page");
        if (index != -1 && args.Count > index+1) {
            page = int.Parse(args[index+1])-1;
        }

        if (page < 0) page = 0;
        if (page >= levelsInfo.Count) page = levelsInfo.Count-1;
        
        var levelInfo = levelsInfo[page];
        var verificationLink = await demonList.GetLevelVerificationLink(levelInfo.Id);
        if (verificationLink != null) {
            verificationLink = $"| {verificationLink}";
        }
        
        var tier = 
            levelInfo.NlwTier == null? 
                string.Empty:
                $"({levelInfo.NlwTier} tier";

        var enjoyment = (levelInfo.EdelEnjoyment == null) switch {
                            true  => string.IsNullOrEmpty(tier) ? string.Empty : ")",
                            false => string.IsNullOrEmpty(tier) ? $"(EDL: {(int)levelInfo.EdelEnjoyment})" : $"; EDL: {(int)levelInfo.EdelEnjoyment})",
                        };

        var pages = 
            levelsInfo.Count <= 1 ? 
                string.Empty : 
                $"Страница {page+1} из {levelsInfo.Count} |";
        
        await client.SendReply(chatMessage.Id, $"{pages} #{levelInfo.Position} {levelInfo.Name} {tier}{enjoyment} {verificationLink}");
    }
    
    private static async Task Ptop(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            logger.Log(LogLevel.Error, "Demon List service is disabled");
            return;
        }
                    
        var index = int.Parse(string.IsNullOrWhiteSpace(args[0])? "-1" : args[0]);
        var levelInfo =  await demonList.GetPlatformerLevelByPlacement(index);
        if (levelInfo == null) {
            await client.SendReply(chatMessage.Id, "Позиция не найдена.");
            return;
        }
        var verificationLink = await demonList.GetPlatformerLevelVerificationLink(levelInfo.Id);
        if (verificationLink != null) {
            verificationLink = $"| {verificationLink}";
        }
        
        var tier = 
            levelInfo.NlwTier == null? 
                string.Empty:
                $"({levelInfo.NlwTier} tier)";
        
        await client.SendReply(chatMessage.Id, $"#{index} {levelInfo.Name} {tier} {verificationLink}");
    }
    
    private static async Task Pplace(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;

        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (args.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var levelName = new StringBuilder();
        foreach (var arg in args) {
            if (arg is "by" or "от" or "--page") break;
            levelName.Append($"{arg} ");
        }

        var index = args.IndexOf("by");
        if (index != -1 && index+1 < args.Count) {
            var creator = args[index+1];
            levelName.Append($" ({creator})");
        }

        var levelsInfo =  await demonList.GetPlatformerLevelsInfoByName(levelName.ToString().Trim());

        if (levelsInfo == null) {
            await client.SendReply(chatMessage.Id, "Уровень не найден.");
            return;
        }
        
        var page = 0;
        index = cmdArgs.Parsed.CommandMessage.IndexOf("--page", StringComparison.Ordinal);
        if (index != -1 && args.Count > index+1) {
            page = int.Parse(args[index+1])-1;
        }

        if (page < 0) page = 0;
        if (page >= levelsInfo.Count) page = levelsInfo.Count-1;
        
        var levelInfo = levelsInfo[page];
        var verificationLink = await demonList.GetPlatformerLevelVerificationLink(levelInfo.Id);
        if (verificationLink != null) {
            verificationLink = $"| {verificationLink}";
        }
        
        var tier = 
            levelInfo.NlwTier == null? 
                string.Empty:
                $"({levelInfo.NlwTier} tier)";
        
        var pages = 
            levelsInfo.Count <= 1 ? 
                string.Empty : 
                $"Страница {page+1} из {levelsInfo.Count} |";
        
        await client.SendReply(chatMessage.Id, $"{pages} #{levelInfo.Position} {levelInfo.Name} {tier} {verificationLink}");
    }

    private static async Task Hardest(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var chatCommands = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }
                    
        if (args.Count < 1) {
            var level = await demonList.GetHardest();
            if (level == null) {
                if (chatCommands.Options.VerboseState == State.Enabled) {
                    ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
                }
                return;
            }
            
            var details = await demonList.GetLevelDetails(level.Id);
            if (details == null || details.verifications.Count < 1) {
                if (chatCommands.Options.VerboseState == State.Enabled) {
                    ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
                }
                return;
            }
            await client.SendReply(chatMessage.Id, $"#{level.Position} {level.Name} | {details.verifications[0].videoUrl}");
            return;
        }
        
        var argSb = new StringBuilder();
        foreach (var arg in args) {
            argSb.Append($"{arg} ");
        }
        var username = argSb.ToString();
                    
        var profile = await demonList.GetProfile(username.Trim());
        if (profile == null) {
            await client.SendReply(chatMessage.Id, "Пользователь не найден.");
            return;
        }
        var hardest = await demonList.GetHardest(profile);
        if (hardest == null) {
            await client.SendReply(chatMessage.Id, "Хардест не найден.");
            return;
        }
                        
        await client.SendReply(chatMessage.Id, $"#{hardest.level.position} {profile.hardest?.name} | {hardest.videoUrl}");
    }

    private static async Task Easiest(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var chatCommands = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client); 
            return;
        }
                    
        if (args.Count < 1) {
            var level = await demonList.GetEasiest();
            if (level == null) {
                if (chatCommands.Options.VerboseState == State.Enabled) {
                    ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
                }
                return;
            }
            
            var details = await demonList.GetLevelDetails(level.Id);
            if (details == null || details.verifications.Count < 1) {
                if (chatCommands.Options.VerboseState == State.Enabled) {
                    await client.SendReply(chatMessage.Id, "Не найдено.");
                }
                return;
            }
            await client.SendReply(chatMessage.Id, $"#{level.Position} {level.Name} | {details.verifications[0].videoUrl}");
            return;
        }

        var argSb = new StringBuilder();
        foreach (var arg in args) {
            argSb.Append($"{arg} ");
        }
        var username = argSb.ToString();

        var profile = await demonList.GetProfile(username.Trim());
        if (profile == null) {
            await client.SendReply(chatMessage.Id, "Пользователь не найден.");
            return;
        }
        var easiest = await demonList.GetEasiest(profile);
        if (easiest == null) {
            await client.SendReply(chatMessage.Id, "Не найдено.");
            return;
        }

        await client.SendReply(chatMessage.Id, $"#{easiest.level.position} {easiest.level.name} | {easiest.videoUrl}");
    }

    private static async Task Roulette(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var from = -1;
        var to = -1;

        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (args.Count > 0) {
            from = int.Parse(args[0]);
            if (args.Count > 1) {
                to = int.Parse(args[1]);
            }
        }
                        
        var levelInfo = await demonList.GetRandomLevel(from, to);
        if (levelInfo == null) {
            ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
            return;
        }
        var verificationLink = await demonList.GetLevelVerificationLink(levelInfo.Id);
        if (verificationLink != null) {
            verificationLink = $"| {verificationLink}";
        }
        await client.SendReply(chatMessage.Id, $"#{levelInfo.Position} {levelInfo.Name} {verificationLink}");
    }

    private static async Task Clan(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }
        if (args.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
                        
        var clanInfo = await demonList.GetClanInfo(args[0]);
        if (clanInfo == null) {
            await client.SendReply(chatMessage.Id, "Клана не существует.");
            return;
        }
                        
        await client.SendReply(chatMessage.Id, $"[{clanInfo.clan.tag}] {clanInfo.clan.globalName} | https://aredl.net/clans/{clanInfo.clan.id}");
    }
    
    private static async Task ClanHardest(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (args.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
                    
        var clanInfo = await demonList.GetClanInfo(args[0]);
        if (clanInfo == null) {
            await client.SendReply(chatMessage.Id, $"Клана не существует.");
            return;
        }
        var hardest = await demonList.GetLevelDetails(clanInfo.hardest.id);
        var verificationLink = string.Empty;
        if (hardest?.verifications.Count > 0) {
            verificationLink = $"| {hardest.verifications[0].videoUrl}";
        }
        await client.SendReply(chatMessage.Id, $"#{hardest?.position} {clanInfo.hardest.name} {verificationLink}");
    }

    private static async Task ClanRoulette(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (args.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var clanInfo = await demonList.GetClanInfo(args[0]);
        if (clanInfo == null) {
            await client.SendReply(chatMessage.Id, $"Клана не существует.");
            return;
        }
        var levelInfo = await demonList.GetRandomClanSubmission(clanInfo.clan.id)!;
        if (levelInfo == null) {
            ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
            return;
        }
        await client.SendReply(chatMessage.Id, $"#{levelInfo.level?.position} {levelInfo.level?.name} | {levelInfo.videoUrl}");
    }

    private static async Task Games(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var gameRequestService = (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests);
        var chatMessage = cmdArgs.Parsed.ChatMessage;


        var gameRequests = gameRequestService.GetGameRequests();

        if (gameRequests?.Count <= 0) {
            ErrorHandler.ReplyWithError(ErrorCode.ListIsEmpty, chatMessage, client) ;
            return;
        }
        
        var reply = new List<string>();
        for (var i = 0; i < gameRequests?.Count; i++) {
            var gameRequest = gameRequests[i];
            var separator = "/";
            
            if (i >= gameRequests.Count-1) {
                separator = string.Empty;
            }

            var username = await Helix.GetUsername(gameRequest.UserId, client.Credentials, true, (_, message) => {
                                                     _logger.Log(LogLevel.Error, message);
                                                 });
            reply.Add($"{i+1}. {gameRequest.GameName} -> {username} {separator}");
        }

        await SendPagedReply(reply, cmdArgs);
    }

    private static async Task AddGameRequestsReward(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var gameRequest = (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests);
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (chatMessage.RewardId == null) {
            await client.SendReply(chatMessage.Id, $"Используйте эту комманду внутри награды.");
            return;
        }
        
        gameRequest.Options.AddReward(chatMessage.RewardId);
        await client.SendReply(chatMessage.Id, "Награда добавлена в список.");
    }

    private static async Task ResetGameRequestsRewards(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var gameRequest = (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        gameRequest.Options.ResetRewards();
        await client.SendReply(chatMessage.Id, "Список наград очищен.");
    }
    
    private static Task AddGame(ChatCmdArgs cmdArgs) {
        var gameRequestService = (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        if (args.Count > 0) {
            return gameRequestService.AddGameRequest(args, chatMessage);
        }
        
        ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
        return Task.CompletedTask;
    }

    private static async Task CompleteGame(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var gameRequestService = (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests);
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var error = ParseIntArg(cmdArgs, out var indexToRemove);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }

        var gameRequests = gameRequestService.GetGameRequests();
        
        if (indexToRemove <= 0 || indexToRemove > gameRequests!.Count) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        indexToRemove--;
        var gameName = gameRequests[indexToRemove].GameName;
        
        gameRequestService.Options.RemoveRequest(indexToRemove);
        await client.SendReply(chatMessage.Id, $"Игра {gameName} удалена из очереди.");
    }
    
    private static async Task ResetGames(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var gameRequestService = (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests);
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        gameRequestService.Options.ResetRequests();
        await client.SendReply(chatMessage.Id, "Список заказов очищен.");
    }

    private static async Task CreateReward(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }

        if (args.Length < 2) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
        
        var title = args[0];
        var cost = int.Parse(args[1]);
        var requireInput = false;

        if (args.Length > 2) {
            if (string.Equals(args[2], "true", StringComparison.InvariantCultureIgnoreCase)) {
                requireInput = true;
            } else if (string.Equals(args[2], "false", StringComparison.InvariantCultureIgnoreCase)) {
                requireInput = false;
            }
        }

        var rewardId = await Helix.CreateChannelReward(title, cost, client.Credentials, userInputRequired: requireInput, callback: (_, message) => { 
                                                                _logger.Log(LogLevel.Error, message); 
                                                            });
        if (rewardId == null) {
            ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
            return;
        }
        
        await client.SendReply(chatMessage.Id, $"Награда успешно создана. ({rewardId})");
    }
    
    private static async Task DeleteReward(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (args.Count <= 0) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var rewardId = args[0];
        var result = await Helix.DeleteChannelReward(rewardId, client.Credentials, (_, message) => {
                                                              _logger.Log(LogLevel.Error, message);
                                                          });
        if (!result) {
            ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
            return;
        }
        
        await client.SendReply(chatMessage.Id, $"Награда успешно удалена. ({rewardId})");
    }
    
    private static async Task TgNotifyEveryone(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var tgNotifications = (TgNotificationsService)ServiceManager.GetService(ServiceName.TgNotifications);
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var stateStr =
            tgNotifications.GetServiceState() == State.Enabled? 
                "включены":
                "отключены";
        await client.SendReply(chatMessage.Id, $"Уведомления о стримах {stateStr}");
    }
    
    private static async Task TgNotify(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var tgNotifications = (TgNotificationsService)ServiceManager.GetService(ServiceName.TgNotifications);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (args.Count <= 0) {
            await TgNotifyEveryone(cmdArgs);
            return;
        }

        switch (args[0]) {
            case "enable": {
                tgNotifications.Options.SetState(State.Enabled);
                break;
            }
            case "disable": {
                tgNotifications.Options.SetState(State.Disabled);
                break;
            }
        }
        
        var stateStr =
            tgNotifications.GetServiceState() == State.Enabled? 
                "включены":
                "отключены";
        await client.SendReply(chatMessage.Id, $"Уведомления о стримах теперь {stateStr}");
    }
    
    private static async Task TgNotifyPrompt(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var tgNotifications = (TgNotificationsService)ServiceManager.GetService(ServiceName.TgNotifications);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (args.Count <= 0) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
        
        var prompt = new StringBuilder();
        foreach (var arg in args) {
            prompt.Append($"{arg} ");
        }
        
        tgNotifications.SetNotificationPrompt(prompt.ToString().Trim());
        await client.SendReply(chatMessage.Id, $"Текст уведомлений успешно изменен.");
    }

    private static async Task ListCustomCmds(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var cmds = _chatCmds.Options.GetCustomCommands();
        if (cmds.Count == 0) {
            ErrorHandler.ReplyWithError(ErrorCode.ListIsEmpty, chatMessage, client);
            return;
        }
        
        var list = cmds.Select((cmd, i) => $"{i + 1}. {cmd.Name} (id: {cmd.Id}) ").ToList();
        await SendPagedReply(list, cmdArgs, _chatCmds.Options.SendWhisperIfPossible == State.Enabled);
    }
    
    private static async Task AddCmd(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }
        
        if (args.Length < 2) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var id = _chatCmds.GetAvailableCustomCmdId();
        var cmdName = args[0];
        var cmdPrompt = args[1];
        var hasIdentifier = true;
        
        if (args.Length > 2) {
            if (args[2].Equals("false")) {
                hasIdentifier = false;
            }
        }
        
        var cmd = new CustomChatCommand(id,
                                        cmdName,
                                        string.Empty,
                                        string.Empty,
                                        hasIdentifier,
                                        [],
                                        cmdPrompt,
                                        Restriction.Everyone);
        
        _chatCmds.AddChatCmd(cmd);
        await client.SendReply(chatMessage.Id, $"Новая команда '{cmd.Name}' добавлена успешно.");
    }

    private static async Task RemoveCmd(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var error = ParseIntArg(cmdArgs, out var id);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }

        var cmd = _chatCmds.Options.GetCustomCmdById(id);
        if (cmd == null || !_chatCmds.Options.TryRemoveChatCmdById(id)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        await client.SendReply(chatMessage.Id, $"Команда '{cmd.Name}' успешно удалена.");
    }
    
    private static async Task ChangeCmdDescription(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }
        
        if (!int.TryParse(args[0], out var id)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        var cmd = _chatCmds.Options.GetCustomCmdById(id);

        if (cmd == null) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }
        
        cmd.SetDescription(args[1]);
        await client.SendReply(chatMessage.Id, $"Описание команды '{cmd.Name}' изменено на {cmd.Description}.");
    }
    
    private static async Task ChangeCmdOutput(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }
        
        if (args.Length < 2) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
        
        if (!int.TryParse(args[0], out var id)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        var cmd = _chatCmds.Options.GetCustomCmdById(id);

        if (cmd == null) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }
        
        cmd.SetOutput(args[1]);
        await client.SendReply(chatMessage.Id, $"Вывод команды '{cmd.Name}' изменено на {cmd.Output}.");
    }

    private static async Task ListChatAds(ChatCmdArgs cmdArgs) {
        var chatAdsService = (ChatAdsService)ServiceManager.GetService(ServiceName.ChatAds);
        var chatAds = chatAdsService.Options.GetChatAds();
        
        var output = new List<string>();
        for (var i = 0; i < chatAds.Count; i++) {
            var chatAd = chatAds[i];
            output.Add($"{i+1}. {chatAd.GetName()} (cooldown: {chatAd.GetCooldown()}) ");
        }

        await SendPagedReply(output, cmdArgs);
    }

    private static async Task AddChatAd(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatAdsService = (ChatAdsService)ServiceManager.GetService(ServiceName.ChatAds);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }
        
        if (args.Length < 3) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var name = args[0];
        var output = args[1];
        
        if (!long.TryParse(args[2], out var cooldown)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        var chatAd = new ChatAd(name, output, cooldown);
        chatAdsService.AddChatAd(chatAd);

        await client.SendReply(chatMessage.Id, $"Чат-реклама с названием {chatAd.GetName()} добавлена успешно.");
    }
    
    private static async Task RemoveChatAd(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatAdsService = (ChatAdsService)ServiceManager.GetService(ServiceName.ChatAds);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var error = ParseIntArg(cmdArgs, out var indexToRemove);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }
        --indexToRemove;
        
        var name = chatAdsService.Options.GetChatAds()[indexToRemove].GetName();
        var result = chatAdsService.RemoveChatAd(indexToRemove);
        if (!result) {
            await client.SendReply(chatMessage.Id, $"Не удалось удалить чат-рекламу с айди {indexToRemove+1}.");
            return;
        }
        
        await client.SendReply(chatMessage.Id, $"Чат-реклама с названием {name} удалена успешно.");
    }

    private static async Task ChangeChatAdName(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatAdsService = (ChatAdsService)ServiceManager.GetService(ServiceName.ChatAds);
        var chatAds = chatAdsService.Options.GetChatAds();
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }

        if (args.Length < 2
         || !int.TryParse(args[0], out var index)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        --index;
        if (index < 0 || index >= chatAds.Count) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        var chatAd = chatAds[index];

        var oldName = chatAd.GetName();
        chatAd.SetName(args[1]);
        
        await client.SendReply(chatMessage.Id, $"Название чат-рекламы {oldName} изменено на {chatAd.GetName()}.");
    }
    
    private static async Task ChangeChatAdOutput(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatAdsService = (ChatAdsService)ServiceManager.GetService(ServiceName.ChatAds);
        var chatAds = chatAdsService.Options.GetChatAds();
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }

        if (args.Length < 2
         || !int.TryParse(args[0], out var index)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        --index;
        if (index < 0
         || index >= chatAds.Count) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        var chatAd = chatAds[index];
        chatAd.SetOutput(args[1]);
        
        await client.SendReply(chatMessage.Id, $"Вывод чат-рекламы {chatAd.GetName()} изменен на {chatAd.GetOutput()}.");
    }
    
    private static async Task ChangeChatAdCooldown(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatAdsService = (ChatAdsService)ServiceManager.GetService(ServiceName.ChatAds);
        var chatAds = chatAdsService.Options.GetChatAds();
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }

        if (args.Length < 2
         || !int.TryParse(args[0], out var index)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        --index;
        if (index < 0 
         || index >= chatAds.Count 
         || !int.TryParse(args[1], out var cooldown)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        var chatAd = chatAds[index];
        chatAd.SetCooldown(cooldown);
        
        await client.SendReply(chatMessage.Id, $"Перезарядка чат-рекламы {chatAd.GetName()} изменена на {chatAd.GetCooldown()}.");
    }
    
    private static Task PageTerminator(ChatCmdArgs cmdArgs) {
        return Task.CompletedTask;
    }
    
    private static async Task SendPagedReply(List<string> reply, ChatCmdArgs cmdArgs, bool whisper = false) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (reply.Count <= 0) {
            ErrorHandler.ReplyWithError(ErrorCode.ListIsEmpty, chatMessage, client);
            return;
        }
        
        var page = 0;
        var index = args.IndexOf("--page");
        if (index >= 0 && index < args.Count-1) {
            int.TryParse(args[index+1], out page);
        }

        if (args.Count > 0 
         && index < 0 
         && int.TryParse(args[0], out var tempPage)) {
            page = tempPage;
        }
        
        var pages = Page.CalculatePages(reply);

        if (page < pages[0]) {
            page = pages[0];
        } else if (page > pages[^1]) {
            page = pages[^1];
        }

        var message = new StringBuilder();
        var pageTerminatorsCount = 0;

        if (pages[^1] > 1) { 
            message.Append($"Страница {page} из {pages[^1]} | ");
        }
        
        for (var i = 0; i < reply.Count; i++) {
            if (reply[i] == Page.PageTerminator) {
                pageTerminatorsCount++;
                continue;
            }
            if (pages[i-pageTerminatorsCount] == page) {
                message.Append($"{reply[i].Trim()} ");
            }
        }

        switch (whisper) {
            case false: {
                await client.SendReply(chatMessage.Id, message.ToString());
                break;
            }
            case true: {
                var result = await Helix.SendWhisper(chatMessage.UserId, message.ToString(), client.Credentials, (_, callback) => {
                                                              _logger.Log(LogLevel.Error, callback);
                                                          });
                if (!result) {
                    ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
                }
                break;
            }
        }
    }

    private static ErrorCode ParseIntArg(ChatCmdArgs cmdArgs, out int value) {
        var args = cmdArgs.Parsed.ArgumentsAsList;
        value = -1;
        
        if (args.Count <= 0) {
            return ErrorCode.TooFewArgs;
        }

        return int.TryParse(args[0], out value)?
                   ErrorCode.None :
                   ErrorCode.InvalidInput;
    }
    
    private static ErrorCode ParseSemicolonSeparatedArgs(ChatCmdArgs cmdArgs, out string[] parsed) {
        var args = cmdArgs.Parsed.ArgumentsAsList;
        parsed = [];
        
        if (args.Count <= 0) {
            return ErrorCode.TooFewArgs;
        }

        var sb = new StringBuilder();
        foreach (var arg in args) {
            sb.Append($"{arg} ");
        }
        
        parsed = sb.ToString().Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return ErrorCode.None;
    }
}