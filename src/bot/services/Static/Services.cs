using ChatBot.bot.services.ai;
using ChatBot.bot.services.bank;
using ChatBot.bot.services.casino;
using ChatBot.bot.services.chat_ads;
using ChatBot.bot.services.chat_commands;
using ChatBot.bot.services.chat_logs;
using ChatBot.bot.services.demon_list;
using ChatBot.bot.services.game_requests;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.interpreter;
using ChatBot.bot.services.level_requests;
using ChatBot.bot.services.logger;
using ChatBot.bot.services.message_filter;
using ChatBot.bot.services.message_randomizer;
using ChatBot.bot.services.moderation;
using ChatBot.bot.services.presets;
using ChatBot.bot.services.shop;
using ChatBot.bot.services.stream_state_checker;
using ChatBot.bot.services.telegram_notifications;
using ChatBot.bot.services.text_generator;
using ChatBot.bot.services.translator;

namespace ChatBot.bot.services.Static;

public enum ServiceId {
    Presets,
    Logger,
    MessageFilter,
    MessageRandomizer,
    ChatCommands,
    Moderation,
    TextGenerator,
    ChatLogs,
    LevelRequests,
    Ai,
    DemonList, 
    Translator, 
    GameRequests, 
    StreamStateChecker,
    TgNotifications,
    ChatAds,
    Casino,
    Bank,
    Shop,
    Interpreter, 
}

public static class Services {
    private static readonly Dictionary<ServiceId, (Service, ServiceEvents)> _services;


    static Services() {
        try {
            _services = new Dictionary<ServiceId, (Service, ServiceEvents)> {
                                                                                {
                                                                                    ServiceId.Presets,
                                                                                    (
                                                                                        new PresetsService(),
                                                                                        new PresetsEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.Logger,
                                                                                    (
                                                                                        new LoggerService(),
                                                                                        new LoggerEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.MessageFilter,
                                                                                    (
                                                                                        new MessageFilterService(),
                                                                                        new MessageFilterEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.MessageRandomizer,
                                                                                    (
                                                                                        new MessageRandomizerService(),
                                                                                        new MessageRandomizerEvents()
                                                                                    ) 
                                                                                }, 
                                                                                {
                                                                                    ServiceId.ChatCommands,
                                                                                    (
                                                                                        new ChatCommandsService(),
                                                                                        new ChatCommandsEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.Moderation,
                                                                                    (
                                                                                        new ModerationService(),
                                                                                        new ModerationEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.TextGenerator,
                                                                                    (
                                                                                        new TextGeneratorService(),
                                                                                        new TextGeneratorEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.ChatLogs,
                                                                                    (
                                                                                        new ChatLogsService(),
                                                                                        new ChatLogsEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.LevelRequests,
                                                                                    (
                                                                                        new LevelRequestsService(),
                                                                                        new LevelRequestsEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.Ai,
                                                                                    (
                                                                                        new AiService(),
                                                                                        new AiEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.DemonList,
                                                                                    (
                                                                                        new DemonListService(),
                                                                                        new DemonListEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.Translator,
                                                                                    (
                                                                                        new TranslatorService(),
                                                                                        new TranslatorEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.GameRequests,
                                                                                    (
                                                                                        new GameRequestsService(),
                                                                                        new GameRequestsEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.StreamStateChecker,
                                                                                    (
                                                                                        new StreamStateCheckerService(),
                                                                                        new StreamStateCheckerEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.TgNotifications,
                                                                                    (
                                                                                        new TgNotificationsService(),
                                                                                        new TgNotificationsEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.ChatAds,
                                                                                    (
                                                                                        new ChatAdsService(),
                                                                                        new ChatAdsEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.Casino,
                                                                                    (
                                                                                        new CasinoService(),
                                                                                        new CasinoEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.Bank,
                                                                                    (
                                                                                        new BankService(),
                                                                                        new BankEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.Shop,
                                                                                    (
                                                                                        new ShopService(),
                                                                                        new ShopEvents()
                                                                                    )
                                                                                },
                                                                                {
                                                                                    ServiceId.Interpreter,
                                                                                    (
                                                                                        new InterpreterService(),
                                                                                        new InterpreterEvents()
                                                                                    )
                                                                                },
                                                                            };
        } catch (Exception e) {
            throw new Exception($"Failed to initialize services: {e.Data}");
        }
    }
    
    public static void Load(ServiceId[] exclude) {
        foreach (var (id, (service, serviceEvents)) in _services) {
            if (exclude.Contains(id)) continue;
            if (serviceEvents.Initialized) {
                serviceEvents.Kill();
            }
            
            service.Init();
        }
    }
    
    public static void Load() {
        Load([]);
    }
    
    public static void Init(ServiceId[] exclude) {
        foreach (var (id, (service, serviceEvents)) in _services) {
            if (exclude.Contains(id)) continue;
            if (serviceEvents.Initialized) {
                serviceEvents.Kill();
            }
            
            serviceEvents.Init(service);
        }
    }
    
    public static void Init() {
        Init([]);
    }

    public static void Kill(ServiceId[] exclude) {
        foreach (var (id, (_, serviceEvents)) in _services) {
            if (exclude.Contains(id)) continue;
            if (serviceEvents.Initialized) {
                serviceEvents.Kill();
            }
        }
    }
    
    public static void Kill() {
        Kill([]);
    }
    
    public static void ServicesToDefault(ServiceId[] exclude) {
        foreach (var (id, (service, _)) in _services) {
            if (exclude.Contains(id)) continue;
            service.Options.SetDefaults();
        }
    }
    
    public static Service Get(ServiceId id) {
        return _services[id].Item1;
    }
}