using ChatBot.bot;
using ChatBot.services.chat_commands.Parser;
using ChatBot.services.interfaces;
using ChatBot.services.logger;
using ChatBot.services.Static;
using TwitchLib.Client.Events;

namespace ChatBot.services.chat_commands;

public class ChatCommandsEvents : ServiceEvents {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    private ChatCommandsService _chatCommands = null!;

    public override bool Initialized { get; protected set; }
    

    public override void Init(Service service) {
        _chatCommands = (ChatCommandsService)service;
        base.Init(service);
    }

    protected override void Subscribe() {
        if (Subscribed) {
            return;
        }
        base.Subscribe();
        
        TwitchChatBot.Instance.OnMessageReceived += ParseCommandAndHandle;
        _chatCommands.Options.OnCommandIdentifierChanged += _chatCommands.ChangeCommandIdentifier;
    }
    
    protected override void UnSubscribe() {
        if (!Subscribed) {
            return;
        }
        base.UnSubscribe();
        
        TwitchChatBot.Instance.OnMessageReceived -= ParseCommandAndHandle;
        _chatCommands.Options.OnCommandIdentifierChanged -= _chatCommands.ChangeCommandIdentifier;
    }

    private void ParseCommandAndHandle(object? sender, OnMessageReceivedArgs args) {
        try {
            var command = ChatCommandParser.Parse(args.ChatMessage);

            if (command == null) {
                _logger.Log(LogLevel.Error, "Chat Command Parser returned null.");
                return;
            }
            
            Task.Run(() => _chatCommands.HandleCommand(command))
                .ContinueWith(t => {
                                  if (t.IsFaulted) {
                                      _logger.Log(LogLevel.Error, $"Command failed: {t.Exception}");
                                  }
                              });
        }
        catch (Exception e) {
            _logger.Log(LogLevel.Error, $"Exception while parsing a command. {e}");
        }
    }
}