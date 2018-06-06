using System;

namespace LangProc
{
    internal class Program
    {
        private static void Main()
        {
            while (true)
            {
                try
                {
                    var text = Console.ReadLine();
                    var interpreter = new Interpreter(text);
                    var result = interpreter.ParseExpression();
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
