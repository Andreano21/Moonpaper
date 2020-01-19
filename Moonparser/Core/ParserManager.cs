﻿//Класс отвечает за переодичность парсинга новостных сайтов
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
        static List<Article> StorageArticles = new List<Article>();

        static List<Article> ParsedArticles = new List<Article>();

        static public void Initial()
        {
            using (AppContext context = new AppContext())
            {
                StorageArticles = context.Articles.ToList();
            }
        }

        static public async Task Run()
        {
            List<Article> stopgameNews = new List<Article>();
            List<Article> habraNews = new List<Article>();


            HabraParser habraParser = new HabraParser();
            StopgameParser stopgameParser = new StopgameParser();

            Task t1 = Task.Run(() => stopgameParser.ParseAsync(stopgameNews));
            Task t2 = Task.Run(() => habraParser.ParseAsync(habraNews));


            await Task.WhenAll(new[] {t1, t2});

            ParsedArticles.AddRange(stopgameNews);
            ParsedArticles.AddRange(habraNews);

        }

        static public void Push()
        { 
            List<Article> newArticles = new List<Article>();

            int parsCountMax = ParsedArticles.Count;
            int strCoutnMax = StorageArticles.Count;

            //Проверка новых статей на наличие в базе данных
            bool isNew = true;
            for (int parsCount = 0; parsCount < parsCountMax; parsCount++)
            {
                for (int strCoutn = 0; strCoutn < strCoutnMax; strCoutn++)
                {
                    if (String.Equals(StorageArticles[strCoutn].Url, ParsedArticles[parsCount].Url, StringComparison.Ordinal))
                    {
                        isNew = false;
                        break;
                    }
                }

                if (isNew)
                {
                    newArticles.Add(ParsedArticles[parsCount]);
                }

                isNew = true;
            }

            using (AppContext context = new AppContext())
            {
                //context.Articles.AddRange(stopgameNews);
                context.Articles.AddRange(newArticles);
                context.SaveChanges();
            }
        }
    }
}
