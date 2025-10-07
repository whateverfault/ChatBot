using System.Globalization;
using System.Text;
using ChatBot.api.basic.trees;
using ChatBot.bot.services.interpreter.lexer;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.shared;

namespace ChatBot.bot.services.interpreter.data;

public enum OpScope {
    None = 0,
    Left = 1,
    Right = 2,
    LeftRight = 3,
}

public enum Priority {
    SpecialLow,
    Lowest,
    Low,
    High,
    Highest,
}

public static class OperationList {
    private static readonly InterpreterService _interpreter =
        (InterpreterService)ServiceManager.GetService(ServiceName.Interpreter);

    private static readonly StringBuilder _sb = new StringBuilder(); 

    private static readonly List<Operation> _operations = [
                                                              new Operation(
                                                                            "+",
                                                                            Addition,
                                                                            OpScope.LeftRight,
                                                                            Priority.Lowest
                                                                           ),
                                                              new Operation(
                                                                            "+",
                                                                            Addition,
                                                                            OpScope.Right,
                                                                            Priority.Highest
                                                                           ),
                                                              new Operation(
                                                                            "-",
                                                                            Subtraction,
                                                                            OpScope.LeftRight,
                                                                            Priority.Lowest
                                                                           ),
                                                              new Operation(
                                                                            "-",
                                                                            Subtraction,
                                                                            OpScope.Right,
                                                                            Priority.Highest
                                                                           ),
                                                              new Operation(
                                                                            "*",
                                                                            Multiplication,
                                                                            OpScope.LeftRight,
                                                                            Priority.High
                                                                           ),
                                                              new Operation(
                                                                            "/",
                                                                            Division,
                                                                            OpScope.LeftRight,
                                                                            Priority.High
                                                                           ),
                                                              new Operation(
                                                                            "%",
                                                                            Modulo,
                                                                            OpScope.LeftRight,
                                                                            Priority.High
                                                                           ),
                                                              new Operation(
                                                                            "%",
                                                                            Modulo,
                                                                            OpScope.Left,
                                                                            Priority.Highest
                                                                           ),
                                                              new Operation(
                                                                            "^",
                                                                            Exponentiation,
                                                                            OpScope.LeftRight,
                                                                            Priority.Low
                                                                           ),
                                                              new Operation(
                                                                            "&",
                                                                            And,
                                                                            OpScope.LeftRight,
                                                                            Priority.Low
                                                                           ),
                                                              new Operation(
                                                                            "|",
                                                                            Or,
                                                                            OpScope.LeftRight,
                                                                            Priority.Low
                                                                           ),
                                                              new Operation(
                                                                            "!",
                                                                            Not,
                                                                            OpScope.Right,
                                                                            Priority.Highest
                                                                           ),
                                                              new Operation(
                                                                            "==",
                                                                            Equal,
                                                                            OpScope.LeftRight,
                                                                            Priority.Low
                                                                           ),
                                                              new Operation(
                                                                            "!=",
                                                                            NotEqual,
                                                                            OpScope.LeftRight,
                                                                            Priority.Low
                                                                           ),
                                                              new Operation(
                                                                            ">",
                                                                            Greater,
                                                                            OpScope.LeftRight,
                                                                            Priority.Low
                                                                           ),
                                                              new Operation(
                                                                            "<",
                                                                            Less,
                                                                            OpScope.LeftRight,
                                                                            Priority.Low
                                                                           ),
                                                              new Operation(
                                                                            ">=",
                                                                            GreaterOrEqual,
                                                                            OpScope.LeftRight,
                                                                            Priority.Low
                                                                           ),
                                                              new Operation(
                                                                            "<=",
                                                                            LessOrEqual,
                                                                            OpScope.LeftRight,
                                                                            Priority.Low
                                                                           ),
                                                              new Operation(
                                                                            "=",
                                                                            Assigment,
                                                                            OpScope.LeftRight,
                                                                            Priority.SpecialLow
                                                                           ),
                                                          ];
    public static Operation? GetOperation(string name, OpScope scope = OpScope.None) {
        if (string.IsNullOrEmpty(name)) return null;
        
        var candidates = _operations.Where(op => op.Name.Equals(name)).ToList();
        if (candidates.Count <= 0) return null;
        
        var result = candidates[0];
        _sb.Clear();
        
        if (scope != OpScope.None && result.Scope != scope) {
            return candidates
                  .Where(candidate => candidate.Scope == scope)
                  .Select(candidate => candidate)
                  .FirstOrDefault();
        }
        return result;
    }

    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> Assigment(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (left == null || right == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.InvalidSyntax);
        if (_interpreter.Calculator == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.NotInitialized);
        
        if (left.Data.Type != ExpressionTokenType.Name) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var result = _interpreter.Calculator.Calculate(new BinaryTree<ExpressionToken>(right));
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);

