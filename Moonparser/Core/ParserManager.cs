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
        static public async void Run()
        {
            List<Article> stopgameNews = new List<Article>();
            List<Article> habraNews = new List<Article>();


            HabraParser habraParser = new HabraParser();


            StopgameParser stopgameParser = new StopgameParser();

            stopgameParser.ParseAsync(stopgameNews);
            habraParser.ParseAsync(habraNews);

            await Task.Delay(20000);

            Console.WriteLine("Задержка окончена");

            using (AppContext context = new AppContext())
            {

                //context.Articles.AddRange(stopgameNews);
                context.Articles.AddRange(habraNews);

                context.SaveChanges();

            }

            //news = await stopgameParser.Parse();

            //news = await habraParser.Parse();
            //news.AddRange(await stopgameParser.Parse());

            Console.ReadKey();
            Console.ReadKey();

        }


    }
}
