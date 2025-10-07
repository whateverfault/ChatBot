using ChatBot.api.basic.trees;
using ChatBot.bot.services.interpreter.data;
using ChatBot.bot.services.interpreter.lexer;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.shared;

namespace ChatBot.bot.services.interpreter.executor;

public class OperationExecutor {
    private static readonly InterpreterService _interpreter =
        (InterpreterService)ServiceManager.GetService(ServiceName.Interpreter);
    
    
    public Result<ExpressionToken?, ErrorCode?> Execute(BinaryTree<ExpressionToken> expression) {
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
            if (!_interpreter.IsOperation(data)) current = current.Parent;
            if (current == null) {
                current = root;
                break;
            }

            var left = current.Left?.Data;
            var right = current.Right?.Data;
            
            var operation = OperationList.GetOperation(data.Value); 
            if (data.Type == ExpressionTokenType.SpecialOperation
             && left == null
             && right == null) {
                if (operation is not { Scope: OpScope.None, }) return new Result<ExpressionToken?, ErrorCode?>(null, ErrorCode.InvalidSyntax);
            }

            if (current.Left != null && left?.Type is ExpressionTokenType.SpecialOperation) {
                current = current.Left;
                continue;
            } if (current.Right != null && right?.Type is ExpressionTokenType.SpecialOperation) {
                current = current.Right;
                continue;
            }
            
            if (operation?.Evaluation == null) return new Result<ExpressionToken?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
            
            var evalResult = operation.Evaluation.Invoke(current.Left, current.Right);
            if (!evalResult.Ok) return new Result<ExpressionToken?, ErrorCode?>(null, evalResult.Error);
            if (evalResult.Value == null) return new Result<ExpressionToken?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
            
            current.Left = null;
            current.Right = null;
            current.Data = evalResult.Value.Root.Data;
        }
        return new Result<ExpressionToken?, ErrorCode?>(current.Data, null);
    }
}