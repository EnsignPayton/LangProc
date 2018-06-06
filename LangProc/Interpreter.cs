using System;
using System.Collections.Generic;

namespace LangProc
{
    internal static class Interpreter
    {
        /// <summary>
        /// Parses an expression of the form "INTEGER PLUS INTEGER"
        /// </summary>
        public static int ParseExpression(string text)
        {
            var tokens = GetTokens(text);

            using (var tokenEnum = tokens.GetEnumerator())
            {
                tokenEnum.MoveNext();

                ValidateType(tokenEnum.Current, TokenType.Integer);

                var left =  (int) tokenEnum.Current.Value;

                tokenEnum.MoveNext();

                ValidateType(tokenEnum.Current, TokenType.Plus);

                tokenEnum.MoveNext();

                ValidateType(tokenEnum.Current, TokenType.Integer);

                var right = (int) tokenEnum.Current.Value;

                return left + right;
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
                yield return Token.Parse(value);
            }

            yield return new Token(TokenType.EndOfFile);
        }

        private static void ValidateType(Token token, TokenType expected)
        {
            if (token.Type != expected)
                throw new Exception($"Expected token type {expected} but found {token.Type}");
        }
    }
}
