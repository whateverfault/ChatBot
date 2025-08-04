using System.Text;
using ChatBot.bot;
using ChatBot.bot.@interface;
using ChatBot.services.ai;
using ChatBot.services.chat_ads;
using ChatBot.services.chat_ads.Data;
using ChatBot.services.chat_commands.Data;
using ChatBot.services.demon_list;
using ChatBot.services.game_requests;
using ChatBot.services.level_requests;
using ChatBot.services.logger;
using ChatBot.services.message_randomizer;
using ChatBot.services.Static;
using ChatBot.services.telegram;
using ChatBot.services.text_generator;
using ChatBot.services.translator;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;
using ChatBot.utils.GD.AREDL;
using ChatBot.utils.Twitch.Helix;
using MessageState = ChatBot.services.message_randomizer.MessageState;

namespace ChatBot.services.chat_commands;

public static class CommandsList {
    private static readonly ChatCommandsService _chatCmds = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
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
                                                      "<name>;<output>;<cooldown>",
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
                                                      "change-cmd-cooldown",
                                                      "<id>;<new_cooldown>",
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
            if (!RestrictionHandler.Handle(cmd.Restriction, chatMessage)) continue;
            if (_chatCmds.Options.DefaultCmds
                         .Any(defaultCmd =>
                                  defaultCmd.Name.Equals(cmd.Name)
                               && defaultCmd.Restriction < cmd.Restriction
                               && RestrictionHandler.Handle(defaultCmd.Restriction, chatMessage))){
                continue;
            }
            
            if (string.IsNullOrEmpty(cmd.Name)) {
                if (cmds[^1].Equals(Page.pageTerminator)) continue;
                cmds.Add(Page.pageTerminator);
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
            if (!RestrictionHandler.Handle(cmd.Restriction, chatMessage)) continue;
            if (_chatCmds.Options.CustomCmds
                         .Any(customCmd =>
                                  customCmd.Name.Equals(cmd.Name)
                               && customCmd.Restriction < cmd.Restriction
                               && RestrictionHandler.Handle(customCmd.Restriction, chatMessage))){
                continue;
            }
            
            if (string.IsNullOrEmpty(cmd.Name)) {
                if (cmds[^1].Equals(Page.pageTerminator)) continue;
                cmds.Add(Page.pageTerminator);
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
        var chatCommands = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var cmdId = cmdArgs.Parsed.CommandIdentifier;
        
        var usage = $"{cmdId}<комманда> \"аргумент1\" \"аргумент2\" ... | {cmdId}{_chatCmds.Options.DefaultCmds[0].Name} для списка комманд";

        switch (chatCommands.Options.SendWhisperIfPossible) {
            case State.Disabled: {
                client?.SendReply(chatMessage.Channel, chatMessage.Id, usage);
                break;
            }
            case State.Enabled: {
                var result = await HelixUtils.SendWhisper(_bot.Options, chatMessage.UserId, usage);
                if (!result) {
                    ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
                }
                break;
            }
        }
    }

        private static Task When(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        
        if (args.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return Task.CompletedTask;
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
        client?.SendReply(chatMessage.Channel, chatMessage.Id, randomizedMessage);
        return Task.CompletedTask;
    }

    private static Task Ban(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        
        if (args.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return Task.CompletedTask;
        }

        var argSb = new StringBuilder();
                
        foreach (var arg in args) {
            argSb.Append($"{arg} ");
        }
                
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"{argSb} отправлен в бан sillyJAIL sillyJAIL sillyJAIL");
        return Task.CompletedTask;
    }

    private static async Task Ai(ChatCmdArgs cmdArgs) {
        var ai = (AiService)ServiceManager.GetService(ServiceName.Ai);
        var client = _bot.GetClient();
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
            client?.SendReply(chatMessage.Channel, chatMessage.Id, message);
        }
    }

    private static Task Verbose(ChatCmdArgs cmdArgs) {
        var chatCommands = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
        var client = _bot.GetClient();
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
        if (!RestrictionHandler.Handle(Restriction.DevMod, chatMessage) || args.Count < 1) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Дополнительные логи {verboseStateStr} {comment}");
            return Task.CompletedTask;
        }

