using ChatBot.Services.game_requests;
using ChatBot.Services.interfaces;

namespace ChatBot.Services;

public static class ServiceManager {
    private static readonly Dictionary<string, Service> _services = new() 
                                                               {
                                                                   {"GameRequestsService", new GameRequestsService()},
                                                               };


    public static void InitServices() {
        foreach (var (_, value) in _services) {
            value.Init();
        }
    }
    
    public static void KillServices() {
        foreach (var (_, value) in _services) {
            value.Kill();
        }
    }
    
    public static Service GetService(string key) {
        return _services[key];
    }
}