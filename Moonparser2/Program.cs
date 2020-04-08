using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Moonparser.Core;

namespace Moonparser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string inputCommand;

            while (true)
            {
                Console.Write(">");
                inputCommand = Console.ReadLine();

                switch (inputCommand)
                {
                    case "p":
                        await ParserManager.Run();
                        ParserManager.Push();
                        Console.WriteLine("Парсинг окончен");
                        break;

                    case "u":
                        Solver.Solve();
                        break;

                    case "q":
                        return;

                    default:
                        Console.WriteLine("Команда не найдена");
                        break;
                }
            }



            //await ParserManager.Run();

            //ParserManager.Push();

            //Console.WriteLine("Парсинг окончен");
            //Console.ReadKey();
        }
    }
}
