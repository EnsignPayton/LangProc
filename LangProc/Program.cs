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
