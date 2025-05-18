using ChatBot.Services;
using ChatBot.twitchAPI;

namespace ChatBot;

internal static class Program {
    private static void Main() {
        ServiceManager.InitServices();
        
        //var bot = new TwitchChatBot();
        var bot = new RandomMessagesBot();
        bot.Start();
        Console.ReadLine(); 
        
        ServiceManager.KillServices();
    }
}