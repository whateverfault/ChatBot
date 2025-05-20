using ChatBot.Services.game_requests;
using ChatBot.Services.interfaces;
using ChatBot.Services.message_randomizer;

namespace ChatBot.Services;

public static class ServiceManager {
    private static readonly Dictionary<string, Service> _services = new() 
                                                               {
                                                                   {"GameRequests", new GameRequestsService()},
                                                                   {"MessageRandomizer", new MessageRandomizerService()},
                                                               };


    public static void InitServices() {
        foreach (var (_, value) in _services) {
            value.Options.Load();
            value.Init();
        }
    }
    
    public static void KillServices() {
        foreach (var (_, value) in _services) {
            value.Options.Save();
            value.Kill();
        }
    }
    
    public static Service GetService(string key) {
        return _services[key];
    }
}