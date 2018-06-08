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

        public TreeNode<Token> Parse()
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
        private TreeNode<Token> ParseExpression()
        {
            var result = ParseTerm();

            var ops = new List<TokenType> {TokenType.Add, TokenType.Sub};
            while (ops.Contains(_enumerator.Current.Type))
            {
                var token = _enumerator.Current;
                _enumerator.MoveNext();

                result = new TreeNode<Token>(token, result, ParseTerm());
            }

            return result;
        }

        // Term -> Factor ( ( Mult | Div ) Factor )*
        private TreeNode<Token> ParseTerm()
        {
            var result = ParseFactor();

            var ops = new List<TokenType> {TokenType.Mult, TokenType.Div};
            while (ops.Contains(_enumerator.Current.Type))
            {
                var token = _enumerator.Current;
                _enumerator.MoveNext();

                result = new TreeNode<Token>(token, result, ParseFactor());
            }

            return result;
        }

        // Factor -> Integer | ParenOpen Expression ParenClose
        private TreeNode<Token> ParseFactor()
        {
            if (_enumerator.Current.Type == TokenType.Integer)
            {
                var token = _enumerator.Current;
                _enumerator.MoveNext();
                return new TreeNode<Token>(token);
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
