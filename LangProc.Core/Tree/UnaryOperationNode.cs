namespace LangProc.Core.Tree
{
    public class UnaryOperationNode : TreeNode<Token>
    {
        public UnaryOperationNode(Token token, TreeNode<Token> value)
            : base(token)
        {
            Value = value;
        }

        public TreeNode<Token> Value { get; }
    }
}
