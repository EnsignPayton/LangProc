using System;
using System.Collections.Generic;

namespace LangProc
{
    internal static class Interpreter
    {
        private static readonly ICollection<TokenType> OperatorTypes = new List<TokenType>
        {
            TokenType.Add, TokenType.Sub, TokenType.Mult, TokenType.Div
        };

        /// <summary>
        /// Parses an expression of the form "INTEGER OPERATOR INTEGER"
        /// </summary>
        public static int ParseExpression(string text)
        {
            var tokens = GetTokens(text);

            using (var tokenEnum = tokens.GetEnumerator())
            {
                tokenEnum.MoveNext();

                ValidateType(tokenEnum.Current, TokenType.Integer);

                int left = (int) tokenEnum.Current.Value;

                // Handle additional digits
                while (true)
                {
                    tokenEnum.MoveNext();
                    if (tokenEnum.Current.Type == TokenType.Integer)
                    {
                        int temp = (int) tokenEnum.Current.Value;

                        left = (left * 10) + temp;
                    }
                    else break;
                }

                ValidateType(tokenEnum.Current, OperatorTypes);

                var operatorToken = tokenEnum.Current;

                tokenEnum.MoveNext();

                ValidateType(tokenEnum.Current, TokenType.Integer);

                int right = (int) tokenEnum.Current.Value;

                while (true)
                {
                    tokenEnum.MoveNext();
                    if (tokenEnum.Current.Type == TokenType.Integer)
                    {
                        int temp = (int) tokenEnum.Current.Value;

                        right = (right * 10) + temp;
                    }
                    else break;
                }

                if (operatorToken.Type == TokenType.Add)
                    return left + right;

                if (operatorToken.Type == TokenType.Sub)
                    return left - right;

                if (operatorToken.Type == TokenType.Mult)
                    return left * right;

                if (operatorToken.Type == TokenType.Div)
                    return left / right;

                throw new Exception("Invalid operator. This should not happen.");
            }
        }

        /// <summary>
        /// Tokenize an expression string
        /// </summary>
        /// <param name="text">Expression</param>
        /// <returns>Token enumerable</returns>
        private static IEnumerable<Token> GetTokens(string text)
        {
            foreach (var value in text)
            {
                if (char.IsWhiteSpace(value)) continue;

                if (Token.TryParseOperator(value, out var token))
                {
                    yield return token;
                }
                else if (char.IsDigit(value))
                {
                    // Python Example
                    //def integer(self):
                    //"""Return a (multidigit) integer consumed from the input."""
                    //result = ''
                    //while self.current_char is not None and self.current_char.isdigit():
                    //result += self.current_char
                    //self.advance()
                    //return int(result)
                }
            }

            yield return new Token(TokenType.EndOfFile);
        }

        private static void ValidateType(Token token, TokenType expected)
        {
            if (token.Type != expected)
                throw new Exception($"Expected token type {expected} but found {token.Type}");
        }

        private static void ValidateType(Token token, ICollection<TokenType> expectedTypes)
        {
            if (!expectedTypes.Contains(token.Type))
                throw new Exception($"Token type {token.Type} was not expected.");
        }
    }
}
