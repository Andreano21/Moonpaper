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

            await ParserManager.Run();

            ParserManager.Push();

            Console.WriteLine("Парсинг окончен");
            Console.ReadKey();
        }
    }
}
