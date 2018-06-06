namespace LangProc
{
    internal class Token
    {
        public Token(TokenType type, object value)
        {
            Type = type;
            Value = value;
        }

        public TokenType Type { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return $"Token({Type}, {Value}";
        }
    }
}
