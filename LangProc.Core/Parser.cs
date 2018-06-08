using System;
using System.Collections.Generic;

namespace LangProc.Core
{
    public class Parser : IDisposable
    {
        private IEnumerator<Token> _enumerator;

        public Parser(IEnumerable<Token> tokens)
        {
            _enumerator = tokens.GetEnumerator();
            _enumerator.MoveNext();
        }

        public void Dispose()
        {
            if (_enumerator != null)
            {
                _enumerator.Dispose();
                _enumerator = null;
            }
        }

        public int Parse()
        {
            // Grammar directly translates to parser
            // Variables -> Functions
            // Terminals -> Tokens
            //
            // G = ( { Expression, Term, Factor },
            //       { Add, Sub, Mult, Div, Integer, ParenOpen, ParenClose, EndOfFile },
            //       Expression, P )
            //
            // Expression -> Term ( ( Add | Sub ) Term )* EndOfFile
            // Term -> Factor ( ( Mult | Div ) Factor )*
            // Factor -> Integer | ParenOpen Expression ParenClose

            int result =  ParseExpression();

            ValidateType(_enumerator.Current, TokenType.EndOfFile);

            return result;
        }

        // Expression -> Term ( ( Add | Sub ) Term )*
        private int ParseExpression()
        {
            int result = ParseTerm();

            var ops = new List<TokenType> {TokenType.Add, TokenType.Sub};
            while (ops.Contains(_enumerator.Current.Type))
            {
                var token = _enumerator.Current;
                _enumerator.MoveNext();

                if (token.Type == TokenType.Add)
                    result += ParseTerm();
                else if (token.Type == TokenType.Sub)
                    result -= ParseTerm();
            }

            return result;
        }

        // Term -> Factor ( ( Mult | Div ) Factor )*
        private int ParseTerm()
        {
            int result = ParseFactor();

            var ops = new List<TokenType> {TokenType.Mult, TokenType.Div};
            while (ops.Contains(_enumerator.Current.Type))
            {
                var token = _enumerator.Current;
                _enumerator.MoveNext();

                if (token.Type == TokenType.Mult)
                    result *= ParseFactor();
                else if (token.Type == TokenType.Div)
                    result /= ParseFactor();
            }

            return result;
        }

        // Factor -> Integer | ParenOpen Expression ParenClose
        private int ParseFactor()
        {
            if (_enumerator.Current.Type == TokenType.Integer)
            {
                var token = _enumerator.Current;
                _enumerator.MoveNext();
                return (int)token.Value;
            }
            else
            {
                ValidateType(_enumerator.Current, TokenType.ParenOpen);
                _enumerator.MoveNext();

                var result = ParseExpression();

                ValidateType(_enumerator.Current, TokenType.ParenClose);
                _enumerator.MoveNext();

                return result;
            }
        }

        private static void ValidateType(Token token, params TokenType[] expectedTypes)
        {
            if (!((IList<TokenType>)expectedTypes).Contains(token.Type))
                throw new InvalidOperationException($"Token type {token.Type} was not expected.");
        }
    }
}
