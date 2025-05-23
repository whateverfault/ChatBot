using System.Text.RegularExpressions;
using ChatBot.Services.chat_commands;
using ChatBot.Services.game_requests;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.regex;

namespace ChatBot.CLI;

public class CliData {
    public twitchAPI.ChatBot Bot { get; }
    public GameRequestsService GameRequests { get; }
    public MessageRandomizerService MessageRandomizer { get; }
    public ChatCommandsService ChatCommands { get; }
    public RegexService Regex { get; }
    

    public CliData(
        twitchAPI.ChatBot bot,
        GameRequestsService gameRequests,
        MessageRandomizerService messageRandomizer,
        ChatCommandsService chatCommands,
        RegexService regex
    ) {
        Bot = bot;
        GameRequests = gameRequests;
        MessageRandomizer = messageRandomizer;
        ChatCommands = chatCommands;
        Regex = regex;
    }
}