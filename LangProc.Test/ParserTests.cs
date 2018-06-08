using System;
using LangProc.Core;
using NUnit.Framework;

namespace LangProc.Test
{
    public class ParserTests
    {
        #region Basic Operations

        [Test]
        public void Adds()
        {
            var tokens = new[]
            {
                new Token(TokenType.Integer, 1),
                new Token(TokenType.Add),
                new Token(TokenType.Integer, 1),
                new Token(TokenType.EndOfFile)
            };

            using (var parser = new Parser(tokens))
            {
                var result = parser.Parse();

                Assert.AreEqual(result, 2);
            }
        }

        [Test]
        public void Subtracts()
        {
            var tokens = new[]
            {
                new Token(TokenType.Integer, 1),
                new Token(TokenType.Sub),
                new Token(TokenType.Integer, 1),
                new Token(TokenType.EndOfFile)
            };

            using (var parser = new Parser(tokens))
            {
                var result = parser.Parse();

                Assert.AreEqual(result, 0);
            }
        }

        [Test]
        public void Multiplies()
        {
            var tokens = new[]
            {
                new Token(TokenType.Integer, 1),
                new Token(TokenType.Mult),
                new Token(TokenType.Integer, 1),
                new Token(TokenType.EndOfFile)
            };

            using (var parser = new Parser(tokens))
            {
                var result = parser.Parse();

                Assert.AreEqual(result, 1);
            }
        }

        [Test]
        public void Divides()
        {
            var tokens = new[]
            {
                new Token(TokenType.Integer, 1),
                new Token(TokenType.Div),
                new Token(TokenType.Integer, 1),
                new Token(TokenType.EndOfFile)
            };

            using (var parser = new Parser(tokens))
            {
                var result = parser.Parse();

                Assert.AreEqual(result, 1);
            }
        }

        #endregion

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

                Assert.AreEqual(result, 7);
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

                Assert.AreEqual(result, 9);
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
