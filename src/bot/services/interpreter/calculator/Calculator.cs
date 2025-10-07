using ChatBot.api.basic.trees;
using ChatBot.bot.services.interpreter.data;
using ChatBot.bot.services.interpreter.lexer;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.shared;

namespace ChatBot.bot.services.interpreter.calculator;

public class Calculator {
    private static readonly InterpreterService _interpreter =
        (InterpreterService)ServiceManager.GetService(ServiceName.Interpreter);
    
    
    public Result<ExpressionToken?, ErrorCode?> Calculate(BinaryTree<ExpressionToken> expression) {
        var ast = expression;
        if (ast.Root.Parent != null) {
            ast = new BinaryTree<ExpressionToken>(expression) {
                                                                   Root = {
                                                                              Parent = null,
                                                                          },
                                                               };
        }
        
        var root = ast.Root;
        var current = root;
        
        while (true) {
            var data = current.Data;
            if (!_interpreter.IsOperation(current.Data)) current = current.Parent;
            if (current == null) {
                current = root;
                break;
            }

            var left = current.Left?.Data;
            var right = current.Right?.Data;
            if (data.Type == ExpressionTokenType.ArithmeticOperation
             && left == null
             && right == null) {
                var susOp = GetOperation(data.Value);
                if (susOp is not { Scope: OpScope.None, }) return new Result<ExpressionToken?, ErrorCode?>(null, ErrorCode.InvalidSyntax);
            }
            
            var scope = OpScope.None;
            if (left != null) scope = OpScope.Left;
            if (right != null) scope = scope == OpScope.Left ? OpScope.LeftRight : OpScope.Right;
            
            var op = GetOperation(current.Data.Value, scope);
            if (op == null) return new Result<ExpressionToken?, ErrorCode?>(null, ErrorCode.IllegalOperation);
            
            var result = op.Evaluation.Invoke(current.Left, current.Right);
            if (!result.Ok) return new Result<ExpressionToken?, ErrorCode?>(null, result.Error);
            if (result.Value == null) return new Result<ExpressionToken?, ErrorCode?>(null, ErrorCode.SmthWentWrong);

            current.Left = null;
            current.Right = null;
            current.Data.Value = result.Value.Root.Data.Value;
            current.Data.Type = result.Value.Root.Data.Type;
        }

        if (current.Data.Type == ExpressionTokenType.Name) {
            if (!_interpreter.AssignVarToName(current.Data)) return new Result<ExpressionToken?, ErrorCode?>(null, ErrorCode.UnknownIdentifier);
        }
        return new Result<ExpressionToken?, ErrorCode?>(current.Data, null);
    }
    
    private Operation? GetOperation(string name, OpScope scope = OpScope.None) {
        var operation = OperationList.GetOperation(name, scope);
        return operation;
    }
}