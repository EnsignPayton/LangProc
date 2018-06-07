using LangProc.Core;
using NUnit.Framework;

namespace LangProc.Test
{
    public class ParserTests
    {
        [Test]
        public void Parser_Basic()
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
    }
}
