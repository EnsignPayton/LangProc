using System.Collections.Generic;

namespace LangProc.Core.Tree
{
    public class ProcedureNode : TreeNode<Token>
    {
        public ProcedureNode(Token token, IEnumerable<ParameterNode> parameters, BlockNode blockNode) : base(token)
        {
            Parameters = parameters;
            BlockNode = blockNode;
        }

        public IEnumerable<ParameterNode> Parameters { get; }
        public BlockNode BlockNode { get; }
    }
}
