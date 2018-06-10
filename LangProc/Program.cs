using System;
using System.IO;
using LangProc.Core;
using LangProc.Core.Tree;

namespace LangProc
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                string text = File.ReadAllText(args[0]);
                var tokens = Tokenizer.GetTokens(text);

                TreeNode<Token> tree;
                using (var parser = new Parser(tokens))
                {
                    tree = parser.Parse();
                }

                var analyzer = new SemanticAnalyzer();
                analyzer.Build(tree);

                var interpreter = new Interpreter();
                interpreter.Interpret(tree);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

#if DEBUG
            Console.Read();
#endif
        }
    }
}