        if (args.Count > 0) {
            if (args[0] == "on") {
                chatCommands.Options.SetVerboseState(State.Enabled);
            }
            if (args[0] == "off") {
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

    private static Task ReqEveryone(ChatCmdArgs cmdArgs) {
        var levelRequests = (LevelRequestsService)ServiceManager.GetService(ServiceName.LevelRequests);
        var client = _bot.GetClient();
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var reqState = levelRequests.GetReqState();
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Реквесты {levelRequests.GetReqStateStr(reqState)}"); 
        return Task.CompletedTask;
    }
    
    private static async Task Req(ChatCmdArgs cmdArgs) {
        var levelRequests = (LevelRequestsService)ServiceManager.GetService(ServiceName.LevelRequests);
        var chatCommands = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        if (levelRequests.GetServiceState() == State.Disabled) return;
        
        var reqState = levelRequests.GetReqState();
        switch (args.Count) {
            case <= 0:
                if (chatCommands.Options.VerboseState == State.Enabled) {
                    client?.SendReply(chatMessage.Channel, chatMessage.Id, "Недостаточно аргументов для изменения состояния.");
                }
                await ReqEveryone(cmdArgs);
                return;
            case > 0:
                switch (args[0]) {
                    case "off": {
                        reqState = ReqState.Off;
                        var result = await HelixUtils.SetChannelRewardState(_bot.Options, levelRequests.GetRewardId(), false);
                        if (!result) {
                            ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
                            return;
                        }
                        
                        break;
                    }
                    case "points": {
                        reqState = ReqState.Points;
                        var result = await HelixUtils.SetChannelRewardState(_bot.Options, levelRequests.GetRewardId(), true);
                        if (!result) {
                            ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
                            return;
                        }
                        break;
                    }
                    case "on": {
                        reqState = ReqState.On;
                        var result = await HelixUtils.SetChannelRewardState(_bot.Options, levelRequests.GetRewardId(), false);
                        if (!result) {
                            ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
                            return;
                        }
                        break;
                    }
                }
                break;
        }

        levelRequests.Options.SetReqState(reqState);
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Реквесты теперь {levelRequests.GetReqStateStr(reqState)}");
    }

    private static Task SetReqReward(ChatCmdArgs cmdArgs) {
        var levelRequests = (LevelRequestsService)ServiceManager.GetService(ServiceName.LevelRequests);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        if (chatMessage.CustomRewardId == null) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Используйте эту комманду внутри награды.");
            logger.Log(LogLevel.Warning, "This command must be used within the reward.");
            return Task.CompletedTask;
        }
        
        levelRequests.SetRewardId(chatMessage.CustomRewardId);
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Награда для реквестов успешно установлена.");
        logger.Log(LogLevel.Info, $"Successfully assigned new Reward Id ({chatMessage.CustomRewardId}).");
        return Task.CompletedTask;
    }
    
    private static Task Potato(ChatCmdArgs chatCmdArgs) {
        var textGenerator = (TextGeneratorService)ServiceManager.GetService(ServiceName.TextGenerator);
        
        textGenerator.GenerateAndSend();
        return Task.CompletedTask;
    }

    private static async Task Clip(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var clipId = await HelixUtils.CreateClip(_bot.Options);

        if (clipId == null) {
            ErrorHandler.ReplyWithError(ErrorCode.ClipCreationFailed, chatMessage, client);
            return;
        }
        client?.SendReply(_bot.Options.Channel!, chatMessage.Id, $"Клип создан - https://www.twitch.tv/{_bot.Options.Channel}/clip/{clipId}");
    }
    
    private static Task Rizz(ChatCmdArgs cmdArgs) {
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        var argSb = new StringBuilder();

        foreach (var arg in args) {
            argSb.Append($"{arg} ");
        }
                
        var message =
            args.Count == 0 ?
                "КШЯЯ" :
                argSb.ToString();
                
        client?.SendMessage(chatMessage.Channel, $"{message} RIZZ RIZZ RIZZ");
        return Task.CompletedTask;
    }

    private static async Task TitleEveryone(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var channelInfo = await HelixUtils.GetChannelInfo(_bot.Options);
        
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Название стрима - {channelInfo!.Title}");
    }
    
    private static async Task Title(ChatCmdArgs cmdArgs) {
        var baseTitle = ((ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands)).GetBaseTitle();
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
                    
        if (args.Count < 1) {
            await TitleEveryone(cmdArgs);
            return;
        }

        var channelInfo = await HelixUtils.GetChannelInfo(_bot.Options);
        var titleSb = new StringBuilder();
        
        titleSb.Append($"{baseTitle} ");
        foreach (var arg in args) {
            titleSb.Append($"{arg} ");
        }

        var result = await HelixUtils.UpdateChannelInfo(_bot.Options, titleSb.ToString(), channelInfo!.GameId);
        if (!result) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Не удалось изменить название");
            return;
        }
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Название стрима изменено на {titleSb}");
    }
    
