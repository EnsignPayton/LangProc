using System;
using System.Collections.Generic;

namespace LangProc.Core.Tree
{
    public class CompoundNode : TreeNode<Token>
    {
        public CompoundNode(Token token, List<TreeNode<Token>> children) : base(token)
        {
            Children = children ?? new List<TreeNode<Token>>();
        }

        public List<TreeNode<Token>> Children { get; }
    }
}
