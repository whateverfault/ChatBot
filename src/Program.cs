using System.Text;
using ChatBot.CLI;
using ChatBot.Services.chat_commands;
using ChatBot.Services.game_requests;
using ChatBot.Services.logger;
using ChatBot.Services.message_filter;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.moderation;
using ChatBot.Services.Static;

namespace ChatBot;

internal static class Program {
    private static Cli _cli = null!;

    private static bool _forcedToRender;


    private static async Task Main() {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
        
        var bot = new bot.ChatBot();

        ServiceManager.InitServices(bot);
        var cliData = new CliData(
                                  bot,
                                  (GameRequestsService)ServiceManager.GetService(ServiceName.GameRequests),
                                  (MessageRandomizerService)ServiceManager.GetService(ServiceName.MessageRandomizer),
                                  (ChatCommandsService)ServiceManager.GetService(ServiceName.ChatCommands),
                                  (MessageFilterService)ServiceManager.GetService(ServiceName.MessageFilter),
                                  (ModerationService)ServiceManager.GetService(ServiceName.Moderation),
                                  (LoggerService)ServiceManager.GetService(ServiceName.Logger)
                                 );
        _cli = new Cli(cliData);
        _cli.RenderNodes();

        var renderTask = Task.Run(Render);
        await renderTask;

        Console.ReadLine();
    }

    public static void ForceToRender() {
        _forcedToRender = true;
    }
        
    
    private static Task Render() {
        while (true) {
            if (!_forcedToRender && !Console.KeyAvailable) {
                continue;
            }

            if (!_forcedToRender) {
                int.TryParse(Console.ReadLine(), out var index);
                _cli.ActivateNode(index);
            }
            Console.Clear();
            _cli.RenderNodes();
            _forcedToRender = false;
        }
    }
}