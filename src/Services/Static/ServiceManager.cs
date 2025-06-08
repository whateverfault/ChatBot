using ChatBot.bot.interfaces;
using ChatBot.Services.chat_commands;
using ChatBot.Services.chat_logs;
using ChatBot.Services.interfaces;
using ChatBot.Services.level_requests;
using ChatBot.Services.logger;
using ChatBot.Services.message_filter;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.moderation;
using ChatBot.Services.text_generator;

namespace ChatBot.Services.Static;

public static class ServiceManager {
    private static readonly Dictionary<string, (Service, ServiceEvents)> _services = new() { 
                                                                                               {
                                                                                                   ServiceName.MessageRandomizer,
                                                                                                   (
                                                                                                       new MessageRandomizerService(),
                                                                                                       new MessageRandomizerEvents()
                                                                                                   ) 
                                                                                               }, 
                                                                                               {
                                                                                                   ServiceName.ChatCommands,
                                                                                                   (
                                                                                                       new ChatCommandsService(),
                                                                                                       new ChatCommandsEvents()
                                                                                                   )
                                                                                               },
                                                                                               {
                                                                                                   ServiceName.MessageFilter,
                                                                                                   (
                                                                                                       new MessageFilterService(),
                                                                                                       new MessageFilterEvents()
                                                                                                   )
                                                                                               },
                                                                                               {
                                                                                                   ServiceName.Moderation,
                                                                                                   (
                                                                                                       new ModerationService(),
                                                                                                       new ModerationEvents()
                                                                                                   )
                                                                                               },
                                                                                               {
                                                                                                   ServiceName.Logger,
                                                                                                   (
                                                                                                       new LoggerService(),
                                                                                                       new LoggerEvents()
                                                                                                   )
                                                                                               },
                                                                                               {
                                                                                                   ServiceName.TextGenerator,
                                                                                                   (
                                                                                                       new TextGeneratorService(),
                                                                                                       new TextGeneratorEvents()
                                                                                                   )
                                                                                               },
                                                                                               {
                                                                                                   ServiceName.ChatLogs,
                                                                                                   (
                                                                                                       new ChatLogsService(),
                                                                                                       new ChatLogsEvents()
                                                                                                   )
                                                                                               },
                                                                                               {
                                                                                                   ServiceName.LevelRequests,
                                                                                                   (
                                                                                                       new LevelRequestsService(),
                                                                                                       new LevelRequestsEvents()
                                                                                                   )
                                                                                               },
                                                                                           };


    public static void InitServices(Bot bot) {
        foreach (var (_, (service, events)) in _services) {
            service.Init(bot);

            events.Init(service, bot);
            events.Subscribe();
        }
    }

    public static Service GetService(string key) {
        return _services[key].Item1;
    }
}