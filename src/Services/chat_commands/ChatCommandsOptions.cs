using ChatBot.Services.demon_list;
using ChatBot.Services.level_requests;
using ChatBot.Services.message_randomizer;
using ChatBot.Services.moderation;
using ChatBot.Services.text_generator;
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
    public int ModActionIndex => _saveData!.ModActionIndex;
    public int Cooldown => _saveData!.Cooldown;
    public State VerboseState => _saveData!.VerboseState;
    public MessageRandomizerService MessageRandomizerService { get; private set; } = null!;
    public ModerationService ModerationService { get; private set; } = null!;
    public TextGeneratorService TextGeneratorService { get; private set; } = null!;
    public LevelRequestsService LevelRequestsService { get; private set; } = null!;
    public DemonListService DemonListService { get; private set; } = null!;

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
    
    public void SetServices(
        MessageRandomizerService messageRandomizer,
        ModerationService moderation,
        TextGeneratorService textGenerator,
        LevelRequestsService levelReqs,
        DemonListService demonList
        ) {
        MessageRandomizerService = messageRandomizer;
        ModerationService = moderation;
        TextGeneratorService = textGenerator;
        LevelRequestsService = levelReqs;
        DemonListService = demonList;
    }

    public Restriction GetRequiredRole() {
        return RequiredRole;
    }

    public void SetRequiredRole(Restriction requiredRole) {
        _saveData!.RequiredRole = requiredRole;
        Save();
    }

    public int GetModActionIndex() {
        return ModActionIndex;
    }

    public void SetModActionIndex(int index) {
        _saveData!.ModActionIndex = index;
        Save();
    }

    public void SetCooldown(int cooldown) {
        _saveData!.Cooldown = cooldown;
        Save();
    }

    public State GetVerboseState() {
        return VerboseState;
    }

    public void SetVerboseState(State state) {
        _saveData!.VerboseState = state;
        Save();
    }
}