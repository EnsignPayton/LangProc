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
            // Data
            if (char.IsDigit(value))
                return new Token(TokenType.Integer, (int) char.GetNumericValue(value));

            // Operators
            if (value == '+')
                return new Token(TokenType.Add, value);
            if (value == '-')
                return new Token(TokenType.Sub, value);
            if (value == '*')
                return new Token(TokenType.Mult, value);
            if (value == '/')
                return new Token(TokenType.Div, value);

            // Other
            return new Token(TokenType.Unknown, value);
        }

        public override string ToString()
        {
            return $"Token({Type}, {Value}";
        }
    }
}
