﻿using System;
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
        /// <remarks>
        /// Constructed around the following context free grammar:
        ///
        /// Program             ::= PROGRAM Variable SEMI Block DOT
        ///
        /// Block               ::= Declarations CompoundStatement
        ///
        /// Declarations        ::= VAR ( Declaration SEMI )+ |
        ///                         ( PROCEDURE ID ( LPAREN ParameterList RPAREN )? SEMI Block SEMI )* |
        ///                         Empty
        ///
        /// Declaration         ::= ID ( COMMA ID )* COLON Type
        ///
        /// Type                ::= INTEGER |
        ///                         REAL
        ///
        /// ParameterList       ::= Parameters |
        ///                         Parameters SEMI ParameterList
        ///
        /// Parameters          ::= ID ( COMMA ID )* COLON Type
        ///
        /// CompoundStatement   ::= BEGIN StatementList END
        ///
        /// StatementList       ::= Statement |
        ///                         Statement SEMI StatementList
        ///
        /// Statement           ::= CompoundStatement |
        ///                         AssignmentStatement |
        ///                         Empty
        ///
        /// AssignmentStatement ::= Variable ASSIGN Expression
        ///
        /// Variable            ::= ID
        ///
        /// Expression          ::= Term ( ( PLUS | MINUS ) Term )*
        ///
        /// Term                ::= Factor ( ( MULT | INTEGER_DIV | FLOAT_DIV ) Factor )*
        ///
        /// Factor              ::= PLUS Factor |
        ///                         MINUS Factor |
        ///                         INTEGER |
        ///                         REAL |
        ///                         LPAREN Expression RPAREN |
        ///                         Variable
        ///
        /// </remarks>
        public TreeNode<Token> Parse()
        {
            var result =  ParseProgram();

            ValidateType(Current, TokenType.EndOfFile);

            return result;
        }

        #region Parsing Grammar Productions

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

        private IEnumerable<TreeNode<Token>> ParseDeclarations()
        {
            var results = new List<TreeNode<Token>>();

            while (true)
            {
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
                else if (Current.Type == TokenType.Procedure)
                {
                    Eat(TokenType.Procedure);
                    var token = Current;
                    Eat(TokenType.Id);

                    IEnumerable<ParameterNode> parameters;

                    if (Current.Type == TokenType.ParenOpen)
                    {
                        Eat(TokenType.ParenOpen);
                        parameters = ParseParameterList();
                        Eat(TokenType.ParenClose);
                    }
                    else
                    {
                        parameters = Enumerable.Empty<ParameterNode>();
                    }

                    Eat(TokenType.Semi);
                    var blockNode = ParseBlock();
                    var procNode = new ProcedureNode(token, parameters, blockNode);
                    results.Add(procNode);
                }
                else break;
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

        private IEnumerable<ParameterNode> ParseParameterList()
        {
            if (Current.Type != TokenType.Id)
                return Enumerable.Empty<ParameterNode>();

            var paramNodes = ParseParameters().ToList();

            while (Current.Type == TokenType.Semi)
            {
                Eat(TokenType.Semi);
                var moreNodes = ParseParameters().ToList();
                paramNodes.AddRange(moreNodes);
            }

            return paramNodes;
        }

        private IEnumerable<ParameterNode> ParseParameters()
        {
            var paramTokens = new List<Token> {Current};
            Eat(TokenType.Id);

            while (Current.Type == TokenType.Comma)
            {
                Eat(TokenType.Comma);
                paramTokens.Add(Current);
                Eat(TokenType.Id);
            }

            Eat(TokenType.Colon);
            var type = ParseType();

            return paramTokens.Select(p => new VariableNode(p))
                .Select(v => new ParameterNode(v, type));
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

        #endregion

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
