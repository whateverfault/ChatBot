using ChatBot.api.twitch.client;
using ChatBot.api.twitch.client.commands.data;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_commands.Data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;

namespace ChatBot.bot.services.chat_commands;

public class ChatCommandsService : Service {
    private static TwitchChatBot Bot => TwitchChatBot.Instance;
    private static ITwitchClient? Client => Bot.GetClient();
    
    private long _time;
    
    public override string Name => ServiceName.ChatCommands;
    public override ChatCommandsOptions Options { get; } = new ChatCommandsOptions();

    public event EventHandler<CustomChatCommand>? OnChatCommandAdded;
    public event EventHandler<int>? OnChatCommandRemoved;
    

    public async void HandleCommand(object? sender, Command parsedCommand) {
        try {
            if (Options.ServiceState == State.Disabled) return;

            bool found;
            var cmdName = parsedCommand.CommandText;
            var chatMessage = parsedCommand.ChatMessage;
            var chatArgs = new ChatCmdArgs(parsedCommand);

            if (parsedCommand.CommandIdentifier != ' ') {
                var defaultCmds = new List<ChatCommand>(Options.DefaultCmds);
                found = await TryActivateCommand(cmdName, defaultCmds, chatArgs);
                if (found) return;
            }
            
            var customCmds = new List<ChatCommand>(Options.CustomCmds);
            found = await TryActivateCommand(cmdName, customCmds, chatArgs);
            if (found) return;

            if (Options.VerboseState == State.Enabled) {
                Client?.SendReply(chatMessage.Id, $"Неизвестная комманда: {Options.CommandIdentifier}{parsedCommand.CommandText}");
            }
        } catch (Exception) {
            if (Options.VerboseState == State.Enabled) {
                Client?.SendReply(parsedCommand.ChatMessage.Id, $"Ошибка при обработке команды {Options.CommandIdentifier}{parsedCommand.CommandText}.");
            }
        }
    }
    
    private Task<bool> TryActivateCommand(string cmdName, List<ChatCommand> cmds, ChatCmdArgs args) {
        var chatMessage = args.Parsed.ChatMessage;

        foreach (var cmd in cmds) {
            if (!chatMessage.Fits(cmd.Restriction)) continue;
            
            if (!string.Equals(cmdName, cmd.Name, StringComparison.InvariantCultureIgnoreCase) 
             && (cmd.Aliases == null || !cmd.Aliases.Contains(cmdName))) continue;
        
            if (cmds
               .Any(defaultCmd =>
                        string.Equals(defaultCmd.Name, cmd.Name, StringComparison.InvariantCultureIgnoreCase)
                     && defaultCmd.Restriction < cmd.Restriction
                     && chatMessage.Fits(defaultCmd.Restriction))
               ) continue;

            if (cmd.State == State.Disabled) return Task.FromResult(false);
            
            var curTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (curTime-_time < cmd.Cooldown) continue;
            
            args.UpdateCommand(cmd);
            
            if ((args.Command.HasIdentifier && args.Parsed.CommandIdentifier == ' ')
             || (!args.Command.HasIdentifier && args.Parsed.CommandIdentifier != ' ')) {
                return Task.FromResult(false);
            }
            
            _ = cmd.Action.Invoke(args);
            _time = curTime;
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
    
    public int GetVerboseStateAsInt() {
        return (int)Options.VerboseState;
    }

    public void VerboseStateNext() {
        Options.SetVerboseState((State)(((int)Options.VerboseState+1)%Enum.GetValues(typeof(State)).Length));
    }

    public int GetSendWhisperIfPossibleStateAsInt() {
        return (int)Options.SendWhisperIfPossible;
    }

    public void SendWhisperIfPossibleStateNext() {
        Options.SetSendWhisperIfPossibleState((State)(((int)Options.SendWhisperIfPossible+1)%Enum.GetValues(typeof(State)).Length));
    }
    
    public string GetBaseTitle() {
        return Options.BaseTitle;
    }

    public void SetBaseTitle(string title) {
        Options.SetBaseTitle(title);
    }
    
    public void SetCommandIdentifier(char identifier) {
        if (Client == null) return;

        if (!Client.SetCommandIdentifier(identifier)) {
            ErrorHandler.LogErrorMessageAndPrint(ErrorCode.InvalidInput, $"Cannot set '{identifier}' as a command identifier.");
            return;
        }
        Options.SetCommandIdentifier(identifier);
    }

    public int GetAvailableCustomCmdId() {
        var cmds = Options.GetCustomCommands();
        if (cmds.Count == 0) {
            return 0;
        }

        var maxId = cmds.Select(cmd => cmd.Id).Max();
        return ++maxId;
    }
    
    public override void Init() {
        base.Init();
        
        if (CommandsList.DefaultsCommands.Count != Options.DefaultCmds.Count) {
            CommandsList.SetDefaults();
        }
    }

    public void AddChatCmd(CustomChatCommand chatCmd) {
        Options.AddChatCmd(chatCmd);
        OnChatCommandAdded?.Invoke(this, chatCmd);
    }
    
    public bool RemoveChatCmd(int index) {
        var result = Options.RemoveChatCmd(index);
        if (result) OnChatCommandRemoved?.Invoke(this, index);
        return result;
    }

    public List<CustomChatCommand> GetCustomChatCommands() {
        return Options.GetCustomCommands();
    }
}