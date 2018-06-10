namespace LangProc.Core.Tree
{
    public class ProcedureNode : TreeNode<Token>
    {
        public ProcedureNode(Token token, BlockNode blockNode) : base(token)
        {
            BlockNode = blockNode;
        }

        public BlockNode BlockNode { get; }
    }
}
