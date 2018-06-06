namespace LangProc
{
    internal class Token
    {
        public Token(TokenType type, object value = null)
        {
            Type = type;
            Value = value;
        }

        public TokenType Type { get; set; }
        public object Value { get; set; }

        public static Token Parse(char value)
        {
            if (char.IsDigit(value))
                return new Token(TokenType.Integer, (int) char.GetNumericValue(value));

            if (value == '+')
                return new Token(TokenType.Plus, value);

            return new Token(TokenType.Unknown, value);
        }

        public override string ToString()
        {
            return $"Token({Type}, {Value}";
        }
    }
}
