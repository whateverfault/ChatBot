using ChatBot.bot.services.ai;
using ChatBot.bot.services.bank;
using ChatBot.bot.services.casino;
using ChatBot.bot.services.chat_ads;
using ChatBot.bot.services.chat_commands;
using ChatBot.bot.services.chat_logs;
using ChatBot.bot.services.demon_list;
using ChatBot.bot.services.game_requests;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.level_requests;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.message_filter;
using ChatBot.bot.services.message_randomizer;
using ChatBot.bot.services.moderation;
using ChatBot.bot.services.presets;
using ChatBot.bot.services.shop;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.telegram;
using ChatBot.bot.services.text_generator;
using ChatBot.bot.services.translator;

namespace ChatBot.bot.services.Static;

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
                                                                             {
                                                                                 ServiceName.Casino,
                                                                                 (
                                                                                     new CasinoService(),
                                                                                     new CasinoEvents()
                                                                                 )
                                                                             },
                                                                             {
                                                                                 ServiceName.Bank,
                                                                                 (
                                                                                     new BankService(),
                                                                                     new BankEvents()
                                                                                 )
                                                                             },
                                                                             {
                                                                                 ServiceName.Shop,
                                                                                 (
                                                                                     new ShopService(),
                                                                                     new ShopEvents()
                                                                                 )
                                                                             },
                                                                         };
        } catch (Exception e) {
            Console.WriteLine($"Failed to initialize services: {e}");
            throw new Exception($"Failed to initialize services: {e}");
        }
    }
    
    public static void LoadServices(string[] exclude) {
        foreach (var (_, (service, serviceEvents)) in _services) {
            if (exclude.Contains(service.Name)) continue;
            if (serviceEvents.Initialized) {
                serviceEvents.Kill();
            }
            
            service.Init();
        }
    }
    
    public static void LoadServices() {
        LoadServices([]);
    }
    
    public static void InitServices(string[] exclude) {
        foreach (var (_, (service, serviceEvents)) in _services) {
            if (exclude.Contains(service.Name)) continue;
            if (serviceEvents.Initialized) {
                serviceEvents.Kill();
            }
            
            serviceEvents.Init(service);
        }
    }
    
    public static void InitServices() {
        InitServices([]);
    }

    public static void KillServices(string[] exclude) {
        foreach (var (_, (service, serviceEvents)) in _services) {
            if (exclude.Contains(service.Name)) continue;
            if (serviceEvents.Initialized) {
                serviceEvents.Kill();
            }
        }
    }
    
    public static void KillServices() {
        KillServices([]);
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