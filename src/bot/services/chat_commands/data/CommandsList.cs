using System.Text;
using ChatBot.bot.chat_bot;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.ai;
using ChatBot.bot.services.bank;
using ChatBot.bot.services.casino;
using ChatBot.bot.services.chat_ads;
using ChatBot.bot.services.chat_ads.data;
using ChatBot.bot.services.demon_list;
using ChatBot.bot.services.game_requests;
using ChatBot.bot.services.interpreter;
using ChatBot.bot.services.level_requests;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.message_randomizer;
using ChatBot.bot.services.shop;
using ChatBot.bot.services.shop.data;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.telegram_notifications;
using ChatBot.bot.services.text_generator;
using ChatBot.bot.services.translator;
using ChatBot.bot.shared;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;
using TwitchAPI.client.data;
using TwitchAPI.shared;
using MessageState = ChatBot.bot.services.message_randomizer.MessageState;

namespace ChatBot.bot.services.chat_commands.data;

public static class CommandsList {
    private const int PAGE_TERMINATOR_CMD_ID = -1;
        
    private static readonly ChatCommandsService _chatCmds = (ChatCommandsService)Services.Get(ServiceId.ChatCommands);
    private static readonly LoggerService _logger = (LoggerService)Services.Get(ServiceId.Logger);
    private static readonly TwitchChatBot _bot = TwitchChatBot.Instance; 
    
    public static List<DefaultChatCommand> DefaultsCommands { get; }


    static CommandsList() {
        DefaultsCommands = [
                               new DefaultChatCommand(
                                                      75,
                                                      "ping",
                                                      string.Empty,
                                                      "пинг.",
                                                      Ping,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                   1,
                                                   "help",
                                                   "[cmd_name]",
                                                   "использование и описание комманд.",
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
                                                      8,
                                                      "req",
                                                      string.Empty, 
                                                      "узнать включены ли реквесты",
                                                      ReqEveryone,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      PAGE_TERMINATOR_CMD_ID,
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
                                                      Restriction.DevMod
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
                                                      PAGE_TERMINATOR_CMD_ID,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      10,
                                                      "when",
                                                      "[text]",
                                                      Get7TvEmote(Emote.Wait),
                                                      When,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      11,
                                                      "ban",
                                                      "[text]",
                                                      Get7TvEmote(Emote.Jail),
                                                      Ban,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      79,
                                                      "mute",
                                                      "<username>",
                                                      "Замутить пользователя.",
                                                      Mute,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      PAGE_TERMINATOR_CMD_ID,
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
                                                      Restriction.Everyone,
                                                      aliases: ["ask",]
                                                     ),
                               new DefaultChatCommand(
                                                      13,
                                                      "potato",
                                                      "[entry]",
                                                      "сгенерировать сообщение",
                                                      Potato,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      14,
                                                      "carrot",
                                                      string.Empty,
                                                      "зарандомить новое сообщение",
                                                      Carrot,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      PAGE_TERMINATOR_CMD_ID,
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
                                                      PAGE_TERMINATOR_CMD_ID,
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
                                                      PAGE_TERMINATOR_CMD_ID,
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
                                                      PAGE_TERMINATOR_CMD_ID,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      28,
                                                      "hardest",
                                                      "[username] [--top <number>]",
                                                      "хардест пользователя по AREDL.",
                                                      Hardest,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      29,
                                                      "easiest",
                                                      "[username] [--top <number>]",
                                                      "легчайший экстрим пользователя по AREDL.",
                                                      Easiest,
                                                      Restriction.Everyone,
                                                      aliases: ["lowest",]
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
                                                      "<level_name> [--page number]",
                                                      "позиция уровня по AREDL.",
                                                      Place,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      27,
                                                      "roulette",
                                                      "<number> <number>",
                                                      "зарандомить экстрим.",
                                                      Roulette,
                                                      Restriction.Everyone,
                                                      aliases: ["rulet",]
                                                     ),
                               new DefaultChatCommand(
                                                      PAGE_TERMINATOR_CMD_ID,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      59,
                                                      "phardest",
                                                      "[username] [--top <number>]",
                                                      "хардест пользователя по Pemon List.",
                                                      PHardest,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      60,
                                                      "peasiest",
                                                      "[username] [--top <number>]",
                                                      "легчайший экстрим пользователя по Pemon List.",
                                                      PEasiest,
                                                      Restriction.Everyone,
                                                      aliases: ["plowest",]
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
                                                      "<level> [--page <number>]",
                                                      "позиция уровня по Pemon List.",
                                                      Pplace,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      PAGE_TERMINATOR_CMD_ID,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      76,
                                                      "profile",
                                                      "<username>",
                                                      "Информация о пользователе.",
                                                      Profile,
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
                                                      PAGE_TERMINATOR_CMD_ID,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      36,
                                                      "games",
                                                      "[--page <number>]",
                                                      "список заказов игр.",
                                                      Games,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      37,
                                                      "add-game",
                                                      "<game_name> [--position <number>] [--user <username>]",
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
                                                      PAGE_TERMINATOR_CMD_ID,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      42,
                                                      "create-reward",
                                                      "<title;cost> [;is_input_required (true/false)]",
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
                                                      PAGE_TERMINATOR_CMD_ID,
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
                                                      PAGE_TERMINATOR_CMD_ID,
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
                                                      PAGE_TERMINATOR_CMD_ID,
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
                               new DefaultChatCommand(
                                                      PAGE_TERMINATOR_CMD_ID,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      62,
                                                      "shop",
                                                      "[--page number]",
                                                      "список лотов.",
                                                      Shop,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      77,
                                                      "add-lot",
                                                      "<lot_name;cost>",
                                                      "добавить лот.",
                                                      ShopAdd,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      78,
                                                      "remove-lot",
                                                      "<lot_name>",
                                                      "удалить лот.",
                                                      ShopRemove,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      75,
                                                      "buy",
                                                      "<lot_name>;[amount]",
                                                      "купить лот.",
                                                      Buy,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      76,
                                                      "use",
                                                      "<lot_name>",
                                                      "использовать лот.",
                                                      Use,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      PAGE_TERMINATOR_CMD_ID,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      63,
                                                      "gamble",
                                                      "<money>",
                                                      "слить баллы.",
                                                      Gamble,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      61,
                                                      "balance",
                                                      string.Empty,
                                                      "вывести баланс.",
                                                      Balance,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      64,
                                                      "give",
                                                      "<money;username>",
                                                      "отдать нищему фантики.",
                                                      Give,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      65,
                                                      "giveaway",
                                                      "[money]",
                                                      "раздать нищим фантики.",
                                                      Giveaway,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      PAGE_TERMINATOR_CMD_ID,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      70,
                                                      "duel",
                                                      "<money;username>",
                                                      "парный гемблинг.",
                                                      Duel,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      71,
                                                      "accept-duel",
                                                      "[username]",
                                                      "принять предложение погемблить.",
                                                      AcceptDuel,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      72,
                                                      "decline-duel",
                                                      "[username]",
                                                      "отказать в предложении погемблить.",
                                                      DeclineDuel,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      73,
                                                      "remove-duels",
                                                      "[username]",
                                                      "удалить все дуэли.",
                                                      RemoveDuels,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      74,
                                                      "list-duels",
                                                      string.Empty,
                                                      "предложения погемблить.",
                                                      ListDuels,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      PAGE_TERMINATOR_CMD_ID,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      66,
                                                      "bank-list-rewards",
                                                      "[--page number]",
                                                      "вывести список наград.",
                                                      BankListRewards,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      67,
                                                      "bank-create-reward",
                                                      "<money>",
                                                      "создать награду.",
                                                      BankCreateReward,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      68,
                                                      "bank-delete-reward",
                                                      "<id>",
                                                      "удалить награду.",
                                                      BankDeleteReward,
                                                      Restriction.DevBroad
                                                     ),
                               new DefaultChatCommand(
                                                      PAGE_TERMINATOR_CMD_ID,
                                                      string.Empty,
                                                      string.Empty,
                                                      string.Empty,
                                                      PageTerminator,
                                                      Restriction.Everyone
                                                     ),
                               new DefaultChatCommand(
                                                      69,
                                                      "evaluate",
                                                      "<expression>",
                                                      "выполнить выражение.",
                                                      Evaluate,
                                                      Restriction.DevBroad,
                                                      aliases: ["eval", "e",]
                                                     ),
                           ];
    }
    
    public static void SetDefaults() {
        _chatCmds.Options.SetDefaultCmds(DefaultsCommands);
    }

    private static async Task Ping(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) {
            return;
        }
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        await client.SendMessage("Pong!", chatMessage.Id);
    }
    
