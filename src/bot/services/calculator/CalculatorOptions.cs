using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.calculator.data;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.calculator;

public class CalculatorOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    private Dictionary<string, Operation> _operations = new Dictionary<string, Operation>();
    
    protected override string Name => "calc";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;
    
    
    public override void Load() {
        if (!Json.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }
        
        _operations = new Dictionary<string, Operation> {
                                                   {
                                                       "+",
                                                       new Operation(
                                                                     OperationList.Addition,
                                                                     OpScope.LeftRight,
                                                                     OpPriority.Lowest
                                                                     )
                                                   },
                                                   {
                                                       "-",
                                                       new Operation(
                                                                     OperationList.Subtraction,
                                                                     OpScope.LeftRight,
                                                                     OpPriority.Lowest
                                                                    )
                                                   },
                                                   {
                                                       "*",
                                                       new Operation(
                                                                     OperationList.Multiplication,
                                                                     OpScope.LeftRight,
                                                                     OpPriority.Lower
                                                                    )
                                                   },
                                                   {
                                                       "/",
                                                       new Operation(
                                                                     OperationList.Division,
                                                                     OpScope.LeftRight,
                                                                     OpPriority.Lower
                                                                    )
                                                   },
                                                   {
                                                       "%",
                                                       new Operation(
                                                                     OperationList.Modulo,
                                                                     OpScope.LeftRight,
                                                                     OpPriority.Lower
                                                                    )
                                                   },
                                                   
                                                   {
                                                       "^",
                                                       new Operation(
                                                                     OperationList.Exponentiation,
                                                                     OpScope.LeftRight,
                                                                     OpPriority.Low
                                                                    )
                                                   },
                                               };
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

    public Operation? GetOperation(string name) {
        var result = _operations.FirstOrDefault(op => op.Key.Equals(name));
        return result.Value;
    }
}