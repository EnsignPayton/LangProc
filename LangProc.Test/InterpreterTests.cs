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

        #region Helpers

        private static TreeNode<Token> BuildBasicTree(TokenType operation)
        {
            var tree = new TreeNode<Token>(new Token(operation),
                new TreeNode<Token>(new Token(TokenType.Integer, 1)),
                new TreeNode<Token>(new Token(TokenType.Integer, 1)));

            return tree;
        }

        #endregion
    }
}
