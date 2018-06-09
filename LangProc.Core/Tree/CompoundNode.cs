using System.Collections.Generic;

namespace LangProc.Core.Tree
{
    public class CompoundNode : TreeNode<Token>
    {
        public CompoundNode(IEnumerable<TreeNode<Token>> children) : base(null)
        {
            Children = children ?? new List<TreeNode<Token>>();
        }

        public IEnumerable<TreeNode<Token>> Children { get; }
    }
}
