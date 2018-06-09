namespace LangProc.Core
{
    public class Token
    {
        public Token(TokenType type, object value = null)
        {
            Type = type;
            Value = value;
        }

        public TokenType Type { get; }
        public object Value { get; }
    }
}
