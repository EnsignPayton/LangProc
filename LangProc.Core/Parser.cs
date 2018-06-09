using System;
using System.Collections.Generic;
using LangProc.Core.Tree;

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
            var result =  ParseProgram();

            ValidateType(_enumerator.Current, TokenType.EndOfFile);

            return result;
        }

        // Program -> CompoundStatement Dot
        private TreeNode<Token> ParseProgram()
        {
            var node = ParseCompoundStatement();

            Eat(TokenType.Dot);

            return node;
        }

        // CompoundStatement -> Begin StatementList End
        private TreeNode<Token> ParseCompoundStatement()
        {
            Eat(TokenType.Begin);

            var nodes = ParseStatementList();

            Eat(TokenType.End);

            var root = new CompoundNode(null, null);

            foreach (var node in nodes)
                root.Children.Add(node);

            return root;
        }

        // StatementList -> Statement | Statement Semi StatementList
        private List<TreeNode<Token>> ParseStatementList()
        {
            var results = new List<TreeNode<Token>>();

            var node = ParseStatement();

            results.Add(node);

            while (_enumerator.Current.Type == TokenType.Semi)
            {
                Eat(TokenType.Semi);
                results.Add(ParseStatement());
            }

            if (_enumerator.Current.Type == TokenType.Id)
                throw new InvalidOperationException($"Token type {TokenType.Id} was not expected.");

            return results;
        }

        // Statement -> CompoundStatement | AssignmentStatement | Nop
        private TreeNode<Token> ParseStatement()
        {
            if (_enumerator.Current.Type == TokenType.Begin)
                return ParseCompoundStatement();

            if (_enumerator.Current.Type == TokenType.Id)
                return ParseAssignmentStatement();

            return new NopNode(null);
        }

        // AssignmentStatement -> Variable Assign Expression
        private TreeNode<Token> ParseAssignmentStatement()
        {
            var left = new VariableNode(_enumerator.Current);
            Eat(TokenType.Id);

            var token = _enumerator.Current;
            Eat(TokenType.Assign);

            var right = ParseExpression();

            return new AssignmentNode(token, left, right);
        }

        // Expression -> Term ( ( Add | Sub ) Term )*
        private TreeNode<Token> ParseExpression()
        {
            var result = ParseTerm();

            var ops = new List<TokenType> {TokenType.Add, TokenType.Sub};
            while (ops.Contains(_enumerator.Current.Type))
            {
                var token = _enumerator.Current;
                Eat(TokenType.Add, TokenType.Sub);

                result = new BinaryOperationNode(token, result, ParseTerm());
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
                Eat(TokenType.Mult, TokenType.Div);

                result = new BinaryOperationNode(token, result, ParseFactor());
            }

            return result;
        }

        // Factor -> Integer | ParenOpen Expression ParenClose
        private TreeNode<Token> ParseFactor()
        {
            var token = _enumerator.Current;

            switch (_enumerator.Current.Type)
            {
                case TokenType.Integer:
                    Eat(TokenType.Integer);
                    return new NumberNode(token);

                case TokenType.Add:
                case TokenType.Sub:
                    Eat(TokenType.Add, TokenType.Sub);
                    return new UnaryOperationNode(token, ParseFactor());

                case TokenType.ParenOpen:
                    Eat(TokenType.ParenOpen);
                    var result = ParseExpression();
                    Eat(TokenType.ParenClose);
                    return result;

                default:
                    Eat(TokenType.Id);
                    return new VariableNode(token);
            }
        }

        /// <summary>
        /// Consumes a token of a specific type
        /// </summary>
        /// <param name="types">Token Types</param>
        /// <exception cref="InvalidOperationException"/>
        private void Eat(params TokenType[] types)
        {
            ValidateType(_enumerator.Current, types);
            _enumerator.MoveNext();
        }

        private static void ValidateType(Token token, params TokenType[] expectedTypes)
        {
            if (!((IList<TokenType>)expectedTypes).Contains(token.Type))
                throw new InvalidOperationException($"Token type {token.Type} was not expected.");
        }
    }
}
