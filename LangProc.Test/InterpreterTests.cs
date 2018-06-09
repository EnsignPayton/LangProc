using LangProc.Core;
using LangProc.Core.Tree;
using NUnit.Framework;

namespace LangProc.Test
{
    public class InterpreterTests
    {
        #region Basic Operations

        [Test]
        public void Adds()
        {
            // 1 + 1
            var tree = BuildBasicTree(TokenType.Add);
            int result = Interpreter.Visit(tree);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void Subtracts()
        {
            // 1 - 1
            var tree = BuildBasicTree(TokenType.Sub);
            int result = Interpreter.Visit(tree);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Multiplies()
        {
            // 1 * 1
            var tree = BuildBasicTree(TokenType.Mult);
            int result = Interpreter.Visit(tree);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void Divides()
        {
            // 1 / 1
            var tree = BuildBasicTree(TokenType.Div);
            int result = Interpreter.Visit(tree);

            Assert.That(result, Is.EqualTo(1));
        }

        #endregion

        #region Unary Operations

        [Test]
        public void UnaryNegation()
        {
            // -1
            var tree = new UnaryOperationNode(new Token(TokenType.Sub),
                new NumberNode(new Token(TokenType.Integer, 1)));
            int result = Interpreter.Visit(tree);

            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void UnaryDoublePositive()
        {
            // +(+1)
            var tree = BuildUnaryTree(TokenType.Add);
            int result = Interpreter.Visit(tree);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void UnaryDoubleNegative()
        {
            // -(-1)
            var tree = BuildUnaryTree(TokenType.Sub);
            int result = Interpreter.Visit(tree);

            Assert.That(result, Is.EqualTo(1));
        }

        #endregion

        #region Helpers

        private static TreeNode<Token> BuildBasicTree(TokenType operation)
        {
            var tree = new BinaryOperationNode(new Token(operation),
                new NumberNode(new Token(TokenType.Integer, 1)),
                new NumberNode(new Token(TokenType.Integer, 1)));

            return tree;
        }

        private static TreeNode<Token> BuildUnaryTree(TokenType operation)
        {
            var tree = new UnaryOperationNode(new Token(operation),
                new UnaryOperationNode(new Token(operation),
                new NumberNode(new Token(TokenType.Integer, 1))));

            return tree;
        }

        #endregion
    }
}
