using ChatBot.bot;
using ChatBot.services.chat_commands.Data;
using ChatBot.services.chat_commands.Parser.Data;
using ChatBot.services.interfaces;
using ChatBot.services.Static;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using TwitchLib.Client.Interfaces;

namespace ChatBot.services.chat_commands;

public class ChatCommandsService : Service {
    private static TwitchChatBot Bot => TwitchChatBot.Instance;
    private static ITwitchClient? Client => Bot.GetClient();
    
    private long _time;
    
    public override string Name => ServiceName.ChatCommands;
    public override ChatCommandsOptions Options { get; } = new ChatCommandsOptions();


    public async void HandleCommand(ParsedChatCommand parsedCommand) {
        try {
            var cmdName = parsedCommand.CommandText;
            var chatMessage = parsedCommand.ChatMessage;
            var chatArgs = new ChatCmdArgs(parsedCommand);
            
            var defaultCmds = new List<ChatCommand>(Options.DefaultCmds);
            var found = await TryActivateCommand(cmdName, defaultCmds, chatArgs);
            if (found) return;
            
            var customCmds = new List<ChatCommand>(Options.CustomCmds);
            found = await TryActivateCommand(cmdName, customCmds, chatArgs);
            if (found) return;

            if (Options.VerboseState == State.Enabled) {
                Client?.SendReply(chatMessage.Channel, chatMessage.Id, $"Неизвестная комманда: {Options.CommandIdentifier}{parsedCommand.CommandText}");
            }
        } catch (Exception) {
            if (Options.VerboseState == State.Enabled) {
                Client?.SendReply(parsedCommand.ChatMessage.Channel, parsedCommand.ChatMessage.Id, $"Ошибка при обработке команды {Options.CommandIdentifier}{parsedCommand.CommandText}.");
            }
        }
    }
    
    private async Task<bool> TryActivateCommand(string cmdName, List<ChatCommand> cmds, ChatCmdArgs args) {
        var chatMessage = args.Parsed.ChatMessage;

        foreach (var cmd in cmds) {
            if (!RestrictionHandler.Handle(cmd.Restriction, chatMessage)) continue;
            
            if (!string.Equals(cmdName, cmd.Name, StringComparison.InvariantCultureIgnoreCase) 
             && (cmd.Aliases == null || !cmd.Aliases.Contains(cmdName))) continue;
        
            if (cmds
               .Any(defaultCmd =>
                        string.Equals(defaultCmd.Name, cmd.Name, StringComparison.InvariantCultureIgnoreCase)
                     && defaultCmd.Restriction < cmd.Restriction
                     && RestrictionHandler.Handle(defaultCmd.Restriction, chatMessage))
               ) continue;

            if (cmd.State == State.Disabled) return false;
            
            var curTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (curTime-_time < cmd.Cooldown) continue;
            
            args.UpdateCommand(cmd);
            
            if ((args.Command.HasIdentifier && args.Parsed.CommandIdentifier == ' ')
             || (!args.Command.HasIdentifier && args.Parsed.CommandIdentifier != ' ')) {
                return false;
            }
            
            await cmd.Action.Invoke(args);
            _time = curTime;
            return true;
        }

        return false;
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
        var err = Bot.TryGetClient(out _);
        if (ErrorHandler.LogErrorAndPrint(err)) {
            return;
        }
        
        Options.SetCommandIdentifier(identifier);
    }
    
    public void ChangeCommandIdentifier(char newId, char oldId) {
        Client?.RemoveChatCommandIdentifier(oldId);
        Client?.AddChatCommandIdentifier(newId);
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
}