using ChatBot.api.json;
using ChatBot.bot.interfaces;
using ChatBot.bot.services.calculator.data;
using ChatBot.bot.shared;

namespace ChatBot.bot.services.calculator;

public class CalculatorOptions : Options {
    private readonly object _fileLock = new object();
    
    private SaveData? _saveData;
    private List<Operation> _operations = [];
    private Dictionary<string, double> _constants = new Dictionary<string, double>();
    
    protected override string Name => "calc";
    protected override string OptionsPath => Path.Combine(Directories.ServiceDirectory+Name, $"{Name}_opt.json");
    
    public override State ServiceState => _saveData!.ServiceState;


    public override void Load() {
        if (!Json.TryRead(OptionsPath, out _saveData!)) {
            SetDefaults();
        }

        _operations = [
                          new Operation(
                                        "+",
                                        OperationList.Addition,
                                        OpScope.LeftRight,
                                        OpPriority.Lowest
                                       ),
                          new Operation(
                                        "+",
                                        OperationList.Addition,
                                        OpScope.Right,
                                        OpPriority.Highest
                                       ),
                          new Operation(
                                        "-",
                                        OperationList.Subtraction,
                                        OpScope.LeftRight,
                                        OpPriority.Lowest
                                       ),
                          new Operation(
                                        "-",
                                        OperationList.Subtraction,
                                        OpScope.Right,
                                        OpPriority.Highest
                                       ),
                          new Operation(
                                        "*",
                                        OperationList.Multiplication,
                                        OpScope.LeftRight,
                                        OpPriority.High
                                       ),
                          new Operation(
                                        "/",
                                        OperationList.Division,
                                        OpScope.LeftRight,
                                        OpPriority.High
                                       ),
                          new Operation(
                                        "%",
                                        OperationList.Modulo,
                                        OpScope.LeftRight,
                                        OpPriority.High
                                       ),
                          new Operation(
                                        "%",
                                        OperationList.Modulo,
                                        OpScope.Left,
                                        OpPriority.Highest
                                       ),
                          new Operation(
                                        "^",
                                        OperationList.Exponentiation,
                                        OpScope.LeftRight,
                                        OpPriority.Low
                                       ),
                          new Operation(
                                        "sin",
                                        OperationList.Sin,
                                        OpScope.Right,
                                        OpPriority.Low
                                       ),
                          new Operation(
                                        "cos",
                                        OperationList.Cos,
                                        OpScope.Right,
                                        OpPriority.Low
                                       ),
                          new Operation(
                                        "tan",
                                        OperationList.Tan,
                                        OpScope.Right,
                                        OpPriority.Low
                                       ),
                          new Operation(
                                        "log",
                                        OperationList.Log,
                                        OpScope.Right,
                                        OpPriority.Low
                                       ),
                          new Operation(
                                        "lg",
                                        OperationList.Lg,
                                        OpScope.Right,
                                        OpPriority.Low
                                       ),
                      ];

        _constants = new Dictionary<string, double> {
                                                        { "pi", Math.PI },
                                                        { "e", Math.E },
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

    public Operation? GetOperation(string name, OpScope scope = OpScope.None) {
        if (string.IsNullOrEmpty(name)) return null;
        
        var candidates = _operations.Where(op => op.Name.Equals(name)).ToList();
        if (candidates.Count <= 0) return null;

        var result = candidates[0];
        if (scope != OpScope.None && result.Scope != scope) {
            return candidates
                  .Where(candidate => candidate.Scope == scope)
                  .Select(candidate => candidate)
                  .FirstOrDefault();
        }
        
        return result;
    }
    
    public double? GetConstant(string name) {
        if (string.IsNullOrEmpty(name)) return null;

        if (!_constants.TryGetValue(name.ToLower(), out var result)) return null;
        return result;
    }
}