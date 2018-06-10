using System;
using System.Collections.Generic;
using System.Text;

namespace LangProc.Core
{
    public static class Tokenizer
    {
        private static readonly IDictionary<char, TokenType> SingleCharTypes = new Dictionary<char, TokenType>
        {
            {'+', TokenType.Add},
            {'-', TokenType.Sub},
            {'*', TokenType.Mult},
            {'/', TokenType.FloatDiv},
            {'(', TokenType.ParenOpen},
            {')', TokenType.ParenClose},
            {';', TokenType.Semi},
            {'.', TokenType.Dot},
            {':', TokenType.Colon},
            {',', TokenType.Comma}
        };

        private static readonly IDictionary<string, TokenType> ReservedWordTypes = new Dictionary<string, TokenType>
        {
            {"PROGRAM", TokenType.Program},
            {"VAR", TokenType.Var},
            {"DIV", TokenType.Div},
            {"INTEGER", TokenType.DeclInteger},
            {"REAL", TokenType.DeclReal},
            {"BEGIN", TokenType.Begin},
            {"END", TokenType.End},
            {"PROCEDURE", TokenType.Procedure}
        };

        /// <summary>
        /// Tokenize an expression string
        /// </summary>
        /// <param name="text">Expression</param>
        /// <param name="errorOnUnknown">Throw exception on unrecognized character</param>
        /// <returns>Token enumerable</returns>
        public static IEnumerable<Token> GetTokens(string text, bool errorOnUnknown = false)
        {
            StringBuilder numBuilder = null;
            StringBuilder wordBuilder = null;

            //foreach (char value in text)
            for (int i = 0; i < text.Length; ++i)
            {
                char value = text[i];
                char? nextValue = i + 1 < text.Length ? (char?) text[i + 1] : null;

                // Skip comments is highest priority
                if (value == '{')
                {
                    while (nextValue != '}' && nextValue != null)
                    {
                        ++i;
                        nextValue = i + 1 < text.Length ? (char?) text[i + 1] : null;
                    }

                    if (nextValue == null)
                        throw new InvalidOperationException("Comment was never closed");

                    ++i;
                    continue;
                }

                // Finish tokenizing a pending integer
                // Before whitespace check so nonsense like 123 456 is not interpreted as a single number
                if (numBuilder != null && !char.IsDigit(value) && value != '.')
                {
                    string number = numBuilder.ToString();
                    numBuilder = null;

                    if (number.Contains("."))
                        yield return new Token(TokenType.Real, double.Parse(number));
                    else
                        yield return new Token(TokenType.Integer, int.Parse(number));
                }

                if (wordBuilder != null && !char.IsLetter(value) && value != '_')
                {
                    yield return GetWordToken(wordBuilder.ToString());
                    wordBuilder = null;
                }

                // Skip whitespace
                if (char.IsWhiteSpace(value)) continue;

                if (numBuilder != null && value == '.')
                {
                    if (!numBuilder.ToString().Contains("."))
                        numBuilder.Append(value);
                    else throw new InvalidOperationException(
                        "Real valued number cannot contain multiple decimal places");
                }
                else if (value == ':' && nextValue == '=')
                {
                    ++i;
                    yield return new Token(TokenType.Assign);
                }
                else if (TryGetSingleCharToken(value, out var token))
                {
                    yield return token;
                }
                else if (char.IsDigit(value))
                {
                    // Append to pending integer builder
                    if (numBuilder == null)
                        numBuilder = new StringBuilder(value.ToString());
                    else
                        numBuilder.Append(value);
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
            if (numBuilder != null)
            {
                string number = numBuilder.ToString();

                if (number.Contains("."))
                    yield return new Token(TokenType.Real, double.Parse(number));
                else 
                    yield return new Token(TokenType.Integer, int.Parse(number));
            }

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
            try
            {
                var type = ReservedWordTypes[word.ToUpper()];
                return new Token(type, word);
            }
            catch (KeyNotFoundException)
            {
                return new Token(TokenType.Id, word);
            }
        }
    }
}
