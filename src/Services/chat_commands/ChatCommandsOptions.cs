using ChatBot.services.chat_commands.Data;
using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.services.chat_commands;
public delegate void CommandIdentifierChangedHandler(char newId, char oldId);

public class ChatCommandsOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;

    private List<DefaultChatCommand> _defaultCmds = null!;
    private string DefaultCmdsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_defaultCmds.json");

    private List<CustomChatCommand> _customCmds = null!;
    private string CustomCmdsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_customCmds.json");


    protected override string Name => "chat_commands";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    public char CommandIdentifier => _saveData!.CommandIdentifier;
    public State VerboseState => _saveData!.VerboseState;
    public List<CustomChatCommand> CustomCmds => _customCmds;
    public List<DefaultChatCommand> DefaultCmds => _defaultCmds;
    public string BaseTitle => _saveData!.BaseTitle;
    public State SendWhisperIfPossible => _saveData!.SendWhisperIfPossible;

    public event CommandIdentifierChangedHandler? OnCommandIdentifierChanged;

    
    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        } if (!JsonUtils.TryRead(DefaultCmdsPath, out _defaultCmds!)) {
            SetDefaults();
        } if (!JsonUtils.TryRead(CustomCmdsPath, out _customCmds!)) {
            SetDefaults();
        }
    }

    public override void Save() {
        lock (_fileLock) {
            JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
            JsonUtils.WriteSafe(DefaultCmdsPath, Path.Combine(Directories.ServiceDirectory, Name), _defaultCmds);
            JsonUtils.WriteSafe(CustomCmdsPath, Path.Combine(Directories.ServiceDirectory, Name), _customCmds);
        }
    }

    public override void SetDefaults() {
        _saveData = new SaveData();
        _defaultCmds = [];
        _customCmds = [];
        CommandsList.SetDefaults();
        Save();
    }

    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }

    public char GetCommandIdentifier() {
        return CommandIdentifier;
    }
    
    public List<CustomChatCommand> GetCustomCommands() {
        return CustomCmds;
    }
    
    public CustomChatCommand? GetCustomCmdById(int id) {
        return CustomCmds.FirstOrDefault(chatCmd => chatCmd.Id == id);
    }
    
    public void SetCommandIdentifier(char identifier) {
        OnCommandIdentifierChanged?.Invoke(identifier, CommandIdentifier);
        _saveData!.CommandIdentifier = identifier;
        Save();
    }

    public void SetVerboseState(State state) {
        _saveData!.VerboseState = state;
        Save();
    }

    public void SetSendWhisperIfPossibleState(State state) {
        _saveData!.SendWhisperIfPossible = state;
        Save();
    }
    
    public void SetBaseTitle(string title) {
        _saveData!.BaseTitle = title;
        Save();
    }
    
    public void AddChatCmd(ChatCommand chatCmd) {
        if (chatCmd.GetType() != typeof(CustomChatCommand)) return;
        
        var cmd = (CustomChatCommand)chatCmd;
        CustomCmds.Add(cmd);
        Save();
    }

    public bool RemoveChatCmd(int index) {
        if (index < 0 || index >= CustomCmds.Count) return false;
        
        CustomCmds.RemoveAt(index);
        Save();
        return true;
    }

    public bool TryRemoveChatCmdById(int id) {
        for (var i = 0; i < CustomCmds.Count; i++) {
            if (CustomCmds[i].Id != id) {
                continue;
            }

            CustomCmds.RemoveAt(i);
            Save();
            return true;
        }

        return false;
    }
    
    public void SetDefaultCmds(List<DefaultChatCommand> cmds) {
        JsonUtils.CreateOld(DefaultCmdsPath);
        ErrorHandler.LogErrorMessageAndPrint(ErrorCode.SaveFileCorrupted, $"List of default commands is restored to the defaults.\nOld save file can be found at: {DefaultCmdsPath}.old.\n \nPress Enter To Continue...");
        
        _defaultCmds = cmds;
        Save();
    }
}