    private static async Task Followage(ChatCmdArgs cmdArgs) {
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient(); 
        
        string message;
        var username = chatMessage.Username;
        if (args.Count > 0) {
            username = args[0];
        }

        var followage = await HelixUtils.GetFollowageHelix(_bot.Options, username!);
        if (followage == null) {
            if (args.Count > 0) {
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
            args.Count > 0 ? 
                $"{username} фолловнут на {chatMessage.Channel} {years} {months} {days}" : 
                $"Вы фолловнуты на {chatMessage.Channel} {years} {months} {days}";
        client?.SendReply(chatMessage.Channel, chatMessage.Id, message);
    }

    private static async Task GameEveryone(ChatCmdArgs cmdArgs) {
        var channelInfo = await HelixUtils.GetChannelInfo(_bot.Options);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Текущая категория - {channelInfo!.GameName}");
    }
    
    private static async Task Game(ChatCmdArgs cmdArgs) {
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        if (args.Count < 1) {
            await GameEveryone(cmdArgs);
            return;
        }
        
        var channelInfo = await HelixUtils.GetChannelInfo(_bot.Options);
        var gameSb = new StringBuilder();

        for (var i = 0; i < args.Count; i++) {
            if (i == args.Count-1) {
                gameSb.Append($"{args[i]}");
                break;
            }
            gameSb.Append($"{args[i]} ");
        }

        var gameId = await HelixUtils.FindGameId(_bot.Options, gameSb.ToString());
        var result = await HelixUtils.UpdateChannelInfo(_bot.Options, channelInfo!.Title, gameId!);
        if (!result || gameId == null) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, "Не удалось изменить категорию");
            return;
        }
        channelInfo = await HelixUtils.GetChannelInfo(_bot.Options);
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Категория изменена на {channelInfo!.GameName}");
    }
    
    private static Task Guess(ChatCmdArgs cmdArgs) {
        var messageRandomizer = (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        if (messageRandomizer.Options.MessageState == MessageState.Guessed) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, "Уже отгадано.");
            return Task.CompletedTask;
        }

