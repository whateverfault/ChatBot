using ChatBot.Services.moderation;
using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.chat_commands;

public class ChatCommandsOptions : Options {
    public delegate void CommandIdentifierChangedHandler(char newId, char oldId);
    private SaveData? _saveData;

    protected override string Name => "chat_commands";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    public override State ServiceState => _saveData!.ServiceState;
    public char CommandIdentifier => _saveData!.CommandIdentifier;
    public Restriction RequiredRole => _saveData!.RequiredRole;
    public State VerboseState => _saveData!.VerboseState;
    public List<CustomChatCommand> CustomCmds => _saveData!.CustomCmds;
    public List<DefaultChatCommand> DefaultCmds => _saveData!.DefaultCmds;
    public ModerationService ModerationService { get; private set; } = null!;

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
    
    public void SetServices(ModerationService moderation) {
        ModerationService = moderation;
    }

    public Restriction GetRequiredRole() {
        return RequiredRole;
    }

    public void SetRequiredRole(Restriction requiredRole) {
        _saveData!.RequiredRole = requiredRole;
        Save();
    }

    public State GetVerboseState() {
        return VerboseState;
    }

    public void SetVerboseState(State state) {
        _saveData!.VerboseState = state;
        Save();
    }

    public List<CustomChatCommand> GetCustomCmds() {
        return CustomCmds;
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