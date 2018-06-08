using System;

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

                return Crawl(tree);
            }
        }

        // Evaluate AST from parser
        public static int Crawl(TreeNode<Token> node)
        {
            switch (node.Data.Type)
            {
                case TokenType.Integer:
                    return (int) node.Data.Value;

                case TokenType.Add:
                    return Crawl(node.LeftChild) + Crawl(node.RightChild);
                case TokenType.Sub:
                    return Crawl(node.LeftChild) - Crawl(node.RightChild);
                case TokenType.Mult:
                    return Crawl(node.LeftChild) * Crawl(node.RightChild);
                case TokenType.Div:
                    return Crawl(node.LeftChild) / Crawl(node.RightChild);

                default:
                    throw new InvalidOperationException($"Token type {node.Data.Type} was not expected.");
            }
        }
    }
}
