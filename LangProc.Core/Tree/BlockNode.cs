using System.Collections.Generic;

namespace LangProc.Core.Tree
{
    public class BlockNode : TreeNode<Token>
    {
        public BlockNode(IEnumerable<TreeNode<Token>> declarationNodes, CompoundNode compoundNode) : base(null)
        {
            Declarations = declarationNodes;
            CompoundNode = compoundNode;
        }

        public IEnumerable<TreeNode<Token>> Declarations { get; }
        public CompoundNode CompoundNode { get; }
    }
}
