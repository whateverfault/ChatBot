using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.interpreter.data;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.interpreter;

public class InterpreterOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    
    protected override string Name => "interpreter";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    private Dictionary<string, Variable> Variables => _saveData!.Variables;
    
    
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

    public Variable SetVariable(string name, VariableType type, string val) {
        var var = new Variable(name, type, val);
        if (!Variables.TryAdd(name, var)) {
            Variables[name] = var;
        }
        Save();
        return var;
    }

    public Variable? GetVariable(string name) {
        Variables.TryGetValue(name, out var var);
        return var;
    }
}