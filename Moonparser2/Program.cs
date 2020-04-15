using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

            int timerStep = 7200000;
            TimerCallback tm = new TimerCallback(StartTimer);

            while (true)
            {
                Console.Write(">");
                inputCommand = Console.ReadLine();

                switch (inputCommand)
                {
                    case "s":
                        await ParserManager.Run();
                        ParserManager.Push();
                        Console.WriteLine("Парсинг окончен");
                        break;

                    case "u":
                        await ParserManager.Update();
                        Solver.Solve();
                        break;

                    case "s t":
                        Timer timer = new Timer(tm, null, 0, timerStep);
                        break;

                    case "q":
                        return;

                    default:
                        Console.WriteLine("Команда не найдена");
                        break;
                }
            }
        }

        private static async void StartTimer(object obj)
        {
            Console.WriteLine("Парсинг и обновление по таймеру начато");


            await ParserManager.Run();
            ParserManager.Push();

            await ParserManager.Update();
            Solver.Solve();

            Console.WriteLine("Парсинг и обновление по таймеру окончено");

        }
    }
}
