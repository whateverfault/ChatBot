using ChatBot.Services.chat_commands.Data;
using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.chat_commands;
public delegate void CommandIdentifierChangedHandler(char newId, char oldId);

public class ChatCommandsOptions : Options {
    private SaveData? _saveData;

    private List<DefaultChatCommand>? _defaultCmds = [];
    private string DefaultCmdsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_defaultCmds.json");
    
    private List<CustomChatCommand>? _customCmds = [];
    private string CustomCmdsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_customCmds.json");
    
    
    protected override string Name => "chat_commands";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    public char CommandIdentifier => _saveData!.CommandIdentifier;
    public State VerboseState => _saveData!.VerboseState;
    public List<CustomChatCommand>? CustomCmds => _customCmds;
    public List<DefaultChatCommand>? DefaultCmds => _defaultCmds;
    public string BaseTitle => _saveData!.BaseTitle;
    public State SendWhisperIfPossible => _saveData!.SendWhisperIfPossible;

    public event CommandIdentifierChangedHandler? OnCommandIdentifierChanged;


    public override bool TryLoad() {
        return JsonUtils.TryRead(OptionsPath, out _saveData)
            && JsonUtils.TryRead(DefaultCmdsPath, out _defaultCmds)
            && JsonUtils.TryRead(CustomCmdsPath, out _customCmds);
    }

    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.SaveIssue);
            SetDefaults();
        } if (!JsonUtils.TryRead(DefaultCmdsPath, out _defaultCmds!)) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.SaveIssue);
            SetDefaults();
        } if (!JsonUtils.TryRead(CustomCmdsPath, out _customCmds!)) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.SaveIssue);
            SetDefaults();
        }
    }

    public override void Save() {
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
        JsonUtils.WriteSafe(DefaultCmdsPath, Path.Combine(Directories.ServiceDirectory, Name), _defaultCmds);
        JsonUtils.WriteSafe(CustomCmdsPath, Path.Combine(Directories.ServiceDirectory, Name), _customCmds);
    }

    public override void SetDefaults() {
        _saveData = new SaveData();
        _defaultCmds = [];
        _customCmds = [];
        CommandsList.SetDefaults();
        Save();
    }

    public override State GetState() {
        return ServiceState;
    }

    public override void SetState(State state) {
        _saveData!.ServiceState = state;
        Save();
    }

    public char GetCommandIdentifier() {
        return CommandIdentifier;
    }
    
    public void SetCommandIdentifier(char identifier) {
        OnCommandIdentifierChanged?.Invoke(identifier, CommandIdentifier);
        _saveData!.CommandIdentifier = identifier;
        Save();
    }

    public State GetVerboseState() {
        return VerboseState;
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

    public List<CustomChatCommand>? GetCustomCommands() {
        return CustomCmds;
    }
    
    public void AddChatCmd(ChatCommand chatCmd) {
        if (chatCmd.GetType() != typeof(CustomChatCommand)) return;
        
        var cmd = (CustomChatCommand)chatCmd;
        CustomCmds?.Add(cmd);
        Save();
    }

    public void RemoveChatCmd(int index) {
        if (index < 0 || index >= CustomCmds?.Count) return;
        CustomCmds?.RemoveAt(index);
        Save();
    }

    public void SetDefaultCmds(List<DefaultChatCommand> cmds) {
        _defaultCmds = cmds;
        Save();
    }
}