    private static async Task Cmds(ChatCmdArgs cmdArgs) {
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var cmdId = cmdArgs.Parsed.CommandIdentifier;
        var index = 1;
        var cmds = new List<string>();

        foreach (var cmd in _chatCmds.Options.DefaultCmds) {
            if (cmd.State == State.Disabled || !chatMessage.Fits(cmd.Restriction)) continue;
            if (_chatCmds.Options.DefaultCmds
                         .Any(defaultCmd =>
                                  defaultCmd.Name.Equals(cmd.Name, StringComparison.InvariantCultureIgnoreCase)
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
            
            var desc = string.IsNullOrEmpty(cmd.Description) || cmd.Description.Equals("--") ? 
                           string.Empty :
                           $"- {cmd.Description}";

            var args = string.IsNullOrEmpty(cmd.Args) || cmd.Args.Equals("--") ?
                           string.Empty :
                           cmd.Args;
            
            cmds.Add($"{index}. {cmdId}{cmd.Name} {args} {desc} ");
            index++;
        }
        
        
        await SendPagedReply(cmds, cmdArgs, _chatCmds.Options.SendWhisperIfPossible);
    }
    
    private static async Task More(ChatCmdArgs cmdArgs) {
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var cmdId = cmdArgs.Parsed.CommandIdentifier;
        
        var index = 1;
        var cmds = new List<string>();
        
        foreach (var cmd in _chatCmds.Options.CustomCmds) {
            if (cmd.State == State.Disabled || !chatMessage.Fits(cmd.Restriction)) continue;
            if (_chatCmds.Options.CustomCmds
                         .Any(customCmd =>
                                  customCmd.Name.Equals(cmd.Name, StringComparison.InvariantCultureIgnoreCase)
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
        
        await SendPagedReply(cmds, cmdArgs, _chatCmds.Options.SendWhisperIfPossible);
    }
    
    private static async Task Help(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList; 
        var cmdId = cmdArgs.Parsed.CommandIdentifier;

        switch (args.Count) {
            case <= 0: {
                var usage = $"{cmdId}<комманда> \"аргумент1\" \"аргумент2\" ... | {cmdId}{_chatCmds.Options.DefaultCmds[1].Name} для списка комманд";
                switch (_chatCmds.Options.SendWhisperIfPossible) {
                    case false: {
                        await client.SendMessage(usage, chatMessage.Id); break;
                    }
                    case true: {
                        await client.SendWhisper(usage, chatMessage.UserId); break;
                    }
                }
                break;
            }
            case > 0: {
                var cmdName = args[0];
                if (cmdName.Length > 0 && cmdName[0] == cmdId) {
                    cmdName = cmdName[1..];
                }

                var page = 1;
                var prevWasTerminator = false;
                
                foreach (var cmd in _chatCmds.Options.DefaultCmds) {
                    var isExplicitTerminator = cmd.Id == PAGE_TERMINATOR_CMD_ID || string.IsNullOrEmpty(cmd.Name);
                    
                    if (isExplicitTerminator) {
                        if (prevWasTerminator) continue;
                        if (cmd.State == State.Disabled || !chatMessage.Fits(cmd.Restriction)) continue;
                        if (_chatCmds.Options.DefaultCmds
                                     .Any(defaultCmd =>
                                              defaultCmd.Name.Equals(cmd.Name, StringComparison.InvariantCultureIgnoreCase)
                                           && defaultCmd.Restriction < cmd.Restriction
                                           && chatMessage.Fits(defaultCmd.Restriction))) {
                            continue;
                        }
                        
                        ++page;
                        prevWasTerminator = true;
                        continue;
                    }
                    
                    prevWasTerminator = false;
                    if (cmd.State == State.Disabled || !chatMessage.Fits(cmd.Restriction)) continue;
                    if (_chatCmds.Options.DefaultCmds
                                 .Any(defaultCmd =>
                                          defaultCmd.Name.Equals(cmd.Name, StringComparison.InvariantCultureIgnoreCase)
                                       && defaultCmd.Restriction < cmd.Restriction
                                       && chatMessage.Fits(defaultCmd.Restriction))){
                        continue;
                    }
                    
                    if (!string.Equals(cmdName, cmd.Name, StringComparison.InvariantCultureIgnoreCase) 
                     && !cmd.Aliases.Contains(cmdName)) continue;
                    
                    var desc = string.IsNullOrEmpty(cmd.Args) || cmd.Description.Equals("--") ? 
                                   string.Empty :
                                   $"- {cmd.Description}";

                    var arguments = string.IsNullOrEmpty(cmd.Args) || cmd.Args.Equals("--") ?
                                   string.Empty :
                                   cmd.Args;
                    
                    await client.SendMessage($"{cmdId}{cmd.Name} {arguments} {desc} | Страница {page}", chatMessage.Id);
                    return;
                }
                
                await client.SendMessage($"Команда {cmdId}{cmdName} не существует.", chatMessage.Id);
                break;
            }
        }
    }

    private static async Task When(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        
        if (args.Count < 1) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var argSb = new StringBuilder();
                
        foreach (var arg in args) {
            argSb.Append($"{arg} ");
        }

        var random = Random.Shared.Next(0, 6);
        var randomizedMessage = random switch {
                                    0 => $"{argSb} уже завтра! PewPewPew PewPewPew PewPewPew",
                                    1 => $"{argSb} на этой неделе! {Get7TvEmote(Emote.Aga)} {Get7TvEmote(Emote.Aga)} {Get7TvEmote(Emote.Aga)}",
                                    2 => $"{argSb} в следующем месяце! {Get7TvEmote(Emote.Like)} {Get7TvEmote(Emote.Like)} {Get7TvEmote(Emote.Like)}",
                                    3 => $"{argSb} через год {Get7TvEmote(Emote.Wait)} {Get7TvEmote(Emote.Wait)} {Get7TvEmote(Emote.Wait)}",
                                    4 => $"{argSb} через 69 лет {Get7TvEmote(Emote.Sad)} {Get7TvEmote(Emote.Sad)} {Get7TvEmote(Emote.Sad)}",
                                    5 => $"{argSb} никогда {Get7TvEmote(Emote.Laugh)} {Get7TvEmote(Emote.Laugh)} {Get7TvEmote(Emote.Laugh)}",
                                    _ => string.Empty,
                                };
        
        await client.SendMessage(randomizedMessage, chatMessage.Id); 
    }

    private static async Task Ban(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        
        if (args.Count < 1) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var argSb = new StringBuilder();
                
        foreach (var arg in args) {
            argSb.Append($"{arg} ");
        }
                
        await client.SendMessage($"{argSb} отправлен в бан sillyJAIL sillyJAIL sillyJAIL", chatMessage.Id);
    }

    private static async Task Mute(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        
        if (args.Count <= 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var username = args[0];
        var userId = await _bot.Api.GetUserId(username, client.Credentials, (_, message) => {
                                                                             _logger.Log(LogLevel.Error, message);
                                                                         });
        if (userId == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.UserNotFound, chatMessage, client);
            return;
        }

        var result = await DefaultLots.Mute(chatMessage.UserId, userId);
        if (!result.Ok) {
            await ErrorHandler.ReplyWithError(result.Error, chatMessage, client);
            return;
        }

        var random = Random.Shared.Next(0, 5);
        var message = random switch {
                          0 => $"{username} отошел поспать.",
                          1 => $"{username} решил передохнуть.",
                          2 => $"{username} bb.",
                          3 => $"{username} очень устал.",
                          _ => $"{username} пошел посрать.",
                      };

        await client.SendMessage(message, chatMessage.Id);
    }
    
    private static async Task Ai(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var shop = (ShopService)Services.Get(ServiceId.Shop);
        var ai = (AiService)Services.Get(ServiceId.Ai);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        
        if (args.Count < 1) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var prompt = new StringBuilder();
        string? id = null;
        
        foreach (var word in args) {
            if (prompt.Length <= 0
             && word.Length > 0
             && word[0] == '#') {
                id = word[1..word.Length];
                continue;
            }
            
            prompt.Append($"{word} ");
        }

        var lot = shop.Get(0);
        if (lot == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }
        
        Result<string?, ErrorCode?> result;

        var promptStr = prompt.ToString().Trim();
        if (lot.State == State.Enabled) {
            result = await DefaultLots.Ai(chatMessage.UserId, promptStr, id);
        } else {
            result = await ai.GetResponse(promptStr, id);
        }
        
        if (!result.Ok) {
            await ErrorHandler.ReplyWithError(result.Error, chatMessage, client);
            return;
        }

        var response = result.Value;
        
        if (string.IsNullOrEmpty(response)) {
            await ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
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
            await client.SendMessage(message, chatMessage.Id);
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
        }
    }

    private static async Task ReqEveryone(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var levelRequests = (LevelRequestsService)Services.Get(ServiceId.LevelRequests);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var reqState = levelRequests.GetReqState();
        await client.SendMessage($"Req {levelRequests.GetReqStateStr(reqState, eng: true)}", chatMessage.Id); 
    }
    
    private static async Task Req(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var levelRequests = (LevelRequestsService)Services.Get(ServiceId.LevelRequests);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (levelRequests.GetServiceState() == State.Disabled) return;
        
        var reqState = levelRequests.GetReqState();
        switch (args.Count) {
            case <= 0:
                await ReqEveryone(cmdArgs);
                return;
            case > 0:
                switch (args[0]) {
                    case "off": {
                        reqState = ReqState.Off;
                        await _bot.Api.SetChannelRewardState(levelRequests.GetRewardId(), false, client.Credentials, 
                                                          (_, message) => { 
                                                              _logger.Log(LogLevel.Error, message);
                                                          });
                        break;
                    }
                    case "points": {
                        reqState = ReqState.Points;
                        var result = await _bot.Api.SetChannelRewardState(levelRequests.GetRewardId(), true, client.Credentials,
                                                                            (_, message) => {
                                                                                _logger.Log(LogLevel.Error, message);
                                                                            });
                        if (!result) {
                            await ErrorHandler.ReplyWithError(ErrorCode.NoRewardSet, chatMessage, client);
                            return;
                        }
                        break;
                    }
                    case "on": {
                        reqState = ReqState.On;
                        await _bot.Api.SetChannelRewardState(levelRequests.GetRewardId(), false, client.Credentials,
                                                          (_, message) => {
                                                              _logger.Log(LogLevel.Error, message);
                                                          });
                        break;
                    }
                }
                break;
        }

        levelRequests.Options.SetReqState(reqState);
        await client.SendMessage($"Реквесты теперь {levelRequests.GetReqStateStr(reqState)}", chatMessage.Id);
    }

    private static async Task SetReqReward(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var levelRequests = (LevelRequestsService)Services.Get(ServiceId.LevelRequests);
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (chatMessage.RewardId == null) {
            await client.SendMessage("Используйте эту команду внутри награды.", chatMessage.Id);
            return;
        }
        
        levelRequests.SetRewardId(chatMessage.RewardId);
        await client.SendMessage("Награда для реквестов успешно установлена.", chatMessage.Id);
    }
    
    private static async Task Potato(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var textGenerator = (TextGeneratorService)Services.Get(ServiceId.TextGenerator);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        
        if (args.Count <= 0) {
            textGenerator.GenerateAndSend();
        }else {
            var sb = new StringBuilder();
            foreach (var arg in args) {
                sb.Append(arg);
            }
            
            var err = textGenerator.Generate(out var message, sb.ToString());
            if (err != ErrorCode.None) {
                await ErrorHandler.ReplyWithError(err, chatMessage, client);
                return;
            }

            await client.SendMessage(message);
        }
    }

    private static async Task Clip(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var clipId = await _bot.Api.CreateClip(client.Credentials, (_, message) => {
                                                                         _logger.Log(LogLevel.Error, message);
                                                                     });

        if (clipId == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.ClipCreationFailed, chatMessage, client);
            return;
        }
        await client.SendMessage($"Клип создан - https://www.twitch.tv/{client.Credentials.Broadcaster.Login}/clip/{clipId}", chatMessage.Id);
    }

    private static async Task TitleEveryone(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var channelInfo = await _bot.Api.GetChannelInfo(client.Credentials, (_, message) => {
                                                                                  _logger.Log(LogLevel.Error, message);
                                                                              });
        
        await client.SendMessage($"Название стрима - {channelInfo!.Title}", chatMessage.Id);
    }
    
    private static async Task Title(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var baseTitle = ((ChatCommandsService)Services.Get(ServiceId.ChatCommands)).GetBaseTitle();
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
                    
        if (args.Count < 1) {
            await TitleEveryone(cmdArgs);
            return;
        }

        var channelInfo = await _bot.Api.GetChannelInfo(client.Credentials, (_, message) => {
                                                                                  _logger.Log(LogLevel.Error, message);
                                                                              });
        if (channelInfo == null) return;
        
        var titleSb = new StringBuilder();
        titleSb.Append($"{baseTitle} ");
        foreach (var arg in args) {
            titleSb.Append($"{arg} ");
        }

        var result = await _bot.Api.UpdateChannelInfo(titleSb.ToString(), channelInfo.GameId, client.Credentials, (_, message) => {
                                                            _logger.Log(LogLevel.Error, message);
                                                        });
        if (!result) {
            await client.SendMessage("Не удалось изменить название", chatMessage.Id);
            return;
        }
        await client.SendMessage($"Название стрима изменено на {titleSb}", chatMessage.Id);
    }
    
    private static async Task Followage(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        string result;
        var username = chatMessage.UserName;
        if (args.Count > 0) {
            username = args[0];
        }

        var followage = await _bot.Api.GetFollowage(username, client.Credentials, (_, message) => {
                                                               _logger.Log(LogLevel.Error, message);
                                                           });
        if (followage == null) {
            if (args.Count > 0) {
                result = 
                    username == chatMessage.Channel ?
                             $"{username} это владелец канала {Get7TvEmote(Emote.Rizz)}" :
                             $"{username} не фолловнут на {chatMessage.Channel} {Get7TvEmote(Emote.Sad)}";
            } else {
                result = 
                    username == chatMessage.Channel ?
                        $"Вы владелец канала {Get7TvEmote(Emote.Rizz)}" :
                        $"Вы не фолловнуты на {chatMessage.Channel} {Get7TvEmote(Emote.Sad)}";
            }

            await client.SendMessage(result, chatMessage.Id);
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
        await client.SendMessage(result, chatMessage.Id);
    }

    private static async Task GameEveryone(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var channelInfo = await _bot.Api.GetChannelInfo(client.Credentials, (_, message) => {
                                                                                  _logger.Log(LogLevel.Error, message);
                                                                              });
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        await client.SendMessage($"Текущая категория - {channelInfo!.GameName}", chatMessage.Id);
    }
    
    private static async Task Game(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (args.Count < 1) {
            await GameEveryone(cmdArgs);
            return;
        }
        
        var channelInfo = await _bot.Api.GetChannelInfo(client.Credentials, (_, message) => {
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

        var gameId = await _bot.Api.FindGameId(gameSb.ToString(), client.Credentials, (_, message) => {
                                                     _logger.Log(LogLevel.Error, message);
                                                 });
        if (gameId == null) return;
        
        var result = await _bot.Api.UpdateChannelInfo(channelInfo.Title, gameId, client.Credentials, (_, message) => {
                                                            _logger.Log(LogLevel.Error, message);
                                                        });
        if (!result) {
            await client.SendMessage("Не удалось изменить категорию", chatMessage.Id);
            return;
        }
        channelInfo = await _bot.Api.GetChannelInfo(client.Credentials, (_, message) => {
                                                                              _logger.Log(LogLevel.Error, message);
                                                                          });
        await client.SendMessage($"Категория изменена на {channelInfo!.GameName}", chatMessage.Id);
    }
    
    private static async Task Guess(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var messageRandomizer = (MessageRandomizerService)Services.Get(ServiceId.MessageRandomizer);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (messageRandomizer.Options.MessageState == MessageState.Guessed) {
            await client.SendMessage("Уже отгадано.", chatMessage.Id);
            return;
        }

        if (args.Count < 1) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var err = messageRandomizer.GetLastGeneratedMessage(out var message);
        if (await ErrorHandler.ReplyWithError(err, chatMessage, client)
         || message == null) {
            return;
        }

        var userId = await _bot.Api.GetUserId(args[0], client.Credentials, (_, msg) => {
                                               _logger.Log(LogLevel.Error, msg);
                                           });
        if (userId == null) return;
        
        if (userId != message.UserId) {
            await client.SendMessage("Неправильно.", chatMessage.Id);
        } else {
            await client.SendMessage($"Правильно, это было сообщение от {args[0]}.", chatMessage.Id);
            messageRandomizer.Options.SetMessageState(MessageState.Guessed);
        }
    }

    private static async Task Whose(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var messageRandomizer = (MessageRandomizerService)Services.Get(ServiceId.MessageRandomizer);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var err = messageRandomizer.GetLastGeneratedMessage(out var message);
        if (await ErrorHandler.ReplyWithError(err, chatMessage, client)
         || message == null) {
            return;
        }

        var username = await _bot.Api.GetUserName(message.UserId, client.Credentials, true, (_, msg) => {
                                                   _logger.Log(LogLevel.Error, msg);
                                               });
        await client.SendMessage($"Это было сообщение от '{username}'", chatMessage.Id);
    }

    private static async Task Repeat(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var messageRandomizer = (MessageRandomizerService)Services.Get(ServiceId.MessageRandomizer);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var err = messageRandomizer.GetLastGeneratedMessage(out var message);
        if (await ErrorHandler.ReplyWithError(err, chatMessage, client) || message == null) {
            return;
        }
        
        await client.SendMessage(message.Text, chatMessage.Id);
    }

    private static async Task Translate(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var translator = (TranslatorService)Services.Get(ServiceId.Translator);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (translator.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }
        
        if (args.Count <= 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
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
            await ErrorHandler.ReplyWithError(ErrorCode.TranslationFailed, chatMessage, client);
            return;
        }
        await client.SendMessage($"Перевод: {translated}", chatMessage.Id);
    }

    private static async Task DetectLang(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var translator = (TranslatorService)Services.Get(ServiceId.Translator);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (translator.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }
        
        if (args.Count <= 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
        
        var textSb = new StringBuilder();
        foreach (var arg in args) {
            if (arg is null or "--") break;
            textSb.Append($"{arg} ");
        }
        
        var lang = await translator.DetectLanguage(textSb.ToString());
        if (lang == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
            return;
        }
        await client.SendMessage($"Самый близкий язык - {lang.LanguageCode}", chatMessage.Id);
    }

    private static async Task Lang(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var translator = (TranslatorService)Services.Get(ServiceId.Translator);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (translator.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }
        
        if (args.Count <= 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
        
        translator.SetTargetLanguage(args[0]);
        await client.SendMessage($"Язык установлен на '{args[0]}'", chatMessage.Id);
    }
    
    private static async Task Carrot(ChatCmdArgs cmdArgs) {
        var messageRandomizer = (MessageRandomizerService)Services.Get(ServiceId.MessageRandomizer);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        if (messageRandomizer.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }
                    
        var err = messageRandomizer.Generate(out var message);
        if (await ErrorHandler.ReplyWithError(err, chatMessage, client)
         || message == null) return;
        
        client?.SendMessage(message.Text);
    }

    private static async Task Top(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)Services.Get(ServiceId.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (demonList.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (args.Count < 1) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
        
        var indexStr = args[0];
        if (string.IsNullOrEmpty(indexStr)
         || !int.TryParse(indexStr, out var index)) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }
        
        var levelInfo = await demonList.GetLevelByPlacement(index);
        if (levelInfo == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }

        var formatedOutput = await demonList.FormatLevelInfo(levelInfo);
        await client.SendMessage(formatedOutput, chatMessage.Id);
    }

    private static async Task Place(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)Services.Get(ServiceId.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (demonList.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (args.Count < 1) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var levelName = new StringBuilder();
        foreach (var arg in args) {
            if (arg is "--page") break;
            levelName.Append($"{arg} ");
        }

        var levelsInfo =  await demonList.GetLevelsInfoByName(levelName.ToString().Trim());
        if (levelsInfo == null || levelsInfo.Count == 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }
        
        var page = ParsePage(levelsInfo.Count, cmdArgs);
        var levelInfo = levelsInfo[page];
        
        var pages = 
            levelsInfo.Count <= 1 ? 
                string.Empty : 
                $"Страница {page+1} из {levelsInfo.Count} |";
        
        var formatedOutput = await demonList.FormatLevelInfo(levelInfo);
        await client.SendMessage($"{pages} {formatedOutput}", chatMessage.Id);
    }
    
    private static async Task Ptop(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)Services.Get(ServiceId.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (demonList.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (args.Count < 1) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
        
        if (!int.TryParse(args[0], out var index)) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }
        
        var levelInfo =  await demonList.GetPlatformerLevelByPlacement(index);
        if (levelInfo == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }
        
        var formatedOutput = await demonList.FormatLevelInfo(levelInfo);
        await client.SendMessage(formatedOutput, chatMessage.Id);
    }
    
    private static async Task Pplace(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)Services.Get(ServiceId.DemonList);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;

        if (demonList.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (args.Count < 1) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var levelName = new StringBuilder();
        foreach (var arg in args) {
            if (arg is "--page") break;
            levelName.Append($"{arg} ");
        }

        var levelsInfo =  await demonList.GetPlatformerLevelsInfoByName(levelName.ToString().Trim());
        if (levelsInfo is not { Count: > 0, }) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }

        var page = ParsePage(levelsInfo.Count, cmdArgs);
        var levelInfo = levelsInfo[page];
        
        var pages = 
            levelsInfo.Count <= 1 ? 
                string.Empty : 
                $"Страница {page+1} из {levelsInfo.Count} |";
        
        var formatedOutput = await demonList.FormatLevelInfo(levelInfo);
        await client.SendMessage($"{pages} {formatedOutput}", chatMessage.Id);
    }

    private static async Task Hardest(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)Services.Get(ServiceId.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (demonList.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (!GetIntArg(cmdArgs, "--top", out var top)) {
            top = 1;
        }

        if (top <= 0) top = 1;
        if (top > 10) top = 10;
        
        var usernameSb = new StringBuilder();
        for (var i = 0; i < args.Count; i++) {
            var arg = args[i];
            if (arg.Length > 1 && arg[0..2] is "--") {
                break;
            }

            usernameSb.Append($"{arg}");
            if (i < args.Count - 1) {
                usernameSb.Append(' ');
            }
        }

        var formatedOutput = new StringBuilder();
        
        var username = usernameSb.ToString();
        
        if (string.IsNullOrEmpty(username)) {
            if (demonList.GetDefaultUserName(out var t)) {
                username = t;
            }
        }
        
        if (string.IsNullOrEmpty(username)) {
            for (var i = 1; i <= top; ++i) {
                var levelInfo = await demonList.GetLevelByPlacement(i);
                if (levelInfo == null) {
                    await ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
                    return;
                }

                formatedOutput.Append($"{(top > 1 ? $"{i}." : string.Empty)} {await demonList.FormatLevelInfo(levelInfo, top == 1)}");

                if (i < top - 1) {
                    formatedOutput.Append(" / ");
                }
            }
            
            await client.SendMessage(formatedOutput.ToString(), chatMessage.Id);
            return;
        }
        
        var profile = await demonList.GetProfile(username);
        if (profile == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.UserNotFound, chatMessage, client);
            return;
        }
        
        var hardests = await demonList.GetHardests(profile, top);
        if (hardests is not { Count: > 0, }) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }

        top = Math.Min(top, hardests.Count);
        
        for (var i = 0; i < top; ++i) {
            var record = hardests[i];
            
            formatedOutput.Append($"{(top > 1? $"{i + 1}." : string.Empty)} {demonList.FormatRecordInfo(record, top == 1)}");

            if (i < top - 1) {
                formatedOutput.Append(" / ");
            }
        }
        
        await client.SendMessage(formatedOutput.ToString(), chatMessage.Id);
    }

    private static async Task PHardest(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)Services.Get(ServiceId.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (demonList.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (!GetIntArg(cmdArgs, "--top", out var top)) {
            top = 1;
        }

        if (top <= 0) top = 1;
        if (top > 10) top = 10;
        
        var usernameSb = new StringBuilder();
        for (var i = 0; i < args.Count; i++) {
            var arg = args[i];
            if (arg.Length > 1 && arg[0..2] is "--") {
                break;
            }

            usernameSb.Append($"{arg}");
            if (i < args.Count - 1) {
                usernameSb.Append(' ');
            }
        }

        var formatedOutput = new StringBuilder();
        
        var username = usernameSb.ToString();

        if (string.IsNullOrEmpty(username)) {
            if (demonList.GetDefaultUserName(out var t)) {
                username = t;
            }
        }
        
        if (string.IsNullOrEmpty(username)) {
            for (var i = 1; i <= top; ++i) {
                var levelInfo = await demonList.GetPlatformerLevelByPlacement(i);
                if (levelInfo == null) {
                    await ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
                    return;
                }

                formatedOutput.Append($"{(top > 1 ? $"{i}." : string.Empty)} {await demonList.FormatLevelInfo(levelInfo, top == 1)}");

                if (i < top - 1) {
                    formatedOutput.Append(" / ");
                }
            }
            
            await client.SendMessage(formatedOutput.ToString(), chatMessage.Id);
            return;
        }
        
        var profile = await demonList.GetProfile(username);
        if (profile == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.UserNotFound, chatMessage, client);
            return;
        }
        
        var hardests = await demonList.GetPlatformerHardests(profile, top);
        if (hardests is not { Count: > 0, }) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }

        top = Math.Min(top, hardests.Count);
        
        for (var i = 0; i < top; ++i) {
            var record = hardests[i];
            
            formatedOutput.Append($"{(top > 1? $"{i + 1}." : string.Empty)} {demonList.FormatRecordInfo(record, top == 1)}");

            if (i < top - 1) {
                formatedOutput.Append(" / ");
            }
        }
        
        await client.SendMessage(formatedOutput.ToString(), chatMessage.Id);
    }
    
    private static async Task Easiest(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)Services.Get(ServiceId.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (demonList.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client); 
            return;
        }

        if (!GetIntArg(cmdArgs, "--top", out var top)) {
            top = 1;
        }

        if (top <= 0) top = 1;
        if (top > 10) top = 10;
        
        var usernameSb = new StringBuilder();
        for (var i = 0; i < args.Count; i++) {
            var arg = args[i];
            if (arg.Length > 1 && arg[0..2] is "--") {
                break;
            }

            usernameSb.Append($"{arg}");
            if (i < args.Count - 1) {
                usernameSb.Append(' ');
            }
        }
        
        var formatedOutput = new StringBuilder();
        
        var username = usernameSb.ToString();

        if (string.IsNullOrEmpty(username)) {
            if (demonList.GetDefaultUserName(out var t)) {
                username = t;
            }
        }
        
        if (string.IsNullOrEmpty(username)) {
            var levelsCount = await demonList.GetLevelsCount();
            
            for (var i = levelsCount; i > levelsCount - top; --i) {
                var levelInfo = await demonList.GetLevelByPlacement(i);
                if (levelInfo == null) {
                    await ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
                    return;
                }

                formatedOutput.Append($"{(top > 1 ? $"{i}." : string.Empty)} {await demonList.FormatLevelInfo(levelInfo, top == 1)}");

                if (i < top - 1) {
                    formatedOutput.Append(" / ");
                }
            }
            
            await client.SendMessage(formatedOutput.ToString(), chatMessage.Id);
            return;
        }

        var profile = await demonList.GetProfile(username);
        if (profile == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.UserNotFound, chatMessage, client);
            return;
        }
        
        var easiests = await demonList.GetEasiests(profile, top);
        if (easiests is not { Count: > 0, }) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }

        top = Math.Min(top, easiests.Count);
        
        for (var i = 0; i < top; ++i) {
            var record = easiests[i];
            
            formatedOutput.Append($"{(top > 1? $"{i + 1}." : string.Empty)} {demonList.FormatRecordInfo(record, top == 1)}");

            if (i < top - 1) {
                formatedOutput.Append(" / ");
            }
        }
        
        await client.SendMessage(formatedOutput.ToString(), chatMessage.Id);
    }

    private static async Task PEasiest(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)Services.Get(ServiceId.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (demonList.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client); 
            return;
        }

        if (!GetIntArg(cmdArgs, "--top", out var top)) {
            top = 1;
        }

        if (top <= 0) top = 1;
        if (top > 10) top = 10;
        
        var usernameSb = new StringBuilder();
        for (var i = 0; i < args.Count; i++) {
            var arg = args[i];
            if (arg.Length > 1 && arg[0..2] is "--") {
                break;
            }

            usernameSb.Append($"{arg}");
            if (i < args.Count - 1) {
                usernameSb.Append(' ');
            }
        }
        
        var formatedOutput = new StringBuilder();
        
        var username = usernameSb.ToString();

        if (string.IsNullOrEmpty(username)) {
            if (demonList.GetDefaultUserName(out var t)) {
                username = t;
            }
        }
        
        if (string.IsNullOrEmpty(username)) {
            var levelsCount = await demonList.GetPlatformerLevelsCount();
            
            for (var i = levelsCount; i > levelsCount - top; --i) {
                var levelInfo = await demonList.GetPlatformerLevelByPlacement(i);
                if (levelInfo == null) {
                    await ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
                    return;
                }

                formatedOutput.Append($"{(top > 1 ? $"{i}." : string.Empty)} {await demonList.FormatLevelInfo(levelInfo, top == 1)}");

                if (i < top - 1) {
                    formatedOutput.Append(" / ");
                }
            }
            
            await client.SendMessage(formatedOutput.ToString(), chatMessage.Id);
            return;
        }

        var profile = await demonList.GetPlatformerProfile(username);
        if (profile == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.UserNotFound, chatMessage, client);
            return;
        }
        
        var easiests = await demonList.GetPlatformerEasiests(profile, top);
        if (easiests is not { Count: > 0, }) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }

        top = Math.Min(top, easiests.Count);
        
        for (var i = 0; i < top; ++i) {
            var record = easiests[i];
            
            formatedOutput.Append($"{(top > 1? $"{i + 1}." : string.Empty)} {demonList.FormatRecordInfo(record, top == 1)}");

            if (i < top - 1) {
                formatedOutput.Append(" / ");
            }
        }
        
        await client.SendMessage(formatedOutput.ToString(), chatMessage.Id);
    }

    private static async Task Profile(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)Services.Get(ServiceId.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (args.Count <= 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var username = cmdArgs.Parsed.CommandMessage;
        
        if (string.IsNullOrEmpty(username)) {
            if (demonList.GetDefaultUserName(out var t)) {
                username = t;
            }
        }
        
        var profile = await demonList.GetProfile(username);
        if (profile == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.UserNotFound, chatMessage, client);
            return;
        }

        var formatedOutput = demonList.FormatUserProfileInfo(profile);
        await client.SendMessage(formatedOutput, chatMessage.Id);
    }
    
    private static async Task Roulette(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)Services.Get(ServiceId.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var from = -1;
        var to = -1;

        if (demonList.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
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
            await ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
            return;
        }

        var formatedOutput = await demonList.FormatLevelInfo(levelInfo);
        await client.SendMessage(formatedOutput, chatMessage.Id);
    }

    private static async Task Clan(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)Services.Get(ServiceId.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (demonList.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }
        if (args.Count < 1) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var clanName = args[0];
        
        var clanInfo = await demonList.GetClanInfo(clanName);
        if (clanInfo == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }

        var formatedOutput = demonList.FormatClanInfo(clanInfo);
        await client.SendMessage(formatedOutput, chatMessage.Id);
    }
    
    private static async Task ClanHardest(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)Services.Get(ServiceId.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (demonList.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (args.Count < 1) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
                    
        var clanInfo = await demonList.GetClanInfo(args[0]);
        if (clanInfo == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }
        
        var hardest = await demonList.GetLevelDetails(clanInfo.Hardest.Id);
        if (hardest == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }
        
        var formatedOutput = demonList.FormatLevelDetails(hardest);
        await client.SendMessage(formatedOutput, chatMessage.Id);
    }

    private static async Task ClanRoulette(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var demonList = (DemonListService)Services.Get(ServiceId.DemonList);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        if (demonList.GetServiceState() == State.Disabled) {
            await ErrorHandler.ReplyWithError(ErrorCode.ServiceDisabled, chatMessage, client);
            return;
        }

        if (args.Count < 1) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var clanInfo = await demonList.GetClanInfo(args[0]);
        if (clanInfo == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }
        
        var clanSubmissionInfo = await demonList.GetRandomClanSubmission(clanInfo.Clan.Id)!;
        if (clanSubmissionInfo?.Level == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }

        var levelsInfo = await demonList.ClanSubmissionInfoToLevelInfo(clanSubmissionInfo);
        if (levelsInfo == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }
        
        var formatedOutput = await demonList.FormatLevelInfo(levelsInfo);
        await client.SendMessage(formatedOutput, chatMessage.Id);
    }

    private static async Task Games(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var gameRequestService = (GameRequestsService)Services.Get(ServiceId.GameRequests);
        var chatMessage = cmdArgs.Parsed.ChatMessage;


        var gameRequests = gameRequestService.GetGameRequests();

        if (gameRequests?.Count <= 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.Empty, chatMessage, client) ;
            return;
        }
        
        var reply = new List<string>();
        for (var i = 0; i < gameRequests?.Count; i++) {
            var gameRequest = gameRequests[i];
            var separator = "/";
            
            if (i >= gameRequests.Count-1) {
                separator = string.Empty;
            }

            var username = await _bot.Api.GetUserName(gameRequest.UserId, client.Credentials, true, (_, message) => {
                                                     _logger.Log(LogLevel.Error, message);
                                                 });
            reply.Add($"{i+1}. {gameRequest.GameName} -> {username} {separator}");
        }

        await SendPagedReply(reply, cmdArgs);
    }

    private static async Task AddGameRequestsReward(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var gameRequest = (GameRequestsService)Services.Get(ServiceId.GameRequests);
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (chatMessage.RewardId == null) {
            await client.SendMessage("Используйте эту комманду внутри награды.", chatMessage.Id);
            return;
        }
        
        gameRequest.Options.AddReward(chatMessage.RewardId);
        await client.SendMessage("Награда добавлена в список.", chatMessage.Id);
    }

    private static async Task ResetGameRequestsRewards(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var gameRequest = (GameRequestsService)Services.Get(ServiceId.GameRequests);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        gameRequest.Options.ResetRewards();
        await client.SendMessage("Список наград очищен.", chatMessage.Id);
    }
    
    private static async Task AddGame(ChatCmdArgs cmdArgs) {
        var gameRequestService = (GameRequestsService)Services.Get(ServiceId.GameRequests);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var client = _bot.GetClient();
        
        if (args.Count > 0) {
            await gameRequestService.AddGameRequest(args, chatMessage);
            return;
        }
        
        await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
    }

    private static async Task CompleteGame(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var gameRequestService = (GameRequestsService)Services.Get(ServiceId.GameRequests);
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var error = ParseIntArg(cmdArgs, out var indexToRemove);
        if (await ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }

        var gameRequests = gameRequestService.GetGameRequests();
        
        if (indexToRemove <= 0 || indexToRemove > gameRequests!.Count) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        indexToRemove--;
        var gameName = gameRequests[indexToRemove].GameName;
        
        gameRequestService.Options.RemoveRequest(indexToRemove);
        await client.SendMessage($"Игра {gameName} удалена из очереди.", chatMessage.Id);
    }
    
    private static async Task ResetGames(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var gameRequestService = (GameRequestsService)Services.Get(ServiceId.GameRequests);
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        gameRequestService.Options.ResetRequests();
        await client.SendMessage("Список заказов очищен.", chatMessage.Id);
    }

    private static async Task CreateReward(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (await ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }

        if (args.Length < 2) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
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

        var rewardId = await _bot.Api.CreateChannelReward(title, 
                                                          cost, 
                                                          client.Credentials, 
                                                          userInputRequired: requireInput, 
                                                          callback: (_, message) => { 
                                                                _logger.Log(LogLevel.Error, message); 
                                                            });
        if (rewardId == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.RequestFailed, chatMessage, client);
            return;
        }
        
        await client.SendMessage($"Награда успешно создана. ({rewardId})", chatMessage.Id);
    }
    
    private static async Task DeleteReward(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (args.Count <= 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var rewardId = args[0];
        var result = await _bot.Api.DeleteChannelReward(rewardId, client.Credentials, (_, message) => {
                                                              _logger.Log(LogLevel.Error, message);
                                                          });
        if (!result) {
            await ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
            return;
        }
        
        await client.SendMessage($"Награда успешно удалена. ({rewardId})", chatMessage.Id);
    }
    
    private static async Task TgNotifyEveryone(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var tgNotifications = (TgNotificationsService)Services.Get(ServiceId.TgNotifications);
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var stateStr =
            tgNotifications.GetServiceState() == State.Enabled? 
                "включены":
                "отключены";
        await client.SendMessage($"Уведомления о стримах {stateStr}", chatMessage.Id);
    }
    
    private static async Task TgNotify(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var tgNotifications = (TgNotificationsService)Services.Get(ServiceId.TgNotifications);
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
        await client.SendMessage($"Уведомления о стримах теперь {stateStr}", chatMessage.Id);
    }
    
    private static async Task TgNotifyPrompt(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var tgNotifications = (TgNotificationsService)Services.Get(ServiceId.TgNotifications);
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (args.Count <= 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
        
        var prompt = new StringBuilder();
        foreach (var arg in args) {
            prompt.Append($"{arg} ");
        }
        
        tgNotifications.SetNotificationPrompt(prompt.ToString().Trim());
        await client.SendMessage("Текст уведомлений успешно изменен.", chatMessage.Id);
    }

    private static async Task ListCustomCmds(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var cmds = _chatCmds.Options.GetCustomCommands();
        if (cmds.Count == 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.Empty, chatMessage, client);
            return;
        }
        
        var list = cmds.Select((cmd, i) => $"{i + 1}. {cmd.Name} (id: {cmd.Id}) ").ToList();
        await SendPagedReply(list, cmdArgs, _chatCmds.Options.SendWhisperIfPossible);
    }
    
    private static async Task AddCmd(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (await ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }
        
        if (args.Length < 2) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
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
        await client.SendMessage($"Новая команда '{cmd.Name}' добавлена успешно.", chatMessage.Id);
    }

    private static async Task RemoveCmd(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var error = ParseIntArg(cmdArgs, out var id);
        if (await ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }

        var cmd = _chatCmds.Options.GetCustomCmdById(id);
        if (cmd == null || !_chatCmds.Options.TryRemoveChatCmdById(id)) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        await client.SendMessage($"Команда '{cmd.Name}' успешно удалена.", chatMessage.Id);
    }
    
    private static async Task ChangeCmdDescription(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (await ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }
        
        if (!int.TryParse(args[0], out var id)) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        var cmd = _chatCmds.Options.GetCustomCmdById(id);

        if (cmd == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }
        
        cmd.SetDescription(args[1]);
        await client.SendMessage($"Описание команды '{cmd.Name}' изменено на {cmd.Description}.", chatMessage.Id);
    }
    
    private static async Task ChangeCmdOutput(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (await ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }
        
        if (args.Length < 2) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
        
        if (!int.TryParse(args[0], out var id)) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        var cmd = _chatCmds.Options.GetCustomCmdById(id);

        if (cmd == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }
        
        cmd.SetOutput(args[1]);
        await client.SendMessage($"Вывод команды '{cmd.Name}' изменено на {cmd.Output}.", chatMessage.Id);
    }

    private static async Task ListChatAds(ChatCmdArgs cmdArgs) {
        var chatAdsService = (ChatAdsService)Services.Get(ServiceId.ChatAds);
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
        
        var chatAdsService = (ChatAdsService)Services.Get(ServiceId.ChatAds);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (await ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }
        
        if (args.Length < 3) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var name = args[0];
        var output = args[1];
        
        if (!long.TryParse(args[2], out var cooldown)) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        var chatAd = new ChatAd(name, output, cooldown);
        chatAdsService.AddChatAd(chatAd);

        await client.SendMessage($"Чат-реклама с названием {chatAd.GetName()} добавлена успешно.", chatMessage.Id);
    }
    
    private static async Task RemoveChatAd(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatAdsService = (ChatAdsService)Services.Get(ServiceId.ChatAds);
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        
        var error = ParseIntArg(cmdArgs, out var indexToRemove);
        if (await ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }
        --indexToRemove;
        
        var name = chatAdsService.Options.GetChatAds()[indexToRemove].GetName();
        var result = chatAdsService.RemoveChatAd(indexToRemove);
        if (!result) {
            await client.SendMessage($"Не удалось удалить чат-рекламу с айди {indexToRemove+1}.", chatMessage.Id);
            return;
        }
        
        await client.SendMessage($"Чат-р еклама с названием {name} удалена успешно.", chatMessage.Id);
    }

    private static async Task ChangeChatAdName(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatAdsService = (ChatAdsService)Services.Get(ServiceId.ChatAds);
        var chatAds = chatAdsService.Options.GetChatAds();
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (await ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }

        if (args.Length < 2
         || !int.TryParse(args[0], out var index)) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        --index;
        if (index < 0 || index >= chatAds.Count) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        var chatAd = chatAds[index];

        var oldName = chatAd.GetName();
        chatAd.SetName(args[1]);
        
        await client.SendMessage($"Название чат-рекламы {oldName} изменено на {chatAd.GetName()}.", chatMessage.Id);
    }
    
    private static async Task ChangeChatAdOutput(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatAdsService = (ChatAdsService)Services.Get(ServiceId.ChatAds);
        var chatAds = chatAdsService.Options.GetChatAds();
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (await ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }

        if (args.Length < 2
         || !int.TryParse(args[0], out var index)) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        --index;
        if (index < 0
         || index >= chatAds.Count) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        var chatAd = chatAds[index];
        chatAd.SetOutput(args[1]);
        
        await client.SendMessage($"Вывод чат-рекламы {chatAd.GetName()} изменен на {chatAd.GetOutput()}.", chatMessage.Id);
    }
    
    private static async Task ChangeChatAdCooldown(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatAdsService = (ChatAdsService)Services.Get(ServiceId.ChatAds);
        var chatAds = chatAdsService.Options.GetChatAds();
        var chatMessage = cmdArgs.Parsed.ChatMessage;

        var error = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (await ErrorHandler.ReplyWithError(error, chatMessage, client)) {
            return;
        }

        if (args.Length < 2
         || !int.TryParse(args[0], out var index)) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        --index;
        if (index < 0 
         || index >= chatAds.Count 
         || !int.TryParse(args[1], out var cooldown)) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        var chatAd = chatAds[index];
        chatAd.SetCooldown(cooldown);
        
        await client.SendMessage($"Перезарядка чат-рекламы {chatAd.GetName()} изменена на {chatAd.GetCooldown()}.", chatMessage.Id);
    }
    
    private static async Task Gamble(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var casino = (CasinoService)Services.Get(ServiceId.Casino);
        var bank = (BankService)Services.Get(ServiceId.Bank);

        var err = ParseIntArg(cmdArgs, out var quantity);
        if (err != ErrorCode.None) {
            await ErrorHandler.ReplyWithError(err, chatMessage, client);
            return;
        }

        if (!bank.GetBalance(chatMessage.UserId, out var balance)) {
            await ErrorHandler.ReplyWithError(ErrorCode.AccountNotFound, chatMessage, client);
            return;
        }
        var result = casino.Gamble(chatMessage.UserId, quantity);
        if (!result.Ok) {
            await ErrorHandler.ReplyWithError(result.Error, chatMessage, client);
            return;
        }
        
        if (!bank.GetBalance(chatMessage.UserId, out var newBalance)) {
            await ErrorHandler.ReplyWithError(ErrorCode.AccountNotFound, chatMessage, client);
            return;
        }

        var gambleResult = result.Value;
        if (gambleResult is not { Ok: true, }) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewPoints, chatMessage, client);
            return;
        }
        
        var arrow = gambleResult.Value.Result ? "↑" : "↓";
        
        await client.SendMessage($"{balance} -> {newBalance} | x{gambleResult.Value.Multiplier:F}{arrow}", chatMessage.Id);
    }
    
    private static async Task Duel(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var casino = (CasinoService)Services.Get(ServiceId.Casino);

        var err = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (err != ErrorCode.None) {
            await ErrorHandler.ReplyWithError(err, chatMessage, client);
            return;
        } if (args.Length < 2) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        if (!long.TryParse(args[0], out var quantity)) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        var userId = await _bot.Api.GetUserId(args[1], client.Credentials, (_, message) => {
                                                                            _logger.Log(LogLevel.Error, message);
                                                                        });
        if (userId == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.AccountNotFound, chatMessage, client);
            return;
        }
        
        var result = casino.CreateDuel(chatMessage.UserId, userId, quantity);
        if (!result.Ok) {
            await ErrorHandler.ReplyWithError(result.Error, chatMessage, client);
            return;
        }
        
        await client.SendMessage($"Вы вызвали {args[1]} на дуэль со ставкой {args[0]} {Declensioner.Points(quantity)}.", chatMessage.Id);
    }
    
    private static async Task AcceptDuel(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var casino = (CasinoService)Services.Get(ServiceId.Casino);

        var userId = string.Empty;
        if (args.Count > 0) {
            userId = await _bot.Api.GetUserId(args[0], client.Credentials,
                                               (_, message) => { _logger.Log(LogLevel.Error, message); });
            if (userId == null) {
                await ErrorHandler.ReplyWithError(ErrorCode.AccountNotFound, chatMessage, client);
                return;
            }
        }

        var result = casino.AcceptDuel(chatMessage.UserId, userId);
        if (!result.Ok) {
            await ErrorHandler.ReplyWithError(result.Error, chatMessage, client);
            return;
        } if (result.Value == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
            return;
        }

        var duelResult = result.Value.Value;
        var winnerUsername = await _bot.Api.GetUserName(duelResult.UserId, client.Credentials, displayName: true,
                                               (_, message) => {
                                                   _logger.Log(LogLevel.Error, message);
                                               });
        if (winnerUsername == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.AccountNotFound, chatMessage, client);
            return;
        }

        var looserId = duelResult.UserId.Equals(chatMessage.UserId)?
                           userId :
                           chatMessage.UserId;
        var looserUsername = await _bot.Api.GetUserName(looserId, client.Credentials, displayName: true, 
                                                     (_, message) => {
                                                         _logger.Log(LogLevel.Error, message);
                                                     });
        if (looserUsername == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.AccountNotFound, chatMessage, client);
            return;
        }
        
        var random = Random.Shared.Next(0, 3);
        switch (random) {
            case 0: {
                await client.SendMessage($"{winnerUsername} выигрывает в дуэли и получает {duelResult.Win} {Declensioner.Points(duelResult.Win)}");
                break;
            }
            case 1: {
                await client.SendMessage($"{winnerUsername} залутал {duelResult.Win} {Declensioner.Points(duelResult.Win)}", chatMessage.Id);
                break;
            }
            default: {
                await client.SendMessage($"{winnerUsername} заовнил {looserUsername} и выиграл {duelResult.Win} {Declensioner.Points(duelResult.Win)}", chatMessage.Id);
                break;
            }
        }
    }
    
    private static async Task DeclineDuel(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var casino = (CasinoService)Services.Get(ServiceId.Casino);
        
        var userId = string.Empty;
        if (args.Count > 0) {
            userId = await _bot.Api.GetUserId(args[1], client.Credentials,
                                           (_, message) => { _logger.Log(LogLevel.Error, message); });
            if (userId == null) {
                await ErrorHandler.ReplyWithError(ErrorCode.AccountNotFound, chatMessage, client);
                return;
            }
        }

        var result = casino.DeclineDuel(chatMessage.UserId, userId);
        if (!result.Ok) {
            await ErrorHandler.ReplyWithError(result.Error, chatMessage, client);
            return;
        }
        
        await client.SendMessage("Дуэль отменена.", chatMessage.Id);
    }
    
    private static async Task RemoveDuels(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var casino = (CasinoService)Services.Get(ServiceId.Casino);

        var result = casino.RemoveDuels(chatMessage.UserId);
        if (!result.Ok) {
            await ErrorHandler.ReplyWithError(result.Error, chatMessage, client);
            return;
        }
        
        await client.SendMessage("Все ваши дуэли отменены.", chatMessage.Id);
    }
    
    private static async Task ListDuels(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var casino = (CasinoService)Services.Get(ServiceId.Casino);

        var result = casino.ListDuels(chatMessage.UserId);
        if (!result.Ok) {
            await ErrorHandler.ReplyWithError(result.Error, chatMessage, client);
            return;
        } if (result.Value == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
            return;
        }

        var duels = result.Value;
        var reply = new List<string>();
        
        for (var i = 0; i < duels.Count; ++i) { 
            var duel = duels[i];
            var username = await _bot.Api.GetUserName(duel.Subject, client.Credentials, displayName: true, 
                                                   (_, message) => {
                                                       _logger.Log(LogLevel.Error, message);
                                                   });
            if (username == null) {
                await ErrorHandler.ReplyWithError(ErrorCode.AccountNotFound, chatMessage, client);
                return;
            }
            
            reply.Add($"{i+1}. {username} - {duel.Quantity} {(i >= duels.Count - 1? string.Empty : "\\")} ");
        }

        await SendPagedReply(reply, cmdArgs, _chatCmds.Options.SendWhisperIfPossible);
    }
    
    private static async Task Balance(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var bank = (BankService)Services.Get(ServiceId.Bank);

        if (!bank.GetAccount(chatMessage.UserId, out var account) || account == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.AccountNotFound, chatMessage, client);
            return;
        }

        var arrow = account.Gain >= 0 ? "↑" : "↓";
        await client.SendMessage($"Баланс: {account.Money} {Declensioner.Points(account.Money)} | За все время: {Math.Abs(account.Gain)}{arrow}.", chatMessage.Id);
    }
    
    private static async Task Shop(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;

        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var shop = (ShopService)Services.Get(ServiceId.Shop);
        var lots = shop.Lots;

        var reply = lots.Select(
                                (lot, i) => $"{i + 1}. {lot.Name} - {lot.Cost}" +
                                            $"{(lot.Buyers.TryGetValue(chatMessage.UserId, out var val)? $"({val})" : string.Empty)}" +
                                            $" {(i >= lots.Count - 1 ? string.Empty : "/")}")
                        .ToList();

        await SendPagedReply(reply, cmdArgs, _chatCmds.Options.SendWhisperIfPossible);
    }

    private static async Task ShopAdd(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client == null) return;

        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var shop = (ShopService)Services.Get(ServiceId.Shop);

        var err = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (err != ErrorCode.None) {
            await ErrorHandler.ReplyWithError(err, chatMessage, client);
            return;
        }

        if (args.Length < 2) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var lotName = args[0];
        if (!long.TryParse(args[1], out var cost)) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }
        
        var result = shop.Add(lotName, cost);
        if (!result) {
            await ErrorHandler.ReplyWithError(ErrorCode.AlreadyExists, chatMessage, client);
            return;
        }
        
        await client.SendMessage($"Успешно создан лот '{lotName}'", chatMessage.Id);
    }
    
    private static async Task ShopRemove(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client == null) return;

        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var shop = (ShopService)Services.Get(ServiceId.Shop);

        if (args.Count <= 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
        
        var result = shop.Remove(args[0]);
        if (!result) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        }
        
        await client.SendMessage($"Лот '{args[0]}' успешно удалён", chatMessage.Id);
    }
    
    private static async Task Buy(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client == null) return;

        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var shop = (ShopService)Services.Get(ServiceId.Shop);

        var err = ParseSemicolonSeparatedArgs(cmdArgs, out var args);
        if (err != ErrorCode.None) {
            await ErrorHandler.ReplyWithError(err, chatMessage, client);
            return;
        } if (args.Length < 1) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        var lotName = args[0];
        if (args.Length < 2 || !long.TryParse(args[1], out var amount)) {
            amount = 1;
        }
        
        var result = shop.Buy(chatMessage.UserId, lotName, amount);
        if (!result.Ok) {
            await ErrorHandler.ReplyWithError(result.Error, chatMessage, client);
            return;
        } if (result.Value == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
            return;
        }

        var lot = result.Value;
        await client.SendMessage($"Куплено '{lot.Name}'. Всего: {lot.Buyers[chatMessage.UserId]}", chatMessage.Id);
    }
    
    private static async Task Use(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var args = cmdArgs.Parsed.ArgumentsAsList;
        var shop = (ShopService)Services.Get(ServiceId.Shop);

        if (args.Count <= 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
        
        var sb = new StringBuilder();
        for (var i = 0; i < args.Count; i++) {
            if (i == args.Count-1) {
                sb.Append($"{args[i]}");
                break;
            }
            sb.Append($"{args[i]} ");
        }

        var lotName = sb.ToString();
        var lot = shop.Get(lotName);
        if (lot == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.NotFound, chatMessage, client);
            return;
        } if (lot.IsDefault) {
            await ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
            return;
        }
        
        var result = shop.Use(chatMessage.UserId, lotName);
        if (!result.Ok) {
            await ErrorHandler.ReplyWithError(result.Error, chatMessage, client);
            return;
        } if (result.Value == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
            return;
        }
        
        await client.SendMessage($"Использовано {lot.Name}", chatMessage.Id);
    }
    
    private static async Task Give(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var bank = (BankService)Services.Get(ServiceId.Bank);

        var err = ParseSemicolonSeparatedArgs(cmdArgs, out var parsed);
        
        if (err != ErrorCode.None) {
            await ErrorHandler.ReplyWithError(err, chatMessage, client);
            return;
        }
        if (parsed.Length < 2) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }

        if (!long.TryParse(parsed[0], out var money)) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }
        var username = parsed[1];

        var userId = await _bot.Api.GetUserId(username, client.Credentials, async void (_, message) => {
                                                                             try {
                                                                                 await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
                                                                                 _logger.Log(LogLevel.Error, message);
                                                                             }
                                                                             catch (Exception e) {
                                                                                 _logger.Log(LogLevel.Error, e.Message);
                                                                             }
                                                                         });
        if (string.IsNullOrEmpty(userId)) return;

        var result = bank.Give(userId, chatMessage.UserId, money);
        if (!result.Ok) {
            await ErrorHandler.ReplyWithError(result.Error, chatMessage, client);
            return;
        }
        await client.SendMessage($"Успешно отправлено {money} {Declensioner.Points(money)} пользователю {username}", chatMessage.Id);
    }
    
    private static async Task Giveaway(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var bank = (BankService)Services.Get(ServiceId.Bank);
        var accounts = bank.Options.GetAccounts();

        if (!bank.GetAccount(chatMessage.UserId, out var account) || account == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.AccountNotFound, chatMessage, client);
            return;
        }
        
        var err = ParseIntArg(cmdArgs, out var temp);
        long money = temp;
        if (err != ErrorCode.None) {
            money = account.Money;
        }

        if (account.Money < money) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewPoints, chatMessage, client);
            return;
        }
        
        var random = Random.Shared;
        var amount = accounts.Count switch {
                         > 1 => (int)random.NextInt64(2, Math.Min(money, accounts.Count)%int.MaxValue),
                         _   => 0,
                     };

        if (amount <= 1) {
            await client.SendMessage("Некому раздавать.", chatMessage.Id);
            return;
        }
        if (amount > 6) amount = 6;
        
        var quantityPerEach = money / amount;
        if (quantityPerEach <= 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewPoints, chatMessage, client);
            return;
        }

        var map = new Dictionary<string, long>();
        var left = money;
        var retries = 0;
        
        for (var i = 0; i < amount; ++i) {
            var index = random.Next(0, accounts.Count);
            var (_, receiver) = accounts.ElementAt(index);

            if (receiver.UserId.Equals(chatMessage.UserId)) {
                if (index - 1 < 0 && index + 1 < accounts.Count) ++index;
                else if (index - 1 >= 0) --index;
                else {
                    await client.SendMessage("Некому раздавать.", chatMessage.Id);
                    return;
                }
                
                (_, receiver) = accounts.ElementAt(index);
            }
            
            var result = bank.Give(receiver.UserId, chatMessage.UserId, quantityPerEach);
            if (!result.Ok) {
                if (retries++ >= 3) break;
                --i;
                continue;
            }

            if (!map.TryAdd(receiver.UserId, quantityPerEach)) {
                map[receiver.UserId] += quantityPerEach;
            }

            left -= quantityPerEach;
            
            var tempAmount = amount - (i + 1);
            if (tempAmount < 1) tempAmount = 1;
            
            quantityPerEach = left / tempAmount;
        }
        
        var sb = new StringBuilder();
        for (var i = 0; i < map.Count; ++i) {
            var (receiver, points) = map.ElementAt(i);
            var username = await _bot.Api.GetUserName(receiver, client.Credentials, true, (_, msg) => {
                                                                                     ErrorHandler.LogMessage(LogLevel.Error, msg);
                                                 });
            if (username == null) continue;
            
            sb.Append($"{username} - {points} {(i >= map.Count-1? string.Empty : "/ ")}");
        }
        
        await client.SendMessage($"Роздано: {money} {Declensioner.Points(money)} - {sb}", chatMessage.Id);
    }
    
    private static async Task BankListRewards(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var bank = (BankService)Services.Get(ServiceId.Bank);

        var rewards = bank.Options.GetRewards();
        var reply = new List<string>();
        
        for (var i = 0; i < rewards.Count; ++i) {
            var (id, quantity) = rewards.ElementAt(i);
            reply.Add($"{i+1}. {id} - {quantity} {(i >= rewards.Count - 1? string.Empty : "\\")} ");
        }

        await SendPagedReply(reply, cmdArgs, _chatCmds.Options.SendWhisperIfPossible);
    }
    
    private static async Task BankCreateReward(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var bank = (BankService)Services.Get(ServiceId.Bank);

        var err = ParseIntArg(cmdArgs, out var quantity);
        if (err != ErrorCode.None) {
            await ErrorHandler.ReplyWithError(err, chatMessage, client);
            return;
        }

        var rewardId = await _bot.Api.CreateChannelReward(
                                                       title: $"+{quantity}",
                                                       cost: quantity,
                                                       credentials: client.Credentials,
                                                       prompt: $"Дает {quantity} фантиков.",
                                                       isEnabled: true,
                                                       userInputRequired: false,
                                                       skipQueue: false,
                                                       callback: (_, message) => {
                                                                     _logger.Log(LogLevel.Error, message);
                                                                 }
                                                       );
        if (rewardId == null) {
            await client.SendMessage("Не удалось создать награду.", chatMessage.Id);
            return;
        }
        
        bank.Options.AddReward(rewardId, quantity);
        await client.SendMessage($"Награда создана успешно. ({rewardId})", chatMessage.Id);
    }
    
    private static async Task BankDeleteReward(ChatCmdArgs cmdArgs) {
        if (await ErrorIfNotFullyAuthorized()) {
            return;
        }
        
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;
        
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var bank = (BankService)Services.Get(ServiceId.Bank);

        var err = ParseIntArg(cmdArgs, out var index);
        if (err != ErrorCode.None) {
            await ErrorHandler.ReplyWithError(err, chatMessage, client);
            return;
        }

        if (index <= 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.InvalidInput, chatMessage, client);
            return;
        }

        var (rewardId, _) = bank.Options.GetReward(index-1);
        
        var result = await _bot.Api.DeleteChannelReward(
                                                        rewardId,
                                                        client.Credentials,
                                                        (_, message) => { 
                                                            _logger.Log(LogLevel.Error, message);
                                                        }
                                                      );
        if (!result) {
            await client.SendMessage("Не удалось удалить награду.", chatMessage.Id);
            return;
        }
        
        bank.Options.RemoveReward(rewardId);
        await client.SendMessage("Награда удалена успешно.", chatMessage.Id);
    }
    
    private static async Task Evaluate(ChatCmdArgs cmdArgs) {
        var client = _bot.GetClient();
        if (client == null) return;

        var args = cmdArgs.Parsed.ArgumentsAsList;
        var chatMessage = cmdArgs.Parsed.ChatMessage;
        var interpreter = (InterpreterService)Services.Get(ServiceId.Interpreter);

        if (args.Count <= 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.TooFewArgs, chatMessage, client);
            return;
        }
        
        var result = interpreter.Evaluate(cmdArgs.Parsed.CommandMessage);
        if (result is { Ok: false, Error: not null, }) {
            await client.SendMessage(result.Error, chatMessage.Id);
            return;
        } if (result.Value == null) {
            await ErrorHandler.ReplyWithError(ErrorCode.SmthWentWrong, chatMessage, client);
            return;
        }
        
        await client.SendMessage(result.Value, chatMessage.Id);
    }
    
    private static Task PageTerminator(ChatCmdArgs cmdArgs) {
        return Task.CompletedTask;
    }
    
    private static async Task SendPagedReply(List<string> reply, ChatCmdArgs cmdArgs, bool whisper = false) {
        var client = _bot.GetClient();
        if (client?.Credentials == null) return;

        var chatMessage = cmdArgs.Parsed.ChatMessage;

        if (reply.Count <= 0) {
            await ErrorHandler.ReplyWithError(ErrorCode.Empty, chatMessage, client);
            return;
        }

        var page = ParsePage(int.MaxValue, cmdArgs);
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
                await client.SendMessage(message.ToString(), chatMessage.Id);
                break;
            }
            case true: {
                await client.SendWhisper(message.ToString(), chatMessage.UserId);
                break;
            }
        }
    }

    private static bool GetIntArg(ChatCmdArgs cmdArgs, string name, out int value) {
        value = 0;
        
        var args = cmdArgs.Parsed.ArgumentsAsList;
        
        var index = args.IndexOf(name);
        if (index < 0 || index >= args.Count) {
            return false;
        }
        
        if (index - 1 < args.Count) {
            value = int.Parse(args[index + 1]);
        }

        return true;
    }
    
    private static ErrorCode ParseIntArg(ChatCmdArgs cmdArgs, out int value, int index = 0) {
        var args = cmdArgs.Parsed.ArgumentsAsList;
        value = -1;

        if (index < 0) {
            return ErrorCode.InvalidInput;
        }
        if (args.Count <= index) {
            return ErrorCode.TooFewArgs;
        }

        return int.TryParse(args[index], out value)?
                   ErrorCode.None :
                   ErrorCode.InvalidInput;
    }

    private static int ParsePage(int high, ChatCmdArgs cmdArgs) {
        var args = cmdArgs.Parsed.ArgumentsAsList;

        var found = GetIntArg(cmdArgs, "--page", out var page);

        --page;
        
        if (args.Count > 0 
         && !found
         && int.TryParse(args[0], out var tempPage)) {
            page = tempPage;
        }
        
        if (page >= high) page = high - 1;
        if (page < 0) page = 0;

        return page;
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

    // TODO: Move to a separate service
    
    private enum Emote {
        Sad,
        Wait,
        Rizz,
        Jail,
        Laugh,
        Aga,
        Like,
    }
    
    private static string Get7TvEmote(Emote emote) {
        if (!_chatCmds.Options.Use7Tv) {
            return string.Empty;
        }

        return emote switch {
            Emote.Sad   => "Sadding",
            Emote.Wait  => "Wait",
            Emote.Rizz  => "RIZZ",
            Emote.Jail  => "SillyJail",
            Emote.Laugh => "GAGAGA",
            Emote.Aga   => "waga",
            Emote.Like  => "LIKE",
            
            _ => string.Empty,
        };
    }

    private static async Task<bool> ErrorIfNotFullyAuthorized(ChatMessage? message = null) {
        var client = _bot.GetClient();
        if (client == null) {
            return true;
        }

        var authLevel = _bot.GetAuthLevel();

        if (authLevel == AuthLevel.Full) {
            return false;
        }

        switch (message) {
                
            case not null: {
                await ErrorHandler.ReplyWithError(ErrorCode.NotFullyAuthorized, message, client);
                break;
            }
                
            case null: {
                await ErrorHandler.SendError(ErrorCode.NotFullyAuthorized, client);
                break;
            }
            
        }
        
        return true;
    }
}