using System;
using System.IO;

namespace LangProc
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
                HandleInput();
            else
                HandleFile(args[0]);

#if DEBUG
            Console.Read();
#endif
        }

        private static void HandleFile(string filePath)
        {
            try
            {
                var text = File.ReadAllText(filePath);
                var result = Interpreter.ParseExpression(text);
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void HandleInput()
        {
            while (true)
            {
                try
                {
                    var text = Console.ReadLine();
                    if (text == null) break;
                    var result = Interpreter.ParseExpression(text);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
