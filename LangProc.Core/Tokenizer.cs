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
            StringBuilder wordBuilder = null;

            //foreach (char value in text)
            for (int i = 0; i < text.Length; ++i)
            {
                char value = text[i];
                char? nextValue = i + 1 < text.Length ? (char?) text[i + 1] : null;

                // Finish tokenizing a pending integer
                // Before whitespace check so nonsense like 123 456 is not interpreted as a single number
                if (intBuilder != null && !char.IsDigit(value))
                {
                    yield return new Token(TokenType.Integer, int.Parse(intBuilder.ToString()));
                    intBuilder = null;
                }

                if (wordBuilder != null && !char.IsLetter(value))
                {
                    yield return GetWordToken(wordBuilder.ToString());
                    wordBuilder = null;
                }

                if (char.IsWhiteSpace(value)) continue;

                if (Token.TryParseOperator(value, out var token))
                {
                    yield return token;
                }
                else if (value =='(')
                {
                    yield return new Token(TokenType.ParenOpen, value);
                }
                else if (value == ')')
                {
                    yield return new Token(TokenType.ParenClose, value);
                }
                else if (value == ':' && nextValue == '=')
                {
                    ++i;
                    yield return new Token(TokenType.Assign);
                }
                else if (value == ';')
                {
                    yield return new Token(TokenType.Semi, value);
                }
                else if (value == '.')
                {
                    yield return new Token(TokenType.Dot);
                }
                else if (char.IsDigit(value))
                {
                    // Append to pending integer builder
                    if (intBuilder == null)
                        intBuilder = new StringBuilder(value.ToString());
                    else
                        intBuilder.Append(value);
                }
                else if (char.IsLetter(value))
                {
                    if (wordBuilder == null)
                        wordBuilder = new StringBuilder(value.ToString());
                    else
                        wordBuilder.Append(value);
                }
                else
                {
                    if (errorOnUnknown)
                        throw new Exception($"Unrecognized token '{value}'");

                    yield return new Token(TokenType.Unknown, value);
                }
            }

            // Finish tokenizing pending stuff
            if (intBuilder != null)
                yield return new Token(TokenType.Integer, int.Parse(intBuilder.ToString()));

            if (wordBuilder != null)
                yield return GetWordToken(wordBuilder.ToString());

            yield return new Token(TokenType.EndOfFile);
        }

        private static Token GetWordToken(string word)
        {
            if (word == "BEGIN")
                return new Token(TokenType.Begin, word);

            if (word == "END")
                return new Token(TokenType.End, word);

            return new Token(TokenType.Id, word);
        }
    }
}