        right.Data = result.Value;
        var variable = 
            _interpreter.GetVariable(left.Data.Value)
         ?? _interpreter.SetVariable(left.Data.Value, right.Data.Value, _interpreter.TokenTypeToVarType(right.Data.Type));

        if (variable == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        variable.SetValue(right.Data.Value, _interpreter.TokenTypeToVarType(right.Data.Type));
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(right), null);
    }
    
    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> Addition(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        if (left == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(right), null);

        var result = ParseNumbers(left, right);
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);

        var lval = result.Value.Left ?? 0;
        var rval = result.Value.Right;
        if (_interpreter.IsLiteral(left.Data) || _interpreter.IsLiteral(right.Data)) {
            _sb.Append($"{left.Data.Value}{right.Data.Value}");
            var strToken = new ExpressionToken(ExpressionTokenType.String, _sb.ToString());
            return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(strToken), null);
        }
        
        if (rval == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        var numToken = new ExpressionToken(ExpressionTokenType.Number, (lval + (double)rval).ToString(CultureInfo.InvariantCulture));
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(numToken), null);
    }
    
    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> Subtraction(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        if (left == null) {
            right.Data.Value = $"-{right.Data.Value}";
            return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(right), null);
        }
        
        var result = ParseNumbers(left, right);
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var lval = result.Value.Left ?? 0;
        var rval = result.Value.Right;
        if (rval is null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var token = new ExpressionToken(ExpressionTokenType.Number, (lval - (double)rval).ToString(CultureInfo.InvariantCulture));
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(token), null);
    }
    
    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> Multiplication(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null || left == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var result = ParseNumbers(left, right);
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var lval = result.Value.Left;
        var rval = result.Value.Right;
        if (rval is null || lval is null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var token = new ExpressionToken(ExpressionTokenType.Number, ((double)lval * (double)rval).ToString(CultureInfo.InvariantCulture));
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(token), null);
    }
    
    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> Division(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null || left == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var result = ParseNumbers(left, right);
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var lval = result.Value.Left;
        var rval = result.Value.Right;
        if (rval is null || lval is null || rval == 0.0) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var token = new ExpressionToken(ExpressionTokenType.Number, ((double)lval / (double)rval).ToString(CultureInfo.InvariantCulture));
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(token), null);
    }
    
    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> Modulo(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var result = ParseNumbers(left, right);
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var lval = result.Value.Left;
        var rval = result.Value.Right ?? 0;
        if (lval is null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var token = rval switch {
                        0 => new ExpressionToken(ExpressionTokenType.Number, ((double)lval / 100).ToString(CultureInfo.InvariantCulture)),
                        _ => new ExpressionToken(ExpressionTokenType.Number, ((double)lval % rval).ToString(CultureInfo.InvariantCulture)),
                    };
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(token), null);
    }
    
    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> Exponentiation(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null || left == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var result = ParseNumbers(left, right);
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var lval = result.Value.Left;
        var rval = result.Value.Right;
        if (rval is null || lval is null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);

        var token = new ExpressionToken(ExpressionTokenType.Number, (Math.Pow((double)lval, (double)rval)).ToString(CultureInfo.InvariantCulture));
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(token), null);
    }
    
    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> And(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null || left == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var result = ParseNumbers(left, right);
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var lval = result.Value.Left;
        var rval = result.Value.Right;
        if (rval is null || lval is null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);

        var token = new ExpressionToken(ExpressionTokenType.Bool, _interpreter.LogicalBoolToBool(lval != 0 && rval != 0).ToString(CultureInfo.InvariantCulture));
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(token), null);
    }
    
    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> Or(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null || left == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);

        var result = ParseNumbers(left, right);
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var lval = result.Value.Left;
        var rval = result.Value.Right;
        if (rval is null || lval is null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);

        var token = new ExpressionToken(ExpressionTokenType.Bool, _interpreter.LogicalBoolToBool(lval != 0 || rval != 0).ToString(CultureInfo.InvariantCulture));
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(token), null);
    }
    
    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> Not(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var result = ParseNumbers(left, right);
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var rval = result.Value.Right;
        if (rval is null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var token = new ExpressionToken(ExpressionTokenType.Bool, _interpreter.LogicalBoolToBool(rval == 0).ToString(CultureInfo.InvariantCulture));
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(token), null);
    }
    
    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> Equal(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null || left == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var result = ParseNumbers(left, right);
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var lval = result.Value.Left;
        var rval = result.Value.Right;
        if (rval is null || lval is null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);

        var token = new ExpressionToken(ExpressionTokenType.Bool, _interpreter.LogicalBoolToBool(lval.Equals(rval)).ToString(CultureInfo.InvariantCulture));
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(token), null);
    }
    
    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> NotEqual(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null || left == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var result = ParseNumbers(left, right);
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var lval = result.Value.Left;
        var rval = result.Value.Right;
        if (rval is null || lval is null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);

        var token = new ExpressionToken(ExpressionTokenType.Bool, _interpreter.LogicalBoolToBool(!lval.Equals(rval)).ToString(CultureInfo.InvariantCulture));
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(token), null);
    }
    
    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> Greater(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null || left == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var result = ParseNumbers(left, right);
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var lval = result.Value.Left;
        var rval = result.Value.Right;
        if (rval is null || lval is null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);

        var token = new ExpressionToken(ExpressionTokenType.Bool, _interpreter.LogicalBoolToBool(lval > rval).ToString(CultureInfo.InvariantCulture));
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(token), null);
    }
    
    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> Less(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null || left == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var result = ParseNumbers(left, right);
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var lval = result.Value.Left;
        var rval = result.Value.Right;
        if (rval is null || lval is null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);

        var token = new ExpressionToken(ExpressionTokenType.Bool, _interpreter.LogicalBoolToBool(lval < rval).ToString(CultureInfo.InvariantCulture));
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(token), null);
    }
    
    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> GreaterOrEqual(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null || left == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var result = ParseNumbers(left, right);
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var lval = result.Value.Left;
        var rval = result.Value.Right;
        if (rval is null || lval is null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);

        var token = new ExpressionToken(ExpressionTokenType.Bool, _interpreter.LogicalBoolToBool(lval >= rval).ToString(CultureInfo.InvariantCulture));
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(token), null);
    }
    
    private static Result<BinaryTree<ExpressionToken>?, ErrorCode?> LessOrEqual(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null || left == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);
        
        var result = ParseNumbers(left, right);
        if (!result.Ok) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var lval = result.Value.Left;
        var rval = result.Value.Right;
        if (rval is null || lval is null) return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(null, ErrorCode.IllegalOperation);

        var token = new ExpressionToken(ExpressionTokenType.Bool, _interpreter.LogicalBoolToBool(lval <= rval).ToString(CultureInfo.InvariantCulture));
        return new Result<BinaryTree<ExpressionToken>?, ErrorCode?>(new BinaryTree<ExpressionToken>(token), null);
    }

    private static Result<NumberPair?, ErrorCode?> ParseNumbers(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right) {
        if (right == null && left == null) return new Result<NumberPair?, ErrorCode?>(null, ErrorCode.IllegalOperation);

        var lval = 0.0;
        var rval = 0.0;
        
        var lvalResult = true;
        var rvalResult = true;
        
        if (left == null || !double.TryParse(left.Data.Value, out lval)) lvalResult = false;
        if (right == null || !double.TryParse(right.Data.Value, out rval)) rvalResult = false;

        if (left is { Data.Type: ExpressionTokenType.Bool, }) {
            if (!_interpreter.BoolToNumber(left.Data, out lval)) {
                return new Result<NumberPair?, ErrorCode?>(null, ErrorCode.IllegalOperation);
            }
            lvalResult = true;
        } if (right is { Data.Type: ExpressionTokenType.Bool, }) {
            if (!_interpreter.BoolToNumber(right.Data, out rval)) {
                return new Result<NumberPair?, ErrorCode?>(null, ErrorCode.IllegalOperation);
            }
            rvalResult = true;
        }
        
        double? l = lvalResult? lval : null;
        double? r = rvalResult? rval : null;
        return new Result<NumberPair?, ErrorCode?>(new NumberPair(l, r), null);
    }
}