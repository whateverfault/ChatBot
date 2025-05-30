using ChatBot.bot.interfaces;
using ChatBot.Services.chat_commands;
using ChatBot.Services.game_requests;
using ChatBot.Services.interfaces;
using ChatBot.Services.message_filter;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.moderation;

namespace ChatBot.Services.Static;

public static class ServiceManager {
    private static readonly Dictionary<string, (Service, ServiceEvents)> _services = new() {
                                                                                                {
                                                                                                   ServiceName.GameRequests,
                                                                                                   (
                                                                                                       new GameRequestsService(),
                                                                                                       new GameRequestsEvents()
                                                                                                   )
                                                                                               }, {
                                                                                                   ServiceName.MessageRandomizer,
                                                                                                   (
                                                                                                       new MessageRandomizerService(),
                                                                                                       new MessageRandomizerEvents()
                                                                                                   )
                                                                                               }, {
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