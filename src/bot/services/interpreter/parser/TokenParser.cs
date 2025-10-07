using ChatBot.api.basic.trees;
using ChatBot.bot.services.interpreter.data;
using ChatBot.bot.services.interpreter.lexer;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.shared;

namespace ChatBot.bot.services.interpreter.parser;

public class TokenParser {
    private static readonly InterpreterService _interpreter =
        (InterpreterService)ServiceManager.GetService(ServiceName.Interpreter);
    
    
    public Result<BinaryTree<ExpressionToken>, ErrorCode?> Parse(List<BinaryTreeNode<ExpressionToken>> nodes, int pos = 0) {
        for (var i = pos; i < nodes.Count; ++i) {
            var node = nodes[i];
            var data = node.Data;

            if (data.Type != ExpressionTokenType.OpeningParenthesis) continue;
            nodes.RemoveAt(i);
            Parse(nodes, i); break;
        }
        
        if (_interpreter.Calculator == null) return new Result<BinaryTree<ExpressionToken>, ErrorCode?>(null, ErrorCode.NotInitialized);
        if (nodes.Count <= 0) return new Result<BinaryTree<ExpressionToken>, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        if (nodes.Count == 1 && !_interpreter.IsOperation(nodes[0].Data)) {
            var node = nodes[0];
            if (node.Data.Type == ExpressionTokenType.Name) {
                if (!_interpreter.AssignVarToName(node.Data)) {
                    return new Result<BinaryTree<ExpressionToken>, ErrorCode?>(null, ErrorCode.UnknownIdentifier);
                }
            }
            return new Result<BinaryTree<ExpressionToken>, ErrorCode?>(new BinaryTree<ExpressionToken>(nodes[0]), null);
        }
        
        var scopeEndIndex = nodes.Count;
        do {
            var maxPriority = int.MinValue;
            Operation? maxOp = null;
            var maxIndex = 0;

            for (var i = pos; i < scopeEndIndex; ++i) {
                var node = nodes[i];
                var data = node.Data;

                if (data.Type == ExpressionTokenType.ClosingParenthesis) {
                    nodes.RemoveAt(i);
                    scopeEndIndex = i; break;
                }
                
                if (!_interpreter.IsOperation(data)
                 || node.Left != null 
                 || node.Right != null) continue;
                
                var op = OperationList.GetOperation(data.Value, GetOperationScope(i, nodes));
                if (op == null) continue;

                if ((int)op.Priority <= maxPriority) continue;
                maxPriority = (int)op.Priority;
                maxOp = op;
                maxIndex = i;
            }

            if (maxOp == null) {
                return new Result<BinaryTree<ExpressionToken>, ErrorCode?>(null, ErrorCode.IllegalOperation);
            }
            
            if (maxIndex > 0 && maxOp.Scope is OpScope.Left or OpScope.LeftRight) {
                nodes[maxIndex].Left = nodes[maxIndex - 1];
                nodes[maxIndex - 1].Parent = nodes[maxIndex];
                nodes.RemoveAt(--maxIndex); --scopeEndIndex;
            } if (maxIndex < nodes.Count - 1 && maxOp.Scope is OpScope.Right or OpScope.LeftRight) {
                if (nodes[maxIndex + 1].Data.Type == ExpressionTokenType.ClosingParenthesis) {
                    return new Result<BinaryTree<ExpressionToken>, ErrorCode?>(null, ErrorCode.InvalidSyntax);
                }
                nodes[maxIndex].Right = nodes[maxIndex + 1];
                nodes[maxIndex + 1].Parent = nodes[maxIndex];
                nodes.RemoveAt(maxIndex + 1); --scopeEndIndex;
            }

            if (!ResolveVariables(nodes[maxIndex])) return new Result<BinaryTree<ExpressionToken>, ErrorCode?>(null, ErrorCode.UnknownIdentifier);
            
            var result = _interpreter.Calculator.Calculate(new BinaryTree<ExpressionToken>(nodes[maxIndex]));
            if (!result.Ok) return new Result<BinaryTree<ExpressionToken>, ErrorCode?>(null, result.Error);
            if (result.Value == null) return new Result<BinaryTree<ExpressionToken>, ErrorCode?>(null, ErrorCode.SmthWentWrong);
            
            nodes[maxIndex].Data.Type = result.Value.Type;
            nodes[maxIndex].Data.Value = result.Value.Value;
        } while (scopeEndIndex - pos > 1 && nodes.Count > 1);
        
        return new Result<BinaryTree<ExpressionToken>, ErrorCode?>(new BinaryTree<ExpressionToken>(nodes[0]), null);
    }

    private OpScope GetOperationScope(int pos, List<BinaryTreeNode<ExpressionToken>> nodes) {
        var scope = OpScope.None;
        if (pos > 0 && _interpreter.IsValue(nodes[pos - 1].Data)) scope = OpScope.Left;
        if (pos < nodes.Count - 1 && _interpreter.IsValue(nodes[pos + 1].Data)) {
            scope = scope == OpScope.Left ? OpScope.LeftRight : OpScope.Right;
        }

        return scope;
    }
    
    private bool ResolveVariables(BinaryTreeNode<ExpressionToken> node) {
        var data = node.Data;
        if (data.Type != ExpressionTokenType.ArithmeticOperation) return true;

        var left = node.Left;
        var right = node.Right;
        
        if (left?.Data.Type == ExpressionTokenType.Name) {
            if (!_interpreter.AssignVarToName(left.Data)) return false;
        }

        return right?.Data.Type != ExpressionTokenType.Name || _interpreter.AssignVarToName(right.Data);
    }
}