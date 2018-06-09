using System;
using System.Collections.Generic;
using System.Linq;
using LangProc.Core.Tree;

namespace LangProc.Core
{
    public sealed class Parser : IDisposable
    {
        private IEnumerator<Token> _enumerator;

        public Parser(IEnumerable<Token> tokens)
        {
            _enumerator = tokens.GetEnumerator();
            _enumerator.MoveNext();
        }

        private Token Current => _enumerator.Current;

        public void Dispose()
        {
            if (_enumerator != null)
            {
                _enumerator.Dispose();
                _enumerator = null;
            }
        }

        /// <summary>
        /// Parses the token sequence into an AST.
        /// </summary>
        /// <returns>Root node of AST</returns>
        public TreeNode<Token> Parse()
        {
            var result =  ParseProgram();

            ValidateType(Current, TokenType.EndOfFile);

            return result;
        }

        private ProgramNode ParseProgram()
        {
            Eat(TokenType.Program);

            var varNode = new VariableNode(Current);
            Eat(TokenType.Id);
            Eat(TokenType.Semi);

            var blockNode = ParseBlock();

            Eat(TokenType.Dot);

            return new ProgramNode(varNode, blockNode);
        }

        private BlockNode ParseBlock()
        {
            var declarations = ParseDeclarations();
            var compound = ParseCompoundStatement();
            return new BlockNode(declarations, compound);
        }

        private IEnumerable<DeclarationNode> ParseDeclarations()
        {
            var results = new List<DeclarationNode>();

            if (Current.Type == TokenType.Var)
            {
                Eat(TokenType.Var);

                while (Current.Type == TokenType.Id)
                {
                    var declaration = ParseDeclaration();
                    results.AddRange(declaration);
                    Eat(TokenType.Semi);
                }
            }

            return results;
        }

        private IEnumerable<DeclarationNode> ParseDeclaration()
        {
            var variables = new List<VariableNode>
            {
                new VariableNode(Current)
            };

            Eat(TokenType.Id);

            while (Current.Type == TokenType.Comma)
            {
                Eat(TokenType.Comma);
                variables.Add(new VariableNode(Current));
                Eat(TokenType.Id);
            }

            Eat(TokenType.Colon);

            var type = ParseType();

            return variables.Select(v => new DeclarationNode(v, type));
        }

        private TypeNode ParseType()
        {
            var token = Current;

            Eat(TokenType.DeclInteger, TokenType.DeclReal);

            return new TypeNode(token);
        }

        private CompoundNode ParseCompoundStatement()
        {
            Eat(TokenType.Begin);

            var nodes = ParseStatementList();

            Eat(TokenType.End);

            return new CompoundNode(nodes);
        }

        private IEnumerable<TreeNode<Token>> ParseStatementList()
        {
            var results = new List<TreeNode<Token>>();

            var node = ParseStatement();

            results.Add(node);

            while (Current.Type == TokenType.Semi)
            {
                Eat(TokenType.Semi);
                results.Add(ParseStatement());
            }

            if (Current.Type == TokenType.Id)
                throw new InvalidOperationException($"Token type {TokenType.Id} was not expected.");

            return results;
        }

        private TreeNode<Token> ParseStatement()
        {
            if (Current.Type == TokenType.Begin)
                return ParseCompoundStatement();

            if (Current.Type == TokenType.Id)
                return ParseAssignmentStatement();

            return new NopNode();
        }

        private AssignmentNode ParseAssignmentStatement()
        {
            var left = new VariableNode(Current);
            Eat(TokenType.Id);

            var token = Current;
            Eat(TokenType.Assign);

            var right = ParseExpression();

            return new AssignmentNode(token, left, right);
        }

        private TreeNode<Token> ParseExpression()
        {
            var result = ParseTerm();

            var ops = new List<TokenType> {TokenType.Add, TokenType.Sub};
            while (ops.Contains(Current.Type))
            {
                var token = Current;
                Eat(TokenType.Add, TokenType.Sub);

                result = new BinaryOperationNode(token, result, ParseTerm());
            }

            return result;
        }

        private TreeNode<Token> ParseTerm()
        {
            var result = ParseFactor();

            var ops = new List<TokenType> {TokenType.Mult, TokenType.Div, TokenType.FloatDiv};
            while (ops.Contains(Current.Type))
            {
                var token = Current;
                Eat(TokenType.Mult, TokenType.Div, TokenType.FloatDiv);

                result = new BinaryOperationNode(token, result, ParseFactor());
            }

            return result;
        }

        private TreeNode<Token> ParseFactor()
        {
            var token = Current;

            switch (token.Type)
            {
                case TokenType.Integer:
                case TokenType.Real:
                    Eat(TokenType.Integer, TokenType.Real);
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
            ValidateType(Current, types);
            _enumerator.MoveNext();
        }

        private static void ValidateType(Token token, params TokenType[] expectedTypes)
        {
            if (!((IList<TokenType>)expectedTypes).Contains(token.Type))
                throw new InvalidOperationException($"Token type {token.Type} was not expected.");
        }
    }
}
