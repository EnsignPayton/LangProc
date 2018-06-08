using System.Collections;
using System.Collections.Generic;

namespace LangProc.Core
{
    public class TreeNode<T> : IEnumerable<T>
    {
        public TreeNode(T data, TreeNode<T> leftChild = null, TreeNode<T> rightChild = null)
        {
            Data = data;
            LeftChild = leftChild;
            RightChild = rightChild;
        }

        public T Data { get; set; }
        public TreeNode<T> LeftChild { get; set; }
        public TreeNode<T> RightChild { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            if (LeftChild != null)
            {
                foreach (var child in LeftChild)
                {
                    yield return child;
                }
            }

            yield return Data;

            if (RightChild != null)
            {
                foreach (var child in RightChild)
                {
                    yield return child;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
