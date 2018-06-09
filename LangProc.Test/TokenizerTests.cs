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

        [Test]
        public void PascalProgram()
        {
            const string input = @"PROGRAM Test;
VAR
    number      : INTEGER;
    a, b, c, x  : INTEGER;
    y           : REAL;

BEGIN {Test}
    BEGIN
        number := 2;
        a := number;
        b := 10 * a + 10 * number DIV 4;
        c := a - - b
    END;
    x := 11;
    y := 20 / 7 + 3.14;
END. {Test}
";

            var tokenTypes = Tokenizer.GetTokens(input).Select(t => t.Type).ToList();
            Assert.That(tokenTypes, Is.EquivalentTo(new[]
            {
                // PROGRAM Test;
                TokenType.Program,
                TokenType.Id,
                TokenType.Semi,

                // VAR
                TokenType.Var,

                // number      : INTEGER;
                TokenType.Id,
                TokenType.Colon,
                TokenType.DeclInteger,
                TokenType.Semi,

                // a, b, c, x  : INTEGER;
                TokenType.Id,
                TokenType.Comma,
                TokenType.Id,
                TokenType.Comma,
                TokenType.Id,
                TokenType.Comma,
                TokenType.Id,
                TokenType.Colon,
                TokenType.DeclInteger,
                TokenType.Semi,

                // y           : REAL;
                TokenType.Id,
                TokenType.Colon,
                TokenType.DeclReal,
                TokenType.Semi,

                TokenType.Begin,
                TokenType.Begin,

                // number := 2;
                TokenType.Id,
                TokenType.Assign,
                TokenType.Integer,
                TokenType.Semi,

                // a := number;
                TokenType.Id,
                TokenType.Assign,
                TokenType.Id,
                TokenType.Semi,

                // b := 10 * a + 10 * number DIV 4;
                TokenType.Id,
                TokenType.Assign,
                TokenType.Integer,
                TokenType.Mult,
                TokenType.Id,
                TokenType.Add,
                TokenType.Integer,
                TokenType.Mult,
                TokenType.Id,
                TokenType.Div,
                TokenType.Integer,
                TokenType.Semi,

                // c := a - - b
                TokenType.Id,
                TokenType.Assign,
                TokenType.Id,
                TokenType.Sub,
                TokenType.Sub,
                TokenType.Id,

                TokenType.End,
                TokenType.Semi,

                // x := 11;
                TokenType.Id,
                TokenType.Assign,
                TokenType.Integer,
                TokenType.Semi,

                // y := 20 / 7 + 3.14;
                TokenType.Id,
                TokenType.Assign,
                TokenType.Integer,
                TokenType.FloatDiv,
                TokenType.Integer,
                TokenType.Add,
                TokenType.Real,
                TokenType.Semi,

                TokenType.End,
                TokenType.Dot,
                TokenType.EndOfFile
            }));
        }
    }
}
