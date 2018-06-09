namespace LangProc.Core
{
    public class TokenNode : TreeNode<Token>
    {
        public TokenNode(Token token, TokenNode leftChild = null, TokenNode rightChild = null)
            : base(token, leftChild, rightChild)
        {
        }

        public bool IsUnary { get; set; }

        public new TokenNode LeftChild => (TokenNode) base.LeftChild;

        public new TokenNode RightChild => (TokenNode) base.RightChild;
    }
}
