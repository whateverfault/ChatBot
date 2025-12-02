using ChatBot.bot.services.interfaces;
using ChatBot.bot.shared.handlers;
using sonlanglib.interpreter;
using sonlanglib.interpreter.data.vars;
using TwitchAPI.shared;

namespace ChatBot.bot.services.interpreter;

public class InterpreterService : Service {
    public override InterpreterOptions Options { get; } = new InterpreterOptions();
    
    public Interpreter Interpreter { get; } = new Interpreter();
        
        
    public Result<string?, string?> Evaluate(string expression) {
        var result = Interpreter.Evaluate(expression);
        if (!result.Ok) return new Result<string?, string?>(null, result.Error);
        if (result.Value == null) return new Result<string?, string?>(null, ErrorHandler.GetErrorString(ErrorCode.SmthWentWrong));
        
        return new Result<string?, string?>(result.Value, result.Error);
    }

    public override void Init() {
        base.Init();
        LoadVars();
    }

    private void LoadVars() {
        var vars = Options.GetVariables();
        var casted = new Dictionary<string, Variable>();
        
        foreach (var (name, var) in vars) {
            var varVal = new VarValue(var.Name, var.Type);
            casted.Add(name, new Variable(var.Name, varVal));
        }
        
        Interpreter.LoadVars(casted);
    }
}