using System;
using System.Linq;
using LangProc.Core;
using LangProc.Core.Tree;
using NUnit.Framework;

namespace LangProc.Test
{
    public class ParserTests
    {
        #region Pascal Basic

        [Test]
        public void EmptyProgram()
        {
            var tokens = new[]
            {
                new Token(TokenType.Program),
                new Token(TokenType.Id),
                new Token(TokenType.Semi),
                new Token(TokenType.Begin),
                new Token(TokenType.End),
                new Token(TokenType.Dot), 
                new Token(TokenType.EndOfFile)
            };

            using (var parser = new Parser(tokens))
            {
                var result = parser.Parse();

                Assert.That(result is ProgramNode);
            }
        }

        [Test]
        [TestCase("apple", 12)]
        [TestCase("BANANA", 24)]
        public void AssignsVariables(string varName, int varValue)
        {
            var tokens = new[]
            {
                new Token(TokenType.Program),
                new Token(TokenType.Id),
                new Token(TokenType.Semi),
                new Token(TokenType.Begin),
                new Token(TokenType.Id, varName),
                new Token(TokenType.Assign),
                new Token(TokenType.Integer, varValue),
                new Token(TokenType.End),
                new Token(TokenType.Dot),
                new Token(TokenType.EndOfFile)
            };

            using (var parser = new Parser(tokens))
            {
                var result = (ProgramNode) parser.Parse();
                var block = result.BlockNode;
                var compound = block.CompoundNode;
                var assign = (AssignmentNode) compound.Children.First();

                Assert.That(assign.Variable.Data.Value, Is.EqualTo(varName));
                Assert.That(assign.Value.Data.Value, Is.EqualTo(varValue));
            }
        }

        #endregion

        #region Order Of Operations

        [Test]
        [Ignore("Changed from simple expressions to Pascal")]
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
        [Ignore("Changed from simple expressions to Pascal")]
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
        [Ignore("Changed from simple expressions to Pascal")]
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
        [Ignore("Changed from simple expressions to Pascal")]
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
