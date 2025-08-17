using System.Text;
using ChatBot.bot;
using ChatBot.bot.services.Static;
using ChatBot.cli;

namespace ChatBot;

internal static class Program {
    private static Cli _cli = null!;

    private static bool _forcedToRender;

    public static bool AutoInit { get; private set; }


    private static void Main(string[] args) {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        foreach (var arg in args) {
            AutoInit = arg switch {
                           "--auto-init" => true,
                           _             => AutoInit,
                       };
        }

        var bot = TwitchChatBot.Instance;
        
        ServiceManager.LoadServices();
        _cli = new Cli(new CliData());
        
        if (AutoInit) {
            bot.Start();
        }
        
        _cli.RenderNodes();

        Render();
        Console.ReadLine();
    }
    
    public static void ForceToRender() {
        _forcedToRender = true;
    }
    
    private static void Render() {
        while (true) {
            switch (_forcedToRender) {
                case false when !Console.KeyAvailable:
                    Thread.Sleep(50);
                    continue;
                case false:
                    int.TryParse(Console.ReadLine() ?? "0", out var index);
                    _cli.ActivateNode(index);
                    break;
            } 
            
            Console.Clear();
            _cli.RenderNodes();
            _forcedToRender = false;
        }
    }
}