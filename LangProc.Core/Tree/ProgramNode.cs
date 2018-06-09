namespace LangProc.Core.Tree
{
    public class ProgramNode : TreeNode<Token>
    {
        public ProgramNode(VariableNode nameNode, BlockNode blockNode) : base(null)
        {
            NameNode = nameNode;
            BlockNode = blockNode;
        }

        public VariableNode NameNode { get; }
        public BlockNode BlockNode { get; }
    }
}
