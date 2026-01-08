using System.Net;
using System.Text;
using ChatBot.bot.chat_bot;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using ChatBot.cli;

namespace ChatBot;

internal static class Program {
    private static async Task Main(string[] args) {
        var autoInit = false;
        
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
        
        IoHandler.InputEncoding = Encoding.UTF8;
        IoHandler.OutputEncoding = Encoding.UTF8;
        
        foreach (var arg in args) {
            autoInit = arg switch {
                           "--auto-init" => true,
                           _             => autoInit,
                       };
        }
        
        Services.Load();
        
        var bot = TwitchChatBot.Instance;
        var cli = new Cli(new CliData());
        
        if (autoInit) _ = bot.Start();
        
        await cli.StartRenderer();
    }
}