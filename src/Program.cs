using System.Text;
using ChatBot.bot;
using ChatBot.bot.services.Static;
using ChatBot.cli;

namespace ChatBot;

internal static class Program {
    private static async Task Main(string[] args) {
        var autoInit = false;
        
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        foreach (var arg in args) {
            autoInit = arg switch {
                           "--auto-init" => true,
                           _             => autoInit,
                       };
        }
        ServiceManager.LoadServices();
        
        var bot = TwitchChatBot.Instance;
        var cli = new Cli(new CliData());
        
        if (autoInit) _ = bot.StartAsync();
        
        await cli.StartRenderer();
    }
}