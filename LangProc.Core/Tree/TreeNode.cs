namespace LangProc.Core.Tree
{
    public abstract class TreeNode<T>
    {
        protected TreeNode(T data)
        {
            Data = data;
        }

        public T Data { get; }
    }
}
