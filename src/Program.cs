using ChatBot.CLI;
using ChatBot.Services;
using ChatBot.Services.game_requests;
using ChatBot.Services.message_randomizer;

namespace ChatBot;

internal static class Program {
    private static void Main() {
        ServiceManager.InitServices();
        
        var bot = new twitchAPI.ChatBot();
        var cliData = new CliData(
                                  bot,
                                  (GameRequestsService)ServiceManager.GetService("GameRequests"),
                                  (MessageRandomizerService)ServiceManager.GetService("MessageRandomizer"));
        var cliHandler = new CliHandler(cliData);
        
        while (true) {
            Console.Clear();
            cliHandler.RenderNodes();
            
            int.TryParse(Console.ReadLine(), out var index);
            cliHandler.ActivateNode(index-1);
        }

    }
}