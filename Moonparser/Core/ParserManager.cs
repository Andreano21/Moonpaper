//Класс отвечает за переодичность парсинга новостных сайтов
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonparser.NewsSources;


namespace Moonparser.Core
{
    static class ParserManager
    {
        static public void Run()
        {
            List<Article> stopgameNews = new List<Article>();
            List<Article> habraNews = new List<Article>();


            HabraParser habraParser = new HabraParser();


            StopgameParser stopgameParser = new StopgameParser();

            stopgameParser.Parse2(stopgameNews);
            habraParser.Parse2(habraNews);


            //news = await stopgameParser.Parse();

            //news = await habraParser.Parse();
            //news.AddRange(await stopgameParser.Parse());

            Console.ReadKey();
            Console.ReadKey();

        }


    }
}
