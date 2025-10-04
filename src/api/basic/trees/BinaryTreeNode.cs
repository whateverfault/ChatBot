namespace ChatBot.api.basic.trees;

public class BinaryTreeNode<T> {
    public BinaryTreeNode<T>? Parent { get; set; }
    public BinaryTreeNode<T>? Left { get; set; }
    public BinaryTreeNode<T>? Right { get; set; }
    public T Data { get; set; }


    public BinaryTreeNode(
        T data,
        BinaryTreeNode<T>? parent = null,
        BinaryTreeNode<T>? left = null,
        BinaryTreeNode<T>? right = null) {
        Data = data;
        Parent = parent;
        Left = left;
        Right = right;
    }
}