        if (args.Count < 1) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            logger.Log(LogLevel.Error, $"Too few arguments for '{cmdArgs.Parsed.CommandText}' command");
            return Task.CompletedTask;
        }

        var err = messageRandomizer.GetLastGeneratedMessage(out var message);
        if (ErrorHandler.ReplyWithError(err, chatMessage, client)) {
            logger.Log(LogLevel.Info, "Tried to access last random message while there is no such.");
            return Task.CompletedTask;
        }

        if (args[0] != message!.Username) {
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
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
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
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
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

    private static async Task Translate(ChatCmdArgs cmdArgs) {
        var translator = (TranslatorService)ServiceManager.GetService(ServiceName.Translator);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

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
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Перевод: {translated}");
    }

    private static async Task DetectLang(ChatCmdArgs cmdArgs) {
        var translator = (TranslatorService)ServiceManager.GetService(ServiceName.Translator);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

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
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Самый близкий язык: {lang.LanguageCode} с вероятностью {(int)(lang.Confidence*100)}%");
    }

    private static Task Lang(ChatCmdArgs cmdArgs) {
        var translator = (TranslatorService)ServiceManager.GetService(ServiceName.Translator);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        if (translator.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return Task.CompletedTask;
        }
        
        if (args.Count <= 0) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return Task.CompletedTask;
        }
        
        translator.SetTargetLanguage(args[0]);
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Язык установлен на '{args[0]}'");
        return Task.CompletedTask;
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
                    
        messageRandomizer.GenerateAndSend(client, chatMessage.Channel);
        return Task.CompletedTask;
    }

    private static async Task Top(ChatCmdArgs cmdArgs) {
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            logger.Log(LogLevel.Error, "Demon List service is disabled");
            return;
        }
                    
        var index = int.Parse(string.IsNullOrWhiteSpace(args[0])? "-1" : args[0]);
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
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

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
            client?.SendReply(chatMessage.Channel, chatMessage.Id, "Уровень не найден.");
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
        var verificationLink = await demonList.GetLevelVerificationLink(levelInfo.id);
        if (verificationLink != null) {
            verificationLink = $"| {verificationLink}";
        }
        
        var tier = 
            levelInfo.nlwTier == null? 
                string.Empty:
                $"({levelInfo.nlwTier} tier";

        var enjoyment = (levelInfo.edelEnjoyment == null) switch {
                            true  => string.IsNullOrEmpty(tier) ? string.Empty : ")",
                            false => string.IsNullOrEmpty(tier) ? $"(EDL: {(int)levelInfo.edelEnjoyment})" : $"; EDL: {(int)levelInfo.edelEnjoyment})",
                        };

        var pages = 
            levelsInfo.Count <= 1 ? 
                string.Empty : 
                $"Страница {page+1} из {levelsInfo.Count} |";
        
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"{pages} #{levelInfo.position} {levelInfo.name} {tier}{enjoyment} {verificationLink}");
    }
    
    private static async Task Ptop(ChatCmdArgs cmdArgs) {
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            logger.Log(LogLevel.Error, "Demon List service is disabled");
            return;
        }
                    
        var index = int.Parse(string.IsNullOrWhiteSpace(args[0])? "-1" : args[0]);
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
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var client = _bot.GetClient();

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
            client?.SendReply(chatMessage.Channel, chatMessage.Id, "Уровень не найден.");
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
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }
                    
        if (args.Count < 1) {
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
        foreach (var arg in args) {
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
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        if (demonList.GetServiceState() == State.Disabled) {
            ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client); 
            return;
        }
                    
        if (args.Count < 1) {
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
        foreach (var arg in args) {
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
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
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
        var verificationLink = await demonList.GetLevelVerificationLink(levelInfo.id);
        if (verificationLink != null) {
            verificationLink = $"| {verificationLink}";
        }
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"#{levelInfo.position} {levelInfo.name} {verificationLink}");
    }

    private static async Task Clan(ChatCmdArgs cmdArgs) {
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
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
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Клана не существует.");
            return;
        }
                        
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"[{clanInfo.clan.tag}] {clanInfo.clan.globalName} | https://aredl.net/clans/{clanInfo.clan.id}");
    }
    
    private static async Task ClanHardest(ChatCmdArgs cmdArgs) {
        var demonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

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
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
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

    private static Task Games(ChatCmdArgs cmdArgs) {
        var gameRequestService = (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();


        var gameRequests = gameRequestService.GetGameRequests();

        if (gameRequests?.Count <= 0) {
            ErrorHandler.ReplyWithError(ErrorCode.ListIsEmpty, chatMessage, client) ;
            return Task.CompletedTask;
        }
        
        var reply = new List<string>();
        for (var i = 0; i < gameRequests?.Count; i++) {
            var gameRequest = gameRequests[i];
            var separator = "/";
            
            if (i >= gameRequests.Count-1) {
                separator = string.Empty;
            }
            
            reply.Add($"{i+1}. {gameRequest.GameName} -> {gameRequest.RequesterUsername} {separator}");
        }

        _ = SendPagedReply(reply, cmdArgs);
        return Task.CompletedTask;
    }

    private static Task AddGameRequestsReward(ChatCmdArgs cmdArgs) {
        var gameRequest = (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        if (chatMessage.CustomRewardId == null) {
            client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Используйте эту комманду внутри награды.");
            logger.Log(LogLevel.Warning, "This command must be used within the reward.");
            return Task.CompletedTask;
        }
        
        gameRequest.Options.AddReward(chatMessage.CustomRewardId);
        client?.SendReply(chatMessage.Channel, chatMessage.Id, "Награда добавлена в список.");
        logger.Log(LogLevel.Info, $"Successfully added new Reward Id ({chatMessage.CustomRewardId}).");
        return Task.CompletedTask;
    }

    private static Task ResetGameRequestsRewards(ChatCmdArgs cmdArgs) {
        var gameRequest = (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        gameRequest.Options.ResetRewards();
        client?.SendReply(chatMessage.Channel, chatMessage.Id, "Список наград очищен.");
        logger.Log(LogLevel.Info, "Successfully cleared Reward's list");
        return Task.CompletedTask;
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

    private static Task CompleteGame(ChatCmdArgs cmdArgs) {
        var gameRequestService = (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        var error = ParseIntArg(cmdArgs, out var indexToRemove);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return Task.CompletedTask;
        }

        var gameRequests = gameRequestService.GetGameRequests();
        
        if (indexToRemove <= 0 || indexToRemove > gameRequests!.Count) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }

        indexToRemove--;
        var gameName = gameRequests[indexToRemove].GameName;
        
        gameRequestService.Options.RemoveRequest(indexToRemove);
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Игра {gameName} удалена из очереди.");
        logger.Log(LogLevel.Info, $"Successfully removed {gameName} out of the queue.");
        return Task.CompletedTask;
    }
    
    private static Task ResetGames(ChatCmdArgs cmdArgs) {
        var gameRequestService = (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests);
        var logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        gameRequestService.Options.ResetRequests();
        client?.SendReply(chatMessage.Channel, chatMessage.Id, "Список заказов очищен.");
        logger.Log(LogLevel.Info, "Successfully cleared Game Request's list");
        return Task.CompletedTask;
    }

    private static async Task CreateReward(ChatCmdArgs cmdArgs) {
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

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

        var rewardId = await HelixUtils.CreateChannelReward(_bot.Options, title, cost, userInputRequired: requireInput);
        if (rewardId == null) {
            ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
            return;
        }
        
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Награда успешно создана. ({rewardId})");
    }
    
    private static async Task DeleteReward(ChatCmdArgs cmdArgs) {
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        if (args.Count <= 0) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var rewardId = args[0];
        var result = await HelixUtils.DeleteChannelReward(_bot.Options, rewardId);
        if (!result) {
            ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
            return;
        }
        
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Награда успешно удалена. ({rewardId})");
    }
    
    private static Task TgNotifyEveryone(ChatCmdArgs cmdArgs) {
        var tgNotifications = (TgNotificationsService)ServiceManager.GetService(ServiceName.TgNotifications);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        var stateStr =
            tgNotifications.GetServiceState() == State.Enabled? 
                "включены":
                "отключены";
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Уведомления о стримах {stateStr}");
        return Task.CompletedTask;
    }
    
    private static Task TgNotify(ChatCmdArgs cmdArgs) {
        var tgNotifications = (TgNotificationsService)ServiceManager.GetService(ServiceName.TgNotifications);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        if (args.Count <= 0) {
            return TgNotifyEveryone(cmdArgs);
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
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Уведомления о стримах теперь {stateStr}");
        return Task.CompletedTask;
    }
    
    private static Task TgNotifyPrompt(ChatCmdArgs cmdArgs) {
        var tgNotifications = (TgNotificationsService)ServiceManager.GetService(ServiceName.TgNotifications);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        if (args.Count <= 0) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return Task.CompletedTask;
        }
        
        var prompt = new StringBuilder();
        foreach (var arg in args) {
            prompt.Append($"{arg} ");
        }
        
        tgNotifications.SetNotificationPrompt(prompt.ToString().Trim());
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Текст уведомлений успешно изменен.");
        return Task.CompletedTask;
    }

    private static Task ListCustomCmds(ChatCmdArgs cmdArgs) {
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        var cmds = _chatCmds.Options.GetCustomCommands();
        if (cmds.Count == 0) {
            ErrorHandler.ReplyWithError(ErrorCode.ListIsEmpty, chatMessage, client);
            return Task.CompletedTask;
        }
        
        var list = cmds.Select((cmd, i) => $"{i + 1}. {cmd.Name} (id: {cmd.Id}) ").ToList();
        return SendPagedReply(list, cmdArgs, _chatCmds.Options.SendWhisperIfPossible == State.Enabled);
    }
    
    private static Task AddCmd(ChatCmdArgs cmdArgs) {
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return Task.CompletedTask;
        }
        
        if (args.Length < 2) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return Task.CompletedTask;
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
        
        _chatCmds.Options.AddChatCmd(cmd);
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Новая команда '{cmd.Name}' добавлена успешно.");
        return Task.CompletedTask;
    }

    private static Task RemoveCmd(ChatCmdArgs cmdArgs) {
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        var error = ParseIntArg(cmdArgs, out var id);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return Task.CompletedTask;
        }

        var cmd = _chatCmds.Options.GetCustomCmdById(id);
        if (cmd == null || !_chatCmds.Options.TryRemoveChatCmdById(id)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }

        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Команда '{cmd.Name}' успешно удалена.");
        return Task.CompletedTask;
    }
    
    private static Task ChangeCmdDescription(ChatCmdArgs cmdArgs) {
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return Task.CompletedTask;
        }
        
        if (!int.TryParse(args[0], out var id)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }

        var cmd = _chatCmds.Options.GetCustomCmdById(id);

        if (cmd == null) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }
        
        cmd.SetDescription(args[1]);
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Описание команды '{cmd.Name}' изменено на {cmd.Description}.");
        return Task.CompletedTask;
    }
    
    private static Task ChangeCmdOutput(ChatCmdArgs cmdArgs) {
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return Task.CompletedTask;
        }
        
        if (args.Length < 2) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return Task.CompletedTask;
        }
        
        if (!int.TryParse(args[0], out var id)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }

        var cmd = _chatCmds.Options.GetCustomCmdById(id);

        if (cmd == null) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }
        
        cmd.SetOutput(args[1]);
        client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Вывод команды '{cmd.Name}' изменено на {cmd.Output}.");
        return Task.CompletedTask;
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

    private static Task AddChatAd(ChatCmdArgs cmdArgs) {
        var chatAdsService = (ChatAdsService)ServiceManager.GetService(ServiceName.ChatAds);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return Task.CompletedTask;
        }
        
        if (args.Length < 3) {
            ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return Task.CompletedTask;
        }

        var name = args[0];
        var output = args[1];
        
        if (!long.TryParse(args[2], out var cooldown)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }

        var chatAd = new ChatAd(name, output, cooldown);
        chatAdsService.Options.AddChatAd(chatAd);

        return Task.CompletedTask;
    }
    
    private static Task RemoveChatAd(ChatCmdArgs cmdArgs) {
        var chatAdsService = (ChatAdsService)ServiceManager.GetService(ServiceName.ChatAds);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        var error = ParseIntArg(cmdArgs, out var indexToRemove);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return Task.CompletedTask;
        }

        chatAdsService.Options.RemoveChatAd(indexToRemove);
        return Task.CompletedTask;
    }

    private static Task ChangeChatAdName(ChatCmdArgs cmdArgs) {
        var chatAdsService = (ChatAdsService)ServiceManager.GetService(ServiceName.ChatAds);
        var chatAds = chatAdsService.Options.GetChatAds();
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return Task.CompletedTask;
        }

        if (args.Length < 2) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }
        
        if (!int.TryParse(args[0], out var index)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }

        --index;
        if (index < 0 || index >= chatAds.Count) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }

        var chatAd = chatAds[index];
        chatAd.SetName(args[1]);
        return Task.CompletedTask;
    }
    
    private static Task ChangeChatAdOutput(ChatCmdArgs cmdArgs) {
        var chatAdsService = (ChatAdsService)ServiceManager.GetService(ServiceName.ChatAds);
        var chatAds = chatAdsService.Options.GetChatAds();
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return Task.CompletedTask;
        }

        if (args.Length < 2) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }
        
        if (!int.TryParse(args[0], out var index)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }

        --index;
        if (index < 0 || index >= chatAds.Count) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }

        var chatAd = chatAds[index];
        chatAd.SetOutput(args[1]);
        return Task.CompletedTask;
    }
    
    private static Task ChangeChatAdCooldown(ChatCmdArgs cmdArgs) {
        var chatAdsService = (ChatAdsService)ServiceManager.GetService(ServiceName.ChatAds);
        var chatAds = chatAdsService.Options.GetChatAds();
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return Task.CompletedTask;
        }

        if (args.Length < 2) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }
        
        if (!int.TryParse(args[0], out var index)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }

        --index;
        if (index < 0 || index >= chatAds.Count) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }
        
        if (!int.TryParse(args[0], out var cooldown)) {
            ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return Task.CompletedTask;
        }

        var chatAd = chatAds[index];
        chatAd.SetCooldown(cooldown);
        return Task.CompletedTask;
    }
    
    private static Task PageTerminator(ChatCmdArgs cmdArgs) {
        return Task.CompletedTask;
    }
    
    private static async Task SendPagedReply(List<string> reply, ChatCmdArgs cmdArgs, bool whisper = false) {
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();

        if (reply.Count <= 0) {
            ErrorHandler.ReplyWithError(ErrorCode.ListIsEmpty, chatMessage, client);
            return;
        }
        
        var page = 0;
        var index = args.IndexOf("--page");
        if (index >= 0 && index < args.Count-1) {
            int.TryParse(args[index+1], out page);
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
            if (reply[i] == Page.pageTerminator) {
                pageTerminatorsCount++;
                continue;
            }
            if (pages[i-pageTerminatorsCount] == page) {
                message.Append($"{reply[i].Trim()} ");
            }
        }

        switch (whisper) {
            case false: {
                client?.SendReply(chatMessage.Channel, chatMessage.Id, message.ToString());
                break;
            }
            case true: {
                var result = await HelixUtils.SendWhisper(_bot.Options, chatMessage.UserId, message.ToString());
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
        
        parsed = sb.ToString().Split(';', StringSplitOptions.TrimEntries);
        return ErrorCode.None;
    }
}