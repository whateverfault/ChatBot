using ChatBot.api.basic.trees;
using ChatBot.bot.services.interpreter.lexer;
using ChatBot.bot.shared.handlers;
using TwitchAPI.shared;

namespace ChatBot.bot.services.interpreter.data;

public delegate Result<BinaryTree<ExpressionToken>?, ErrorCode?> EvaluationHandler(BinaryTreeNode<ExpressionToken>? left, BinaryTreeNode<ExpressionToken>? right);

public class Operation {
    public string Name { get; }
    public OpScope Scope { get; }
    public Priority Priority { get; }
    
    public EvaluationHandler Evaluation { get; }
    

    public Operation(string name, EvaluationHandler evaluation, OpScope scope, Priority priority) {
        Name = name;
        Evaluation = evaluation;
        Scope = scope;
        Priority = priority;
    }
}