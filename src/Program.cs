using ChatBot.CLI;
using ChatBot.Services.chat_commands;
using ChatBot.Services.game_requests;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.Static;

namespace ChatBot;

internal static class Program {
    private static CliHandler _cliHandler = null!;
    
    
    private static async Task Main() {
        var bot = new twitchAPI.ChatBot();
        
        ServiceManager.InitServices(bot);
        var cliData = new CliData(
                                  bot,
                                  (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests),
                                  (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer),
                                  (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands)
                                  );
        _cliHandler = new CliHandler(cliData);
        _cliHandler.RenderNodes();

        var renderTask = Task.Run(Render);
        await renderTask;

        Console.ReadLine();
    }

    private static Task Render() {
        while (true) {
            if (!Console.KeyAvailable) continue;
            
            int.TryParse(Console.ReadLine(), out var index);
            _cliHandler.ActivateNode(index-1);
            Console.Clear();
            _cliHandler.RenderNodes();
        }
    }
}