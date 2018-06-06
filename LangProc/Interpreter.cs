using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangProc
{
    internal class Interpreter
    {
        private string _text;
        private int _position;
        private Token _currentToken;

        public Interpreter(string text)
        {
            _text = text;
        }

        /// <summary>
        /// Lexical Analyzer / Scanner / Tokenizer
        /// </summary>
        public Token GetNextToken()
        {
            string text = _text;

            // No more input left
            if (_position > text.Length - 1)
                return new Token(TokenType.EndOfFile, null);

            char currentChar = text[_position];

            if (char.IsDigit(currentChar))
            {
                _position++;
                return new Token(TokenType.Integer, (int) char.GetNumericValue(currentChar));
            }

            if (currentChar == '+')
            {
                _position++;
                return new Token(TokenType.Plus, currentChar);
            }

            throw new Exception($"Error parsing input: '{currentChar}' not recognized as a valid token.");
        }

        /// <summary>
        /// Consumes and validates the next token
        /// </summary>
        public void Eat(TokenType type)
        {
            if (_currentToken.Type == type)
                _currentToken = GetNextToken();
            else
                throw new Exception($"Expected token of type {type} but found token of type {_currentToken.Type}.");
        }

        /// <summary>
        /// Parses an expression of the form "INTEGER PLUS INTEGER"
        /// </summary>
        public int ParseExpression()
        {
            _currentToken = GetNextToken();

            var left = _currentToken;
            Eat(TokenType.Integer);

            Eat(TokenType.Plus);

            var right = _currentToken;
            Eat(TokenType.Integer);

            // We ignore the rest. To error instead, Eat an EOF here.

            return (int)left.Value + (int)right.Value;
        }
    }
}
