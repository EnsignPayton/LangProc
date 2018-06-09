using System;
using LangProc.Core.Tree;

namespace LangProc.Core
{
    public static class Interpreter
    {
        /// <summary>
        /// Parses an expression.
        /// </summary>
        public static int ParseExpression(string text)
        {
            var tokens = Tokenizer.GetTokens(text, true);

            using (var parser = new Parser(tokens))
            {
                var tree = parser.Parse();

                return Visit(tree);
            }
        }

        public static int Visit(TreeNode<Token> node)
        {
            switch (node)
            {
                case NumberNode numberNode:
                    return Visit(numberNode);
                case BinaryOperationNode binNode:
                    return Visit(binNode);
                case UnaryOperationNode unNode:
                    return Visit(unNode);
                default:
                    throw new InvalidOperationException("Unsupported node type.");
            }
        }

        private static int Visit(NumberNode node)
        {
            return (int) node.Data.Value;
        }

        private static int Visit(BinaryOperationNode node)
        {
            switch (node.Data.Type)
            {
                case TokenType.Add:
                    return Visit(node.LeftChild) + Visit(node.RightChild);
                case TokenType.Sub:
                    return Visit(node.LeftChild) - Visit(node.RightChild);
                case TokenType.Mult:
                    return Visit(node.LeftChild) * Visit(node.RightChild);
                case TokenType.Div:
                    return Visit(node.LeftChild) / Visit(node.RightChild);
                default:
                    throw new InvalidOperationException($"Token type {node.Data.Type} not expected for binary operations.");
            }
        }

        private static int Visit(UnaryOperationNode node)
        {
            switch (node.Data.Type)
            {
                case TokenType.Add:
                    return +Visit(node.Value);
                case TokenType.Sub:
                    return -Visit(node.Value);
                default:
                    throw new InvalidOperationException($"Token type {node.Data.Type} not expected for unary operations.");
            }
        }
    }
}
