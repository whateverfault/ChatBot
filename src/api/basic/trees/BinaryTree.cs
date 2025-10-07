namespace ChatBot.api.basic.trees;

public class BinaryTree<T> {
    public BinaryTreeNode<T> Root { get; set; }
    
    
    public BinaryTree(BinaryTreeNode<T> root) {
        Root = root;
    }
    
    public BinaryTree(T data) {
        Root = new BinaryTreeNode<T>(data);
    }
    
    public BinaryTree(BinaryTree<T> tree) {
        Root = tree.Root;
    }
}