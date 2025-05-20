using ChatBot.Services.game_requests;
using ChatBot.Services.message_randomizer;
using ChatBot.twitchAPI;
using ChatBot.twitchAPI.interfaces;

namespace ChatBot.CLI;

public class CliData {
    public Bot Bot { get; }
    public GameRequestsService GameRequests { get; }
    public MessageRandomizerService MessageRandomizer { get; }
    

    public CliData(
        Bot bot,
        GameRequestsService gameRequests,
        MessageRandomizerService messageRandomizer
        ) {
        Bot = bot;
        GameRequests = gameRequests;
        MessageRandomizer = messageRandomizer;
    }
}