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
        /// Parses an expression.
        /// </summary>
        public static int ParseExpression(string text)
        {
            var tokens = GetTokens(text);
            var tokenQueue = new Queue<Token>();

            using (var tokenEnum = tokens.GetEnumerator())
            {
                // First should be an integer type
                tokenEnum.MoveNext();
                ValidateType(tokenEnum.Current, TokenType.Integer);
                tokenQueue.Enqueue(tokenEnum.Current);

                // Now, we can alternate operators and integers
                tokenEnum.MoveNext();
                while (tokenEnum.Current.Type != TokenType.EndOfFile)
                {
                    ValidateType(tokenEnum.Current, OperatorTypes);
                    tokenQueue.Enqueue(tokenEnum.Current);

                    tokenEnum.MoveNext();
                    ValidateType(tokenEnum.Current, TokenType.Integer);
                    tokenQueue.Enqueue(tokenEnum.Current);

                    tokenEnum.MoveNext();
                }

                // Error on unknowns
                ValidateType(tokenEnum.Current, TokenType.EndOfFile);
            }

            int result = (int) tokenQueue.Dequeue().Value;
            while (tokenQueue.Count != 0)
            {
                TokenType opType = tokenQueue.Dequeue().Type;
                int rhs = (int) tokenQueue.Dequeue().Value;

                switch (opType)
                {
                    case TokenType.Add:
                        result += rhs;
                        break;
                    case TokenType.Sub:
                        result -= rhs;
                        break;
                    case TokenType.Mult:
                        result *= rhs;
                        break;
                    case TokenType.Div:
                        result /= rhs;
                        break;
                }
            }

            return result;
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
                else
                {
                    yield return new Token(TokenType.Unknown, value);
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
