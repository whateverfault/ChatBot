using ChatBot.Services.chat_commands.Data;
using ChatBot.Services.moderation;
using ChatBot.Services.Static;
using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.chat_commands;
public delegate void CommandIdentifierChangedHandler(char newId, char oldId);

public class ChatCommandsOptions : Options {
    private SaveData? _saveData;
    
    
    protected override string Name => "chat_commands";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    public char CommandIdentifier => _saveData!.CommandIdentifier;
    public State VerboseState => _saveData!.VerboseState;
    public List<CustomChatCommand> CustomCmds => _saveData!.CustomCmds;
    public List<DefaultChatCommand> DefaultCmds => _saveData!.DefaultCmds;
    public string BaseTitle => _saveData!.BaseTitle;

    public event CommandIdentifierChangedHandler? OnCommandIdentifierChanged;


    public override bool TryLoad() {
        return JsonUtils.TryRead(OptionsPath, out _saveData);
    }

    public override void Load() {
        if (!JsonUtils.TryRead(OptionsPath, out _saveData!)) {
            ErrorHandler.LogErrorAndPrint(ErrorCode.SaveIssue);
            SetDefaults();
        }
    }

    public override void Save() {
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
    }

    public override void SetDefaults() {
        _saveData = new SaveData();
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

    public void RemoveChatCmd(int index) {
        if (index < 0 || index >= CustomCmds.Count) return;
        CustomCmds.RemoveAt(index);
        Save();
    }

    public void SetDefaultCmds(List<DefaultChatCommand> cmds) {
        _saveData!.DefaultCmds = cmds;
    }
}