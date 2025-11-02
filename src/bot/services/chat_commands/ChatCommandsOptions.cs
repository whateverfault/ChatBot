using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.chat_commands.data;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.chat_commands;

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
    
    
    public override void Load() {
        var shouldSave = false;
        
        if (!Json.TryRead(OptionsPath, out _saveData!)) {
            _saveData = new SaveData();
            shouldSave = true;
        } if (!Json.TryRead(DefaultCmdsPath, out _defaultCmds!)) {
            _defaultCmds = [];
            CommandsList.SetDefaults();
            shouldSave = true;
        } if (!Json.TryRead(CustomCmdsPath, out _customCmds!)) {
            _customCmds = [];
            shouldSave = true;
        }
        
        if (shouldSave) Save();
    }

    public override void Save() {
        lock (_fileLock) {
            Json.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
            Json.WriteSafe(DefaultCmdsPath, Path.Combine(Directories.ServiceDirectory, Name), _defaultCmds);
            Json.WriteSafe(CustomCmdsPath, Path.Combine(Directories.ServiceDirectory, Name), _customCmds);
        }
    }

    public override void SetDefaults() {
        _saveData = new SaveData();
        _customCmds = [];
        _defaultCmds = [];
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
    
    public void AddChatCmd(CustomChatCommand chatCmd) {
        CustomCmds.Add(chatCmd);
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
        Json.CreateOld(DefaultCmdsPath);
        
        _defaultCmds = cmds;
        Save();
    }
}