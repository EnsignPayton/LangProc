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
            var tokenTypes = Tokenizer.GetTokens("1+2").Select(t => t.Type).ToList();

            Assert.That(tokenTypes, Is.EquivalentTo(new[]
            {
                TokenType.Integer,
                TokenType.Add,
                TokenType.Integer,
                TokenType.EndOfFile
            }));
        }

        [Test]
        public void IgnoresWhitespace()
        {
            var tokenTypes = Tokenizer.GetTokens("1 2\n3\t4\n\t 5").Select(t => t.Type).ToList();

            Assert.That(tokenTypes, Is.EquivalentTo(new[]
            {
                TokenType.Integer,
                TokenType.Integer,
                TokenType.Integer,
                TokenType.Integer,
                TokenType.Integer,
                TokenType.EndOfFile
            }));
        }

        [Test]
        public void MultiDigitIntegers()
        {
            var tokens = Tokenizer.GetTokens("12345").ToList();
            Assert.That(tokens[0].Value, Is.EqualTo(12345));
            Assert.That(tokens[1].Type, Is.EqualTo(TokenType.EndOfFile));
        }

        [Test]
        public void HandlesParentheses()
        {
            var tokenTypes = Tokenizer.GetTokens("1(2+3)*4").Select(t => t.Type).ToList();
            Assert.That(tokenTypes, Is.EquivalentTo(new[]
            {
                TokenType.Integer,
                TokenType.ParenOpen,
                TokenType.Integer,
                TokenType.Add,
                TokenType.Integer,
                TokenType.ParenClose,
                TokenType.Mult,
                TokenType.Integer,
                TokenType.EndOfFile
            }));
        }

        [Test]
        public void PascalStatement()
        {
            var tokenTypes = Tokenizer.GetTokens("BEGIN a := 2; END.").Select(t => t.Type).ToList();
            Assert.That(tokenTypes, Is.EquivalentTo(new[]
            {
                TokenType.Begin,
                TokenType.Id,
                TokenType.Assign,
                TokenType.Integer,
                TokenType.Semi,
                TokenType.End,
                TokenType.Dot,
                TokenType.EndOfFile
            }));
        }
    }
}
