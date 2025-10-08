using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.interpreter.data;
using ChatBot.bot.shared;
using sonlanglib.interpreter.data;

namespace ChatBot.bot.services.interpreter;

public class InterpreterOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    protected override string Name => "interpreter";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    private Dictionary<string, StoredVariable> Variables => _saveData!.Variables;
    
    
    public override void Load() {
        if (!Json.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
    }

    public override void Save() {
        lock (_fileLock) {
            Json.WriteSafe(OptionsPath, Path.Combine(Directories.ServiceDirectory, Name), _saveData);
        }
    }

    public override void SetDefaults() {
        _saveData = new SaveData();
        Save();
    }
    
    public override void SetState(State state) {
        _saveData!.ServiceState = state;
    }

    public Dictionary<string, StoredVariable> GetVariables() {
        return Variables;
    }
    
    public void SaveVariable(Variable variable) {
        var var = new StoredVariable(variable);
        if (!Variables.TryAdd(var.Name, var)) {
            Variables[var.Name] = var;
        }
        Save();
    }
}