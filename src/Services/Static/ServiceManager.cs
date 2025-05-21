using ChatBot.Services.chat_commands;
using ChatBot.Services.game_requests;
using ChatBot.Services.interfaces;
using ChatBot.Services.message_randomizer;
using ChatBot.twitchAPI.interfaces;

namespace ChatBot.Services.Static;

public static class ServiceManager {
    private static readonly Dictionary<string, (Service, ServiceEvents)> _services = new() 
                                                               {
                                                                   {ServiceName.GameRequests, (new GameRequestsService(), new GameRequestsEvents())},
                                                                   {ServiceName.MessageRandomizer, (new MessageRandomizerService(), new MessageRandomizerEvents())},
                                                                   {ServiceName.ChatCommands, (new ChatCommandsService(), new ChatCommandsEvents())},
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