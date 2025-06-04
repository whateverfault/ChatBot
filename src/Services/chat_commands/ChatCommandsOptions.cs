using ChatBot.Services.message_randomizer;
using ChatBot.shared;
using ChatBot.shared.Handlers;
using ChatBot.shared.interfaces;
using ChatBot.utils;

namespace ChatBot.Services.chat_commands;

public class ChatCommandsOptions : Options {

    public delegate void CommandIdentifierChangedHandler(char newId, char oldId);
    private SaveData? _saveData;

    protected override string Name => "chat_commands";
    protected override string OptionsPath => Path.Combine(Directories.serviceDirectory+Name, $"{Name}_opt.json");
    public override State ServiceState => _saveData!.ServiceState;
    public char CommandIdentifier => _saveData!.CommandIdentifier;
    public Restriction RequiredRole => _saveData!.RequiredRole;

    public MessageRandomizerService MessageRandomizerService { get; private set; } = null!;

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
        JsonUtils.WriteSafe(OptionsPath, Path.Combine(Directories.serviceDirectory, Name), _saveData);
    }

    public override void SetDefaults() {
        _saveData = new SaveData(
                                 State.Disabled,
                                 '~',
                                 Restriction.Everyone
                                );
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
    
    public void SetServices(MessageRandomizerService mr) {
        MessageRandomizerService = mr;
    }

    public Restriction GetRequiredRole() {
        return RequiredRole;
    }

    public void SetRequiredRole(Restriction requiredRole) {
        _saveData!.RequiredRole = requiredRole;
    }
}