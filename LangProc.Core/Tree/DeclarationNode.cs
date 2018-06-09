namespace LangProc.Core.Tree
{
    public class DeclarationNode : TreeNode<Token>
    {
        public DeclarationNode(VariableNode variableNode, TypeNode typeNode) : base(null)
        {
            VariableNode = variableNode;
            TypeNode = typeNode;
        }

        public VariableNode VariableNode { get; }
        public TypeNode TypeNode { get; }
    }
}
