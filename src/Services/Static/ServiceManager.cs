using ChatBot.services.ai;
using ChatBot.services.chat_ads;
using ChatBot.services.chat_commands;
using ChatBot.services.chat_logs;
using ChatBot.services.demon_list;
using ChatBot.services.game_requests;
using ChatBot.services.interfaces;
using ChatBot.services.level_requests;
using ChatBot.services.logger;
using ChatBot.services.message_filter;
using ChatBot.services.message_randomizer;
using ChatBot.services.moderation;
using ChatBot.services.presets;
using ChatBot.services.stream_state_checker;
using ChatBot.services.telegram;
using ChatBot.services.text_generator;
using ChatBot.services.translator;

namespace ChatBot.services.Static;

public static class ServiceManager {
    private static readonly Dictionary<string, (Service, ServiceEvents)> _services;


    static ServiceManager() {
        try {
            _services = new Dictionary<string, (Service, ServiceEvents)> {
                                                                             {
                                                                                 ServiceName.Presets,
                                                                                 (
                                                                                     new PresetsService(),
                                                                                     new PresetsEvents()
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
                                                                                 ServiceName.MessageFilter,
                                                                                 (
                                                                                     new MessageFilterService(),
                                                                                     new MessageFilterEvents()
                                                                                 )
                                                                             },
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
                                                                                 ServiceName.Moderation,
                                                                                 (
                                                                                     new ModerationService(),
                                                                                     new ModerationEvents()
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
                                                                             {
                                                                                 ServiceName.Ai,
                                                                                 (
                                                                                     new AiService(),
                                                                                     new AiEvents()
                                                                                 )
                                                                             },
                                                                             {
                                                                                 ServiceName.DemonList,
                                                                                 (
                                                                                     new DemonListService(),
                                                                                     new DemonListEvents()
                                                                                 )
                                                                             },
                                                                             {
                                                                                 ServiceName.Translator,
                                                                                 (
                                                                                     new TranslatorService(),
                                                                                     new TranslatorEvents()
                                                                                 )
                                                                             },
                                                                             {
                                                                                 ServiceName.GameRequests,
                                                                                 (
                                                                                     new GameRequestsService(),
                                                                                     new GameRequestsEvents()
                                                                                 )
                                                                             },
                                                                             {
                                                                                 ServiceName.StreamStateChecker,
                                                                                 (
                                                                                     new StreamStateCheckerService(),
                                                                                     new StreamStateCheckerEvents()
                                                                                 )
                                                                             },
                                                                             {
                                                                                 ServiceName.TgNotifications,
                                                                                 (
                                                                                     new TgNotificationsService(),
                                                                                     new TgNotificationsEvents()
                                                                                 )
                                                                             },
                                                                             {
                                                                                 ServiceName.ChatAds,
                                                                                 (
                                                                                     new ChatAdsService(),
                                                                                     new ChatAdsEvents()
                                                                                 )
                                                                             },
                                                                         };
        } catch (Exception e) {
            Console.WriteLine($"Failed to initialize services: {e.Message}");
            throw new Exception($"Failed to initialize services: {e}");
        }
    }
    
    public static void InitServices(string[] exclude) {
        foreach (var (_, (service, serviceEvents)) in _services) {
            if (exclude.Contains(service.Name)) continue;
            if (serviceEvents.Initialized) {
                serviceEvents.Kill();
            }
            
            service.Init();
            serviceEvents.Init(service);
        }
    }
    
    public static void InitServices() {
        InitServices([]);
    }
    
    public static void ServicesToDefault(string[] exclude) {
        foreach (var (_, (service, _)) in _services) {
            if (exclude.Contains(service.Name)) continue;
            service.Options.SetDefaults();
        }
    }
    
    public static Service GetService(string key) {
        return _services[key].Item1;
    }
}