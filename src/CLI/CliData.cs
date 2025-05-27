using System.Text.RegularExpressions;
using ChatBot.Services.chat_commands;
using ChatBot.Services.game_requests;
using ChatBot.Services.message_filter;
using ChatBot.Services.message_randomizer;

namespace ChatBot.CLI;

public class CliData {
    public bot.ChatBot Bot { get; }
    public GameRequestsService GameRequests { get; }
    public MessageRandomizerService MessageRandomizer { get; }
    public ChatCommandsService ChatCommands { get; }
    public MessageFilterService MessageFilter { get; }
    

    public CliData(
        bot.ChatBot bot,
        GameRequestsService gameRequests,
        MessageRandomizerService messageRandomizer,
        ChatCommandsService chatCommands,
        MessageFilterService messageFilter
    ) {
        Bot = bot;
        GameRequests = gameRequests;
        MessageRandomizer = messageRandomizer;
        ChatCommands = chatCommands;
        MessageFilter = messageFilter;
    }
}