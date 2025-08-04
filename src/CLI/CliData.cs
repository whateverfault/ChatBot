using ChatBot.bot;
using ChatBot.services.ai;
using ChatBot.services.chat_ads;
using ChatBot.services.chat_commands;
using ChatBot.services.chat_logs;
using ChatBot.services.demon_list;
using ChatBot.services.game_requests;
using ChatBot.services.level_requests;
using ChatBot.services.logger;
using ChatBot.services.message_filter;
using ChatBot.services.message_randomizer;
using ChatBot.services.moderation;
using ChatBot.services.presets;
using ChatBot.services.Static;
using ChatBot.services.stream_state_checker;
using ChatBot.services.telegram;
using ChatBot.services.text_generator;
using ChatBot.services.translator;

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
    }
}