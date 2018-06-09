namespace LangProc.Core.Tree
{
    public class AssignmentNode : TreeNode<Token>
    {
        public AssignmentNode(Token token, TreeNode<Token> leftChild = null, TreeNode<Token> rightChild = null)
            : base(token)
        {
            LeftChild = leftChild;
            RightChild = rightChild;
        }

        public TreeNode<Token> LeftChild { get; }

        public TreeNode<Token> RightChild { get; }
    }
}
