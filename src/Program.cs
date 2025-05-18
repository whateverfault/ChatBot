using ChatBot.Services;
using ChatBot.twitchAPI;

namespace ChatBot;

internal static class Program {
    private static void Main() {
        ServiceManager.InitServices();
        
        var bot = new Bot();
        bot.Start();
        Console.ReadLine();
        bot.Stop();
        
        ServiceManager.KillServices();
    }
}