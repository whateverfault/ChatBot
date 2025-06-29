using ChatBot.bot.interfaces;
using ChatBot.Services.ai;
using ChatBot.Services.chat_commands;
using ChatBot.Services.chat_logs;
using ChatBot.Services.demon_list;
using ChatBot.Services.interfaces;
using ChatBot.Services.level_requests;
using ChatBot.Services.logger;
using ChatBot.Services.message_filter;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.moderation;
using ChatBot.Services.presets;
using ChatBot.Services.text_generator;
using ChatBot.Services.translator;

namespace ChatBot.Services.Static;

public static class ServiceManager {
    private static readonly Dictionary<string, (Service, ServiceEvents)> _services;


    static ServiceManager() {
        try {
            _services = new() {
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
                              };
        } catch (Exception e) {
            Console.WriteLine($"Failed to initialize services: {e}");
            throw new Exception($"Failed to initialize services: {e}");
        }
    }
    
    public static void InitServices(Bot bot, string[] exclude) {
        foreach (var (_, (service, events)) in _services) {
            if (exclude.Contains(service.Name)) continue;
            service.Init(bot);

            events.Init(service, bot);
            events.Subscribe();
        }
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