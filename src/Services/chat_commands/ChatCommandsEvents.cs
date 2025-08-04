using ChatBot.bot;
using ChatBot.services.chat_commands.Parser;
using ChatBot.services.interfaces;
using TwitchLib.Client.Events;

namespace ChatBot.services.chat_commands;

public class ChatCommandsEvents : ServiceEvents {
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
        var command = ChatCommandParser.Parse(args.ChatMessage);

        if (command == null) {
            return;
        }

        _chatCommands.HandleCommand(command);
    }
}