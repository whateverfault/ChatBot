namespace ChatBot.api.basic.trees;

public class BinaryTree<T> {
    public BinaryTreeNode<T>? Root { get; set; }
    
    
    public BinaryTree(BinaryTreeNode<T>? root = null) {
        Root = root;
    }
}