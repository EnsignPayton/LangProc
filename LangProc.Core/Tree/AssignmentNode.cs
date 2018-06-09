namespace LangProc.Core.Tree
{
    public class AssignmentNode : TreeNode<Token>
    {
        public AssignmentNode(Token token, VariableNode variable = null, TreeNode<Token> value = null)
            : base(token)
        {
            Variable = variable;
            Value = value;
        }

        public VariableNode Variable { get; }

        public TreeNode<Token> Value { get; }
    }
}
