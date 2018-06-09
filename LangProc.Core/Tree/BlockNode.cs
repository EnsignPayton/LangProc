using System.Collections.Generic;

namespace LangProc.Core.Tree
{
    public class BlockNode : TreeNode<Token>
    {
        public BlockNode(IEnumerable<DeclarationNode> declarationNodes, CompoundNode compoundNode) : base(null)
        {
            Declarations = declarationNodes;
            CompoundNode = compoundNode;
        }

        public IEnumerable<DeclarationNode> Declarations { get; }
        public CompoundNode CompoundNode { get; }
    }
}
