using ChatBot.bot.chat_bot;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_commands.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;
using TwitchAPI.client.commands.data;
using TwitchAPI.client.data;

namespace ChatBot.bot.services.chat_commands;

public class ChatCommandsService : Service {
    private static TwitchChatBot Bot => TwitchChatBot.Instance;
    private static ITwitchClient? Client => Bot.GetClient();
    
    public override ChatCommandsOptions Options { get; } = new ChatCommandsOptions();

    public event EventHandler<CustomChatCommand>? OnChatCommandAdded;
    public event EventHandler<int>? OnChatCommandRemoved;
    

    public async void HandleCommand(object? sender, Command parsedCommand) {
        try {
            if (Options.ServiceState == State.Disabled) return;
            if (Client == null) return;

            bool found;
            var cmdName = parsedCommand.CommandText;
            var chatMessage = parsedCommand.ChatMessage;
            var chatArgs = new ChatCmdArgs(parsedCommand);

            if (!char.IsWhiteSpace(parsedCommand.CommandIdentifier)) {
                var defaultCmds = new List<ChatCommand>(Options.DefaultCmds);
                found = await TryActivateCommand(cmdName, defaultCmds, chatArgs);
                if (found) return;
            }
            
            var customCmds = new List<ChatCommand>(Options.CustomCmds);
            found = await TryActivateCommand(cmdName, customCmds, chatArgs);
            if (found) return;

            if (Options.VerboseState == State.Enabled) {
                await Client.SendMessage($"Unknown command: {Options.CommandIdentifier}{parsedCommand.CommandText}", chatMessage.Id);
            }
        } catch (Exception e) {
            var msg = $"Failed to handle a command: {Options.CommandIdentifier}{parsedCommand.CommandText}.";
            ErrorHandler.LogMessage(LogLevel.Error, $"{msg} {e.Message}");
            
            if (Client == null) return;
            await Client.SendMessage(msg);
        }
    }
    
    private async Task<bool> TryActivateCommand(string cmdName, List<ChatCommand> cmds, ChatCmdArgs args) {
        var chatMessage = args.Parsed.ChatMessage;

        foreach (var cmd in cmds) {
            if (!chatMessage.Fits(cmd.Restriction)) continue;

            if (!string.Equals(cmdName, cmd.Name, StringComparison.InvariantCultureIgnoreCase)
             && !cmd.Aliases.Contains(cmdName)) continue;

            if (cmds
               .Any(command =>
                        string.Equals(command.Name, cmd.Name, StringComparison.InvariantCultureIgnoreCase)
                     && command.Restriction < cmd.Restriction
                     && chatMessage.Fits(command.Restriction))
               ) continue;

            if (cmd.State == State.Disabled) return false;
            args.UpdateCommand(cmd);

            var onCooldown = await OnCooldown(cmd, chatMessage);
            if (onCooldown) continue;
            
            if ((args.Command.HasIdentifier && args.Parsed.CommandIdentifier == ' ')
             || (!args.Command.HasIdentifier && args.Parsed.CommandIdentifier != ' ')) {
                return false;
            }

            await cmd.Action.Invoke(args);
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

    public bool GetSendWhisperIfPossible() {
        return Options.SendWhisperIfPossible;
    }

    public void SetSendWhisperIfPossibleState(bool value) {
        Options.SetSendWhisperIfPossibleState(value);
    }
    
    public bool GetUse7Tv() {
        return Options.Use7Tv;
    }

    public void SetUse7Tv(bool value) {
        Options.SetUse7Tv(value);
    }
    
    public string GetBaseTitle() {
        return Options.BaseTitle;
    }

    public void SetBaseTitle(string title) {
        Options.SetBaseTitle(title);
    }
    
    public void SetCommandIdentifier(char identifier) {
        if (Client == null) {
            if (!char.IsPunctuation(identifier)) return;
            Options.SetCommandIdentifier(identifier);
            return;
        }

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

    private async Task SendCooldownLeft(long usedAt, long cooldown, string replyId) {
        if (Client == null) return;
        
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var elapsed = now - usedAt;
        
        if (elapsed >= cooldown) return;
        
        await Client.SendMessage($"{cooldown - elapsed}...", replyId);
    }

    private async Task<bool> OnCooldown(ChatCommand cmd, ChatMessage chatMessage) {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        var lastUsed = cmd.CooldownPerUser ? -1 : cmd.LastUsed;
        if (cmd.CooldownPerUser) {
            var index = cmd.CooldownUsers.FindIndex(x => x.UserId.Equals(chatMessage.UserId));
            if (index >= 0) {
                var user = cmd.CooldownUsers[index];
                lastUsed = user.UsedAt;
                
                cmd.CooldownUsers.RemoveAt(index);
            }
        }
        
        if (now - lastUsed < cmd.Cooldown) {
            await SendCooldownLeft(lastUsed, cmd.Cooldown, chatMessage.Id);
            return true;
        }

        if (cmd.CooldownPerUser) cmd.AddCooldownUser(chatMessage.UserId);
        cmd.SetLastUsed(now);
        return false;
    }
}