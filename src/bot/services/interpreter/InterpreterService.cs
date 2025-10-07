using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.interpreter.calculator;
using ChatBot.bot.services.interpreter.data;
using ChatBot.bot.services.interpreter.executor;
using ChatBot.bot.services.interpreter.lexer;
using ChatBot.bot.services.interpreter.parser;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.shared;

namespace ChatBot.bot.services.interpreter;

public class InterpreterService : Service {
    private readonly ExpressionLexer _lexer = new ExpressionLexer();
    private readonly TokenParser _parser = new TokenParser();
    private readonly OperationExecutor _executor = new OperationExecutor();
    
    public override string Name => ServiceName.Interpreter;
    public override InterpreterOptions Options { get; } = new InterpreterOptions();

    public Calculator? Calculator { get; private set; } = new Calculator();

    
    public Result<ExpressionToken?, ErrorCode?> Evaluate(string expression) {
        var lexResult = _lexer.Lex(expression);
        if (!lexResult.Ok) return new Result<ExpressionToken?, ErrorCode?>(null, lexResult.Error);
        if (lexResult.Value == null) return new Result<ExpressionToken?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var parseResult = _parser.Parse(lexResult.Value);
        if (!parseResult.Ok) return new Result<ExpressionToken?, ErrorCode?>(null, parseResult.Error);
        if (parseResult.Value == null) return new Result<ExpressionToken?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        return _executor.Execute(parseResult.Value);
    }
    
    public Variable? SetVariable(string name, string val, VariableType type) {
        if (string.IsNullOrEmpty(name)) return null;
        return Options.SetVariable(name, type, val);
    }

    public ExpressionTokenType VarTypeToTokenType(VariableType varType) {
        return varType switch {
                   VariableType.String => ExpressionTokenType.String,
                   VariableType.Number => ExpressionTokenType.Number,
                   VariableType.Bool => ExpressionTokenType.Bool,
                   _                   => throw new ArgumentOutOfRangeException(nameof(varType), varType, null),
               };
    }
    
    public VariableType TokenTypeToVarType(ExpressionTokenType tokenType) {
        return tokenType switch {
                   ExpressionTokenType.String => VariableType.String,
                   ExpressionTokenType.Number => VariableType.Number,
                   ExpressionTokenType.Bool => VariableType.Bool,
                   _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, null),
               };
    }

    public bool BoolToNumber(ExpressionToken token, out double value) {
        value = -1.0;
        if (token.Type != ExpressionTokenType.Bool) return false;

        switch (token.Value) {
            case InterpreterConstants.True:
                value = InterpreterConstants.TrueD;
                break;
            case InterpreterConstants.False:
                value = InterpreterConstants.FalseD;
                break;
            default: return false;
        }
        return true;
    }

    public bool NumberToBool(ExpressionToken token, out string value) {
        value = token.Value;
        
        if (token.Type == ExpressionTokenType.Bool) return true;
        if (token.Type != ExpressionTokenType.Number) return false;

        if (!double.TryParse(value, out var valueD)) return false;
        
        switch (valueD) {
            case InterpreterConstants.TrueD:
                value = InterpreterConstants.True;
                break;
            case InterpreterConstants.FalseD:
                value = InterpreterConstants.False;
                break;
            default: return false;
        }

        return true;
    }
    
    public VariableType ImplyVariableType(string value) {
        if (double.TryParse(value, out _)) return VariableType.Number;
        return value switch {
                   InterpreterConstants.False or InterpreterConstants.True => VariableType.Bool,
                   _                                                       => VariableType.String,
               };
    }
    
    public ExpressionTokenType ImplyTokenType(string token) {
        switch (token) {
            case "%":
            case "^":
            case "/":
            case "*":
            case "-":
            case "+":
            case "!":
            case "|":
            case "&":
            case "<":
            case ">":
            case "<=":
            case ">=":
            case "!=":
            case "==": return ExpressionTokenType.ArithmeticOperation;
            case "=":  return ExpressionTokenType.SpecialOperation;
            case "(":  return ExpressionTokenType.OpeningParenthesis;
            case ")":  return ExpressionTokenType.ClosingParenthesis;
            case ";":  return ExpressionTokenType.Semicolon;
        }

        var implied = ImplyVariableType(token);
        return implied == VariableType.String? 
                   ExpressionTokenType.Name :
                   VarTypeToTokenType(implied);
    }
    
    public string LogicalBoolToBool(bool logical) {
        return logical ? InterpreterConstants.True : InterpreterConstants.False;
    }
    
    public bool AssignVarToName(ExpressionToken token) {
        if (token.Type != ExpressionTokenType.Name) return false;
        
        var variable = GetVariable(token.Value);
        if (variable == null) return false;
        
        token.Type = VarTypeToTokenType(variable.Type);
        token.Value = variable.Value;
        return true;
    }

    public bool IsOperation(ExpressionToken token) {
        return token.Type is ExpressionTokenType.ArithmeticOperation or ExpressionTokenType.SpecialOperation;
    }

    public bool IsValue(ExpressionToken token) {
        return token.Type is ExpressionTokenType.String or ExpressionTokenType.Number or ExpressionTokenType.Bool or ExpressionTokenType.Name;
    }
    
    public bool IsLiteral(ExpressionToken token) {
        return token.Type is ExpressionTokenType.String;
    }
    
    public Variable? GetVariable(string name) {
        return string.IsNullOrEmpty(name)? 
                   null :
                   Options.GetVariable(name);
    }
}