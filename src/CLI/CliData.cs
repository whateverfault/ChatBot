using ChatBot.bot.chat_bot;
using ChatBot.bot.services.ai;
using ChatBot.bot.services.bank;
using ChatBot.bot.services.casino;
using ChatBot.bot.services.chat_ads;
using ChatBot.bot.services.chat_commands;
using ChatBot.bot.services.chat_logs;
using ChatBot.bot.services.demon_list;
using ChatBot.bot.services.game_requests;
using ChatBot.bot.services.level_requests;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.message_filter;
using ChatBot.bot.services.message_randomizer;
using ChatBot.bot.services.moderation;
using ChatBot.bot.services.presets;
using ChatBot.bot.services.shop;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.telegram;
using ChatBot.bot.services.text_generator;
using ChatBot.bot.services.translator;

namespace ChatBot.cli;

public class CliData {
    public TwitchChatBot Bot { get; }
    public MessageRandomizerService MessageRandomizer { get; }
    public ChatCommandsService ChatCommands { get; }
    public MessageFilterService MessageFilter { get; }
    public ModerationService Moderation { get; }
    public LoggerService Logger { get; }
    public ChatLogsService ChatLogs { get; }
    public TextGeneratorService TextGenerator { get; }
    public LevelRequestsService LevelRequests { get; }
    public PresetsService Presets { get; }
    public DemonListService DemonList { get; }
    public AiService Ai { get; }
    public TranslatorService Translator { get; }
    public GameRequestsService GameRequests { get; }
    public StreamStateCheckerService StreamStateChecker { get; }
    public TgNotificationsService TgNotifications { get; }
    public ChatAdsService ChatAds { get; }
    public CasinoService Casino { get; }
    public BankService Bank { get; }
    public ShopService Shop { get; }


    public CliData() {
        Bot = TwitchChatBot.Instance;
        MessageRandomizer = (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer);
        ChatCommands = (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands);
        MessageFilter = (MessageFilterService)ServiceManager.GetService(ServiceName.MessageFilter);
        Moderation = (ModerationService)ServiceManager.GetService(ServiceName.Moderation);
        Logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
        ChatLogs = (ChatLogsService)ServiceManager.GetService(ServiceName.ChatLogs);
        TextGenerator = (TextGeneratorService)ServiceManager.GetService(ServiceName.TextGenerator);
        LevelRequests = (LevelRequestsService)ServiceManager.GetService(ServiceName.LevelRequests);
        Presets = (PresetsService)ServiceManager.GetService(ServiceName.Presets);
        DemonList = (DemonListService)ServiceManager.GetService(ServiceName.DemonList);
        Ai = (AiService)ServiceManager.GetService(ServiceName.Ai);
        Translator = (TranslatorService)ServiceManager.GetService(ServiceName.Translator);
        GameRequests = (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests);
        StreamStateChecker = (StreamStateCheckerService)ServiceManager.GetService(ServiceName.StreamStateChecker);
        TgNotifications = (TgNotificationsService)ServiceManager.GetService(ServiceName.TgNotifications);
        ChatAds = (ChatAdsService)ServiceManager.GetService(ServiceName.ChatAds);
        Casino = (CasinoService)ServiceManager.GetService(ServiceName.Casino);
        Bank = (BankService)ServiceManager.GetService(ServiceName.Bank);
        Shop = (ShopService)ServiceManager.GetService(ServiceName.Shop);
    }
}