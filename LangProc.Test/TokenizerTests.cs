using System.Linq;
using LangProc.Core;
using NUnit.Framework;

namespace LangProc.Test
{
    public class TokenizerTests
    {
        [Test]
        public void Basic()
        {
            var tokens = Tokenizer.GetTokens("1+2").ToList();
            Assert.AreEqual(tokens[0].Type, TokenType.Integer);
            Assert.AreEqual(tokens[1].Type, TokenType.Add);
            Assert.AreEqual(tokens[2].Type, TokenType.Integer);
            Assert.AreEqual(tokens[3].Type, TokenType.EndOfFile);
        }

        [Test]
        public void IgnoresWhitespace()
        {
            var tokens = Tokenizer.GetTokens("1 2\n3\t4\n\t 5").ToList();
            Assert.AreEqual(tokens[0].Type, TokenType.Integer);
            Assert.AreEqual(tokens[1].Type, TokenType.Integer);
            Assert.AreEqual(tokens[2].Type, TokenType.Integer);
            Assert.AreEqual(tokens[3].Type, TokenType.Integer);
            Assert.AreEqual(tokens[4].Type, TokenType.Integer);
            Assert.AreEqual(tokens[5].Type, TokenType.EndOfFile);
        }

        [Test]
        public void MultiDigitIntegers()
        {
            var tokens = Tokenizer.GetTokens("12345").ToList();
            Assert.AreEqual(tokens[0].Value, 12345);
            Assert.AreEqual(tokens[1].Type, TokenType.EndOfFile);
        }

        [Test]
        public void HandlesParentheses()
        {
            var tokens = Tokenizer.GetTokens("1(2+3)*4").ToList();
            Assert.AreEqual(tokens[0].Type, TokenType.Integer);
            Assert.AreEqual(tokens[1].Type, TokenType.ParenOpen);
            Assert.AreEqual(tokens[2].Type, TokenType.Integer);
            Assert.AreEqual(tokens[3].Type, TokenType.Add);
            Assert.AreEqual(tokens[4].Type, TokenType.Integer);
            Assert.AreEqual(tokens[5].Type, TokenType.ParenClose);
            Assert.AreEqual(tokens[6].Type, TokenType.Mult);
            Assert.AreEqual(tokens[7].Type, TokenType.Integer);
            Assert.AreEqual(tokens[8].Type, TokenType.EndOfFile);
        }
    }
}
