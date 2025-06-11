using ChatBot.Services.chat_commands;
using ChatBot.Services.chat_logs;
using ChatBot.Services.level_requests;
using ChatBot.Services.logger;
using ChatBot.Services.message_filter;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.moderation;
using ChatBot.Services.presets;
using ChatBot.Services.text_generator;

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
    

    public CliData(
        bot.ChatBot bot,
        MessageRandomizerService messageRandomizer,
        ChatCommandsService chatCommands,
        MessageFilterService messageFilter,
        ModerationService moderation,
        LoggerService logger,
        ChatLogsService chatLogs, 
        TextGeneratorService textGenerator, LevelRequestsService levelRequests, PresetsService presets) {
        Bot = bot;
        MessageRandomizer = messageRandomizer;
        ChatCommands = chatCommands;
        MessageFilter = messageFilter;
        Moderation = moderation;
        Logger = logger;
        ChatLogs = chatLogs;
        TextGenerator = textGenerator;
        this.LevelRequests = levelRequests;
        Presets = presets;
    }
}