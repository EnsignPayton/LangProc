using System;
using LangProc.Core;
using LangProc.Core.Tree;
using NUnit.Framework;

namespace LangProc.Test
{
    public class ParserTests
    {
        #region Order Of Operations

        [Test]
        public void OrderOfOperations()
        {
            var tokens = new[]
            {
                new Token(TokenType.Integer, 1),
                new Token(TokenType.Add),
                new Token(TokenType.Integer, 2),
                new Token(TokenType.Mult),
                new Token(TokenType.Integer, 3),
                new Token(TokenType.EndOfFile)
            };

            using (var parser = new Parser(tokens))
            {
                var result = parser.Parse();

                Assert.That(result.Data.Type, Is.EqualTo(TokenType.Add));
            }
        }

        [Test]
        public void OrderOfOperationsWithParen()
        {
            var tokens = new[]
            {
                new Token(TokenType.ParenOpen),
                new Token(TokenType.Integer, 1),
                new Token(TokenType.Add),
                new Token(TokenType.Integer, 2),
                new Token(TokenType.ParenClose),
                new Token(TokenType.Mult),
                new Token(TokenType.Integer, 3),
                new Token(TokenType.EndOfFile)
            };

            using (var parser = new Parser(tokens))
            {
                var result = parser.Parse();

                Assert.That(result.Data.Type, Is.EqualTo(TokenType.Mult));
            }
        }

        #endregion

        #region Unary Operations

        [Test]
        public void UnaryPlus()
        {
            var tokens = new[]
            {
                new Token(TokenType.Add),
                new Token(TokenType.Integer, 1),
                new Token(TokenType.EndOfFile)
            };

            using (var parser = new Parser(tokens))
            {
                var result = parser.Parse();

                Assert.That(result is UnaryOperationNode);
            }
        }

        [Test]
        public void UnaryMinus()
        {
            var tokens = new[]
            {
                new Token(TokenType.Sub),
                new Token(TokenType.Integer, 1),
                new Token(TokenType.EndOfFile)
            };

            using (var parser = new Parser(tokens))
            {
                var result = parser.Parse();

                Assert.That(result is UnaryOperationNode);
            }
        }

        #endregion

        #region Error Handling

        [Test]
        public void UnknownFails()
        {
            var tokens = new[]
            {
                new Token(TokenType.Unknown),
                new Token(TokenType.Integer, 1),
                new Token(TokenType.EndOfFile)
            };

            using (var parser = new Parser(tokens))
            {
                Assert.Throws<InvalidOperationException>(() => parser.Parse());
            }
        }

        [Test]
        public void MissingEofFails()
        {
            var tokens = new[]
            {
                new Token(TokenType.Integer, 1)
            };

            using (var parser = new Parser(tokens))
            {
                Assert.Throws<InvalidOperationException>(() => parser.Parse());
            }
        }

        [Test]
        public void ParenMismatchFails()
        {
            var tokens = new[]
            {
                new Token(TokenType.Integer, 1),
                new Token(TokenType.Add),
                new Token(TokenType.ParenOpen),
                new Token(TokenType.Integer, 1),
                new Token(TokenType.EndOfFile)
            };

            using (var parser = new Parser(tokens))
            {
                Assert.Throws<InvalidOperationException>(() => parser.Parse());
            }
        }

        [Test]
        public void ParenReverseFails()
        {
            var tokens = new[]
            {
                new Token(TokenType.Integer, 1),
                new Token(TokenType.Add),
                new Token(TokenType.ParenClose),
                new Token(TokenType.Integer, 1),
                new Token(TokenType.Add),
                new Token(TokenType.Integer, 1),
                new Token(TokenType.ParenOpen),
                new Token(TokenType.EndOfFile)
            };

            using (var parser = new Parser(tokens))
            {
                Assert.Throws<InvalidOperationException>(() => parser.Parse());
            }
        }

        #endregion
    }
}
