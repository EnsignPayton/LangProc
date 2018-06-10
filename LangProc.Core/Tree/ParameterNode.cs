namespace LangProc.Core.Tree
{
    public class ParameterNode : TreeNode<Token>
    {
        public ParameterNode(VariableNode variable, TypeNode type) : base(null)
        {
            Variable = variable;
            Type = type;
        }

        public VariableNode Variable { get; }
        public TypeNode Type { get; }
    }
}
