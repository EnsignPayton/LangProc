using System;
using System.Collections.Generic;
using System.Text;

namespace LangProc.Core
{
    public static class Tokenizer
    {
        /// <summary>
        /// Tokenize an expression string
        /// </summary>
        /// <param name="text">Expression</param>
        /// <param name="errorOnUnknown">Throw exception on unrecognized character</param>
        /// <returns>Token enumerable</returns>
        public static IEnumerable<Token> GetTokens(string text, bool errorOnUnknown = false)
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
                    if (errorOnUnknown)
                        throw new Exception($"Unrecognized token '{value}'");

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
    }
}
