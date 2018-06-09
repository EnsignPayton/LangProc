namespace LangProc.Core.Symbols
{
    public abstract class Symbol
    {
        protected Symbol(string name, object type = null)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public object Type { get; }
    }
}
