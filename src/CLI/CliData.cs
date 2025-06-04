using ChatBot.Services.chat_commands;
using ChatBot.Services.logger;
using ChatBot.Services.message_filter;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.moderation;

namespace ChatBot.CLI;

public class CliData {
    public bot.ChatBot Bot { get; }
    public MessageRandomizerService MessageRandomizer { get; }
    public ChatCommandsService ChatCommands { get; }
    public MessageFilterService MessageFilter { get; }
    public ModerationService Moderation { get; }
    public LoggerService Logger { get; }
    

    public CliData(
        bot.ChatBot bot,
        MessageRandomizerService messageRandomizer,
        ChatCommandsService chatCommands,
        MessageFilterService messageFilter,
        ModerationService moderation,
        LoggerService logger) {
        Bot = bot;
        MessageRandomizer = messageRandomizer;
        ChatCommands = chatCommands;
        MessageFilter = messageFilter;
        Moderation = moderation;
        Logger = logger;
    }
}