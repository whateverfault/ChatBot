using ChatBot.Services.ai;
using ChatBot.Services.chat_commands;
using ChatBot.Services.chat_logs;
using ChatBot.Services.demon_list;
using ChatBot.Services.game_requests;
using ChatBot.Services.level_requests;
using ChatBot.Services.logger;
using ChatBot.Services.message_filter;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.moderation;
using ChatBot.Services.presets;
using ChatBot.Services.Static;
using ChatBot.Services.text_generator;
using ChatBot.Services.translator;

namespace ChatBot.CLI;

public class CliData {
    public bot.ChatBot Bot { get; }
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


    public CliData(bot.ChatBot bot) {
        Bot = bot;
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
    }
}