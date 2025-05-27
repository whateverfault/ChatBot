using ChatBot.CLI;
using ChatBot.Services.chat_commands;
using ChatBot.Services.game_requests;
using ChatBot.Services.message_filter;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.Static;

namespace ChatBot;

internal static class Program {
    private static Cli _cli = null!;


    private static async Task Main() {
        var bot = new bot.ChatBot();

        ServiceManager.InitServices(bot);
        var cliData = new CliData(
                                  bot,
                                  (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests),
                                  (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer),
                                  (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands),
                                  (MessageFilterService)ServiceManager.GetService(ServiceName.MessageFilter)
                                 );
        _cli = new Cli(cliData);
        _cli.RenderNodes();

        var renderTask = Task.Run(Render);
        await renderTask;

        Console.ReadLine();
    }

    private static Task Render() {
        while (true) {
            if (!Console.KeyAvailable) {
                continue;
            }

            int.TryParse(Console.ReadLine(), out var index);
            _cli.ActivateNode(index);
            Console.Clear();
            _cli.RenderNodes();
        }
    }
}