using ChatBot.bot.interfaces;
using ChatBot.Services.interfaces;
using ChatBot.Services.logger;
using ChatBot.Services.moderation;
using ChatBot.Services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace ChatBot.Services.chat_commands;

public class ChatCommandsService : Service {
    private static readonly LoggerService _logger = (LoggerService)ServiceManager.GetService(ServiceName.Logger);
    
    private bot.ChatBot _bot = null!;
    private long _time;
    private ITwitchClient? Client => _bot.GetClient();

    public override string Name => ServiceName.ChatCommands;
    public override ChatCommandsOptions Options { get; } = new();


    public async void HandleCommand(object? sender, OnChatCommandReceivedArgs args) {
        try {
            var cmdName = args.Command.CommandText;
            var parsed = ProcessArgs(args.Command.ArgumentsAsList);
            var chatMessage = args.Command.ChatMessage;
        
            foreach (var cmd in Options.DefaultCmds) {
                if (!RestrictionHandler.Handle(cmd.Restriction, chatMessage)) continue;
                if (cmdName != cmd.Name && (cmd.Aliases == null || !cmd.Aliases.Contains(cmdName))) continue;
                if (cmd.State == State.Disabled) continue;
                
                var curTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (curTime-_time < cmd.Cooldown) continue;
                
                var cmdArgs = new ChatCmdArgs(args, parsed, _bot, cmd);
                await cmd.Action?.Invoke(cmdArgs)!;
                _time = curTime;
                return;
            }
            
            foreach (var cmd in Options.CustomCmds) {
                if (!RestrictionHandler.Handle(cmd.Restriction, chatMessage)) continue;
                if (cmdName != cmd.Name && (cmd.Aliases == null || !cmd.Aliases.Contains(cmdName))) continue;
                if (cmd.State == State.Disabled) continue;
                
                var curTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (curTime-_time < cmd.Cooldown) continue;
                
                var cmdArgs = new ChatCmdArgs(args, parsed, _bot, cmd);
                await cmd.Action?.Invoke(cmdArgs)!;
                _time = curTime;
                return;
            }
            
            if (Options.VerboseState == State.Enabled) {
                Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Неизвестная комманда: {Options.CommandIdentifier}{args.Command.CommandText}");
            }
        } catch (Exception e) {
            if (Options.VerboseState == State.Enabled) {
                Client?.SendReply(args.Command.ChatMessage.Channel, args.Command.ChatMessage.Id, $"Ошибка при обработке команды {Options.CommandIdentifier}{args.Command.CommandText}.");
            }
            _logger.Log(LogLevel.Error, $"Error while handling a command: {e.Message}");
        }
    }
    
    private List<string> ProcessArgs(List<string?> args) {
        var processedArgs = new List<string>();
        foreach (var arg in args) {
            if (string.IsNullOrEmpty(arg)) continue;
            if (arg.Length is 1 or 2 && !char.IsLetterOrDigit(arg[0])) continue;
            processedArgs.Add(arg);
        }
        return processedArgs;
    }

    public int GetVerboseStateAsInt() {
        return (int)Options.VerboseState;
    }

    public void VerboseStateNext() {
        Options.SetVerboseState((State)(((int)Options.VerboseState+1)%Enum.GetValues(typeof(State)).Length));
    }
    
    public void SetCommandIdentifier(char identifier) {
        var err = _bot.TryGetClient(out _);
        if (ErrorHandler.LogErrorAndPrint(err)) {
            return;
        }
        
        Options.SetCommandIdentifier(identifier);
    }
    
    public void ChangeCommandIdentifier(char newId, char oldId) {
        Client?.RemoveChatCommandIdentifier(oldId);
        Client?.AddChatCommandIdentifier(newId);
    }
    
    public override void Init(Bot bot) {
        _bot = (bot.ChatBot)bot;

        if (!Options.TryLoad()) {
            Options.SetDefaults();
        }

        Options.SetServices((ModerationService)ServiceManager.GetService(ServiceName.Moderation));
    }

}