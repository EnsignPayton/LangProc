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

        public TokenNode Parse()
        {
            // Grammar directly translates to parser
            // Variables -> Functions
            // Terminals -> Tokens
            //
            // G = ( { Expression, Term, Factor },
            //       { Add, Sub, Mult, Div, Integer, ParenOpen, ParenClose, EndOfFile },
            //       Expression, P )
            //
            // Expression -> Term ( ( Add | Sub ) Term )*
            // Term -> Factor ( ( Mult | Div ) Factor )*
            // Factor -> Integer | ParenOpen Expression ParenClose

            var result =  ParseExpression();

            ValidateType(_enumerator.Current, TokenType.EndOfFile);

            return result;
        }

        // Expression -> Term ( ( Add | Sub ) Term )*
        private TokenNode ParseExpression()
        {
            var result = ParseTerm();

            var ops = new List<TokenType> {TokenType.Add, TokenType.Sub};
            while (ops.Contains(_enumerator.Current.Type))
            {
                var token = _enumerator.Current;
                _enumerator.MoveNext();

                result = new TokenNode(token, result, ParseTerm());
            }

            return result;
        }

        // Term -> Factor ( ( Mult | Div ) Factor )*
        private TokenNode ParseTerm()
        {
            var result = ParseFactor();

            var ops = new List<TokenType> {TokenType.Mult, TokenType.Div};
            while (ops.Contains(_enumerator.Current.Type))
            {
                var token = _enumerator.Current;
                _enumerator.MoveNext();

                result = new TokenNode(token, result, ParseFactor());
            }

            return result;
        }

        // Factor -> Integer | ParenOpen Expression ParenClose
        private TokenNode ParseFactor()
        {
            switch (_enumerator.Current.Type)
            {
                case TokenType.Integer:
                    var token1 = _enumerator.Current;
                    _enumerator.MoveNext();
                    return new TokenNode(token1);

                case TokenType.Add:
                case TokenType.Sub:
                    var token2 = _enumerator.Current;
                    _enumerator.MoveNext();
                    return new TokenNode(token2, ParseFactor()) { IsUnary = true };

                default:
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
