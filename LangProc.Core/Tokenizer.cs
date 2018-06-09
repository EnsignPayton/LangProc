using System;
using System.Collections.Generic;
using System.Text;

namespace LangProc.Core
{
    public static class Tokenizer
    {
        private static readonly Dictionary<char, TokenType> SingleCharTypes = new Dictionary<char, TokenType>
        {
            {'+', TokenType.Add},
            {'-', TokenType.Sub},
            {'*', TokenType.Mult},
            //{'/', TokenType.Div}, Handled by "div" keyword
            {'(', TokenType.ParenOpen},
            {')', TokenType.ParenClose},
            {';', TokenType.Semi},
            {'.', TokenType.Dot},
        };

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

                if (wordBuilder != null && !char.IsLetter(value) && value != '_')
                {
                    yield return GetWordToken(wordBuilder.ToString());
                    wordBuilder = null;
                }

                if (char.IsWhiteSpace(value)) continue;

                if (TryGetSingleCharToken(value, out var token))
                {
                    yield return token;
                }
                else if (value == ':' && nextValue == '=')
                {
                    ++i;
                    yield return new Token(TokenType.Assign);
                }
                else if (char.IsDigit(value))
                {
                    // Append to pending integer builder
                    if (intBuilder == null)
                        intBuilder = new StringBuilder(value.ToString());
                    else
                        intBuilder.Append(value);
                }
                else if (char.IsLetter(value) || value == '_')
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

        private static bool TryGetSingleCharToken(char value, out Token token)
        {
            try
            {
                var type = SingleCharTypes[value];
                token = new Token(type, value);
                return true;
            }
            catch (KeyNotFoundException)
            {
                token = null;
                return false;
            }
        }

        private static Token GetWordToken(string word)
        {
            switch (word.ToUpper())
            {
                case "BEGIN":
                    return new Token(TokenType.Begin, word);
                case "END":
                    return new Token(TokenType.End, word);
                case "DIV":
                    return new Token(TokenType.Div, word);
                default:
                    return new Token(TokenType.Id, word);
            }
        }
    }
}
