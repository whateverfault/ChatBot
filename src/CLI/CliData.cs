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
using ChatBot.bot.services.scopes;
using ChatBot.bot.services.shop;
using ChatBot.bot.services.Static;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.telegram_notifications;
using ChatBot.bot.services.text_generator;
using ChatBot.bot.services.translator;

namespace ChatBot.cli;

public class CliData {
    public TwitchChatBot Bot { get; } = TwitchChatBot.Instance;
    public MessageRandomizerService MessageRandomizer { get; } = (MessageRandomizerService)Services.Get(ServiceId.MessageRandomizer);
    public ChatCommandsService ChatCommands { get; } = (ChatCommandsService)Services.Get(ServiceId.ChatCommands);
    public MessageFilterService MessageFilter { get; } = (MessageFilterService)Services.Get(ServiceId.MessageFilter);
    public ModerationService Moderation { get; } = (ModerationService)Services.Get(ServiceId.Moderation);
    public LoggerService Logger { get; } = (LoggerService)Services.Get(ServiceId.Logger);
    public ChatLogsService ChatLogs { get; } = (ChatLogsService)Services.Get(ServiceId.ChatLogs);
    public TextGeneratorService TextGenerator { get; } = (TextGeneratorService)Services.Get(ServiceId.TextGenerator);
    public LevelRequestsService LevelRequests { get; } = (LevelRequestsService)Services.Get(ServiceId.LevelRequests);
    public PresetsService Presets { get; } = (PresetsService)Services.Get(ServiceId.Presets);
    public DemonListService DemonList { get; } = (DemonListService)Services.Get(ServiceId.DemonList);
    public AiService Ai { get; } = (AiService)Services.Get(ServiceId.Ai);
    public TranslatorService Translator { get; } = (TranslatorService)Services.Get(ServiceId.Translator);
    public GameRequestsService GameRequests { get; } = (GameRequestsService)Services.Get(ServiceId.GameRequests);
    public StreamStateCheckerService StreamStateChecker { get; } = (StreamStateCheckerService)Services.Get(ServiceId.StreamStateChecker);
    public TgNotificationsService TgNotifications { get; } = (TgNotificationsService)Services.Get(ServiceId.TgNotifications);
    public ChatAdsService ChatAds { get; } = (ChatAdsService)Services.Get(ServiceId.ChatAds);
    public CasinoService Casino { get; } = (CasinoService)Services.Get(ServiceId.Casino);
    public BankService Bank { get; } = (BankService)Services.Get(ServiceId.Bank);
    public ShopService Shop { get; } = (ShopService)Services.Get(ServiceId.Shop);
    public ScopesService Scopes { get; } = (ScopesService)Services.Get(ServiceId.Scopes);
}