using System;
using System.Collections.Generic;

namespace LangProc
{
    internal class Parser : IDisposable
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
            //       { Add, Sub, Mult, Div, Integer },
            //       Expression, P )
            //
            // Expression -> Term ( ( Add | Sub ) Term )*
            // Term -> Factor ( ( Mult | Div ) Factor )*
            // Factor -> Integer

            return ParseExpression();
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

        // Factor -> Integer
        private int ParseFactor()
        {
            var token = _enumerator.Current;
            ValidateType(token, TokenType.Integer);
            _enumerator.MoveNext();
            return (int) token.Value;
        }

        private static void ValidateType(Token token, params TokenType[] expectedTypes)
        {
            if (!((IList<TokenType>)expectedTypes).Contains(token.Type))
                throw new Exception($"Token type {token.Type} was not expected.");
        }
    }
}
