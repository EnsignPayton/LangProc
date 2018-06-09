using System;
using LangProc.Core;

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
                    if (text == null) break;
                    var interpreter = new Interpreter();
                    interpreter.Interpret(text);
                    foreach (var kvp in interpreter.GlobalScope)
                    {
                        Console.WriteLine($"{kvp.Key} -> {kvp.Value}");
                    }

                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
