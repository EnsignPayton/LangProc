namespace LangProc
{
    internal static class Interpreter
    {
        /// <summary>
        /// Parses an expression.
        /// </summary>
        public static int ParseExpression(string text)
        {
            var tokens = Tokenizer.GetTokens(text, true);

            using (var parser = new Parser(tokens))
            {
                var result = parser.Parse();

                return result;
            }
        }
    }
}
