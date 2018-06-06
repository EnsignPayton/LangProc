using System;
using System.Collections.Generic;
using System.Text;

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

                tokenEnum.MoveNext();
                ValidateType(tokenEnum.Current, OperatorTypes);
                var operatorToken = tokenEnum.Current;

                tokenEnum.MoveNext();
                ValidateType(tokenEnum.Current, TokenType.Integer);
                int right = (int) tokenEnum.Current.Value;

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
            StringBuilder intBuilder = null;

            foreach (char value in text)
            {
                // Finish tokenizing a pending integer
                // Before whitespace check so nonsense like 123 456 is not interpreted as a single number
                if (intBuilder != null && !char.IsDigit(value))
                {
                    yield return new Token(TokenType.Integer, int.Parse(intBuilder.ToString()));
                    intBuilder = null;
                }

                if (char.IsWhiteSpace(value)) continue;

                if (Token.TryParseOperator(value, out var token))
                {
                    yield return token;
                }
                else if (char.IsDigit(value))
                {
                    // Append to pending integer builder
                    if (intBuilder == null)
                        intBuilder = new StringBuilder(value.ToString());
                    else
                        intBuilder.Append(value);
                }
            }

            // Finish tokenizing a pending integer
            if (intBuilder != null)
            {
                yield return new Token(TokenType.Integer, int.Parse(intBuilder.ToString()));
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
