namespace LangProc.Core.Tree
{
    public class BinaryOperationNode : TreeNode<Token>
    {
        public BinaryOperationNode(Token token, TreeNode<Token> leftChild = null, TreeNode<Token> rightChild = null)
            : base(token)
        {
            LeftChild = leftChild;
            RightChild = rightChild;
        }

        public TreeNode<Token> LeftChild { get; }

        public TreeNode<Token> RightChild { get; }
    }
}
