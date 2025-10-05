using System.Text;
using ChatBot.api.basic.trees;
using ChatBot.bot.services.calculator.data;
using ChatBot.bot.services.interfaces;
using ChatBot.bot.services.Static;
using ChatBot.bot.shared.handlers;
using TwitchAPI.client;
using TwitchAPI.shared;

namespace ChatBot.bot.services.calculator;

public class CalculatorService : Service {
    public override string Name => ServiceName.Calculator;
    public override CalculatorOptions Options { get; } = new CalculatorOptions();


    public Result<double?, ErrorCode?> Calculate(string expression) {
        expression = expression.Replace(" ", "")
                               .Trim();

        var opened = expression.Count(c => c == '(');
        var closed = expression.Count(c => c == ')');
        if (opened != closed)  return new Result<double?, ErrorCode?>(null, ErrorCode.UnbalancedParentheses);
        
        foreach (var c in expression) {
            ErrorHandler.LogMessage(LogLevel.Info, $"{c}: {(int)c}");
        }
        
        var result = Parse(expression, out _);
        if (!result.Ok) return new Result<double?, ErrorCode?>(null, result.Error);
        if (result.Value == null) return new Result<double?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var res = BuildTree(result.Value);
        if (!res.Ok) return new Result<double?, ErrorCode?>(null, res.Error);
        if (res.Value == null) return new Result<double?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var rslt = Evaluate(res.Value);
        if (!rslt.Ok) return new Result<double?, ErrorCode?>(null, rslt.Error);
        if (rslt.Value == null) return new Result<double?, ErrorCode?>(null, ErrorCode.SmthWentWrong);

        return new Result<double?, ErrorCode?>(rslt.Value, null);
    }

    private Result<List<BinaryTreeNode<ExprNode>>, ErrorCode?> Parse(string expr, out int pos, int start = 0) {
        var nodes = new List<BinaryTreeNode<ExprNode>>();
        var sb = new StringBuilder();
        pos = start;

        for (; pos < expr.Length;) {
            var isDigit = false;
            while (pos < expr.Length && (char.IsDigit(expr[pos]) || expr[pos] == '.' || expr[pos] == ',')) {
                sb.Append(expr[pos++]);
                isDigit = true;
            }

            if (isDigit) {
                nodes.Add(
                          new BinaryTreeNode<ExprNode>(
                                                       new ExprNode(sb.ToString(), ExprNodeKind.Num)
                                                       )
                          );
            } else {
                var subEquation = false;
                while (pos < expr.Length && !char.IsDigit(expr[pos]) && Options.GetOperation(sb.ToString()) == null) {
                    if (expr[pos] == ')') {
                        ++pos;
                        return new Result<List<BinaryTreeNode<ExprNode>>, ErrorCode?>(nodes, null);
                    }
                    if (expr[pos] == '(') {
                        if (sb.Length > 0) {
                            nodes.Add(
                                      new BinaryTreeNode<ExprNode>(
                                                                   new ExprNode(sb.ToString(), ExprNodeKind.Op)
                                                                  )
                                     );
                            sb.Clear();
                        }

                        subEquation = true;
                        ++pos;

                        var result = Parse(expr, out pos, pos);
                        if (!result.Ok) return result;
                        if (result.Value == null) return new Result<List<BinaryTreeNode<ExprNode>>, ErrorCode?>(null, ErrorCode.SmthWentWrong);

                        var res = BuildTree(result.Value);
                        if (!res.Ok) return new Result<List<BinaryTreeNode<ExprNode>>, ErrorCode?>(null, res.Error);
                        if (res.Value?.Root == null) return new Result<List<BinaryTreeNode<ExprNode>>, ErrorCode?>(null, ErrorCode.SmthWentWrong);
                        
                        nodes.Add(res.Value.Root);
                        break;
                    }

                    sb.Append(expr[pos++]);
                }
                if (subEquation) continue;
                nodes.Add(
                          new BinaryTreeNode<ExprNode>(
                                                       new ExprNode(sb.ToString(), ExprNodeKind.Op)
                                                      )
                         );
            }
            sb.Clear();
        }

        return new Result<List<BinaryTreeNode<ExprNode>>, ErrorCode?>(nodes, null);
    }

    private Result<BinaryTree<ExprNode>, ErrorCode?> BuildTree(List<BinaryTreeNode<ExprNode>> nodes) {
        if (nodes.Count <= 0) return new Result<BinaryTree<ExprNode>, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        if (nodes is [{ Data.Kind: ExprNodeKind.Num, },]) {
            return new Result<BinaryTree<ExprNode>, ErrorCode?>(new BinaryTree<ExprNode>(nodes[0].Data), null);
        }
        
        do {
            var maxPriority = int.MinValue;
            var maxIndex = 0;

            for (var i = 0; i < nodes.Count; ++i) {
                var node = nodes[i];
                var data = node.Data;
                
                if (data.Kind != ExprNodeKind.Op
                 || node.Left != null 
                 || node.Right != null) continue;

                var op = Options.GetOperation(data.Value);
                if (op == null) return new Result<BinaryTree<ExprNode>, ErrorCode?>(null, ErrorCode.IllegalOperation);

                if ((int)op.Priority <= maxPriority) continue;
                maxPriority = (int)op.Priority;
                maxIndex = i;
            }
            
            var maxOp = Options.GetOperation(nodes[maxIndex].Data.Value);
            if (maxOp == null) return new Result<BinaryTree<ExprNode>, ErrorCode?>(null, ErrorCode.IllegalOperation);
            
            if (maxIndex > 0 && maxOp.Scope is OpScope.Left or OpScope.LeftRight) {
                nodes[maxIndex].Left = nodes[maxIndex - 1];
                nodes[maxIndex - 1].Parent = nodes[maxIndex];
                nodes.RemoveAt(--maxIndex);
            }
            if (maxIndex < nodes.Count - 1 && maxOp.Scope is OpScope.Right or OpScope.LeftRight) {
                nodes[maxIndex].Right = nodes[maxIndex + 1];
                nodes[maxIndex + 1].Parent = nodes[maxIndex];
                nodes.RemoveAt(maxIndex + 1);
            }
        } while (nodes.Count > 1);

        return new Result<BinaryTree<ExprNode>, ErrorCode?>(new BinaryTree<ExprNode>(nodes[0]), null);
    }
    
    private Result<double?, ErrorCode?> Evaluate(BinaryTree<ExprNode> expression) {
        var root = expression.Root;
        if (root == null) return new Result<double?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        
        var current = root;
        while (true) {
            var data = current.Data;
            if (data.Kind == ExprNodeKind.Num) current = current.Parent;
            if (current == null) {
                current = root;
                break;
            }

            var left = current.Left?.Data;
            var right = current.Right?.Data;
            if (data.Kind == ExprNodeKind.Op
             && left == null
             && right == null) {
                var susOp = Options.GetOperation(data.Value);
                if (susOp is not { Scope: OpScope.None, }) return new Result<double?, ErrorCode?>(null, ErrorCode.InvalidSyntax);
            }

            var lval = 0.0;
            var rval = 0.0;

            if (left is { Kind: ExprNodeKind.Num, }) {
                if (!double.TryParse(left.Value, out lval)) lval = 0;
            } else if (left != null && current.Left != null) {
                current = current.Left;
                continue;
            }
            
            if (right is { Kind: ExprNodeKind.Num, }) {
                if (!double.TryParse(right.Value, out rval)) rval = 0;
            } else if (right != null && current.Right != null) {
                current = current.Right;
                continue;
            }

            var op = Options.GetOperation(current.Data.Value);
            if (op == null) return new Result<double?, ErrorCode?>(null, ErrorCode.IllegalOperation);

            var result = op.Evaluation.Invoke(lval, rval);
            if (!result.Ok) return result;
            if (result.Value == null) return new Result<double?, ErrorCode?>(null, ErrorCode.SmthWentWrong);

            current.Left = null;
            current.Right = null;
            current.Data.Value = result.Value.ToString() ?? string.Empty;
            current.Data.ChangeKind(ExprNodeKind.Num);
        }

        if (!double.TryParse(current.Data.Value, out var answer)) {
            return new Result<double?, ErrorCode?>(null, ErrorCode.SmthWentWrong);
        }

        return new Result<double?, ErrorCode?>(answer, null);
    }
}