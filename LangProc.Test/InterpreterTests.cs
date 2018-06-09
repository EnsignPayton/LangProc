using LangProc.Core;
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
            int result = Interpreter.Crawl(tree);

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void Subtracts()
        {
            // 1 - 1
            var tree = BuildBasicTree(TokenType.Sub);
            int result = Interpreter.Crawl(tree);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Multiplies()
        {
            // 1 * 1
            var tree = BuildBasicTree(TokenType.Mult);
            int result = Interpreter.Crawl(tree);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void Divides()
        {
            // 1 / 1
            var tree = BuildBasicTree(TokenType.Div);
            int result = Interpreter.Crawl(tree);

            Assert.That(result, Is.EqualTo(1));
        }

        #endregion

        #region Unary Operations

        [Test]
        public void UnaryNegation()
        {
            // -1
            var tree = new TokenNode(new Token(TokenType.Sub),
                new TokenNode(new Token(TokenType.Integer, 1))) {IsUnary = true};
            int result = Interpreter.Crawl(tree);

            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void UnaryDoublePositive()
        {
            // +(+1)
            var tree = BuildUnaryTree(TokenType.Add);
            int result = Interpreter.Crawl(tree);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void UnaryDoubleNegative()
        {
            // -(-1)
            var tree = BuildUnaryTree(TokenType.Sub);
            int result = Interpreter.Crawl(tree);

            Assert.That(result, Is.EqualTo(1));
        }

        #endregion

        #region Helpers

        private static TokenNode BuildBasicTree(TokenType operation)
        {
            var tree = new TokenNode(new Token(operation),
                new TokenNode(new Token(TokenType.Integer, 1)),
                new TokenNode(new Token(TokenType.Integer, 1)));

            return tree;
        }

        private static TokenNode BuildUnaryTree(TokenType operation)
        {
            var tree = new TokenNode(new Token(operation),
                new TokenNode(new Token(operation),
                    new TokenNode(new Token(TokenType.Integer, 1))
                    ) {IsUnary = true}
                ) {IsUnary = true};

            return tree;
        }

        #endregion
    }
}
