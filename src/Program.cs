using System.Text;
using ChatBot.CLI;
using ChatBot.Services.ai;
using ChatBot.Services.chat_commands;
using ChatBot.Services.chat_logs;
using ChatBot.Services.demon_list;
using ChatBot.Services.game_requests;
using ChatBot.Services.level_requests;
using ChatBot.Services.logger;
using ChatBot.Services.message_filter;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.moderation;
using ChatBot.Services.presets;
using ChatBot.Services.Static;
using ChatBot.Services.text_generator;
using ChatBot.Services.translator;

namespace ChatBot;

internal static class Program {
    private static Cli _cli = null!;
    private static bot.ChatBot _bot = null!;

    private static bool _forcedToRender;
    
    
    private static async Task Main(string[] args) {
        var autoInit = false;
        var resetDefaultCmds = false;
        
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        foreach (var arg in args) {
            switch (arg) {
                case "--auto-init": {
                    autoInit = true;
                    break;
                }
                case "--res-def-cmds": {
                    resetDefaultCmds = true;
                    break;
                }
            }
        }
        
        _bot = new bot.ChatBot();
        
        ServiceManager.InitServices(_bot, []);
        var cliData = new CliData(_bot);
        _cli = new Cli(cliData);
        if (autoInit) {
            _bot.Options.Load();
            _bot.Start();
        }
        if (resetDefaultCmds) {
            CommandsList.SetDefaults();
        }
        _cli.RenderNodes();

        var renderTask = Task.Run(Render);
        await renderTask;

        Console.ReadLine();
    }

    public static bot.ChatBot GetBot() {
        return _bot;
    }
    
    public static void ForceToRender() {
        _forcedToRender = true;
    }
    
    private static Task Render() {
        while (true) {
            switch (_forcedToRender) {
                case false when !Console.KeyAvailable:
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