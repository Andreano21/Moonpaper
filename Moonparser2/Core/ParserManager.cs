﻿//Класс отвечает за переодичность парсинга новостных сайтов
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonparser.NewsSources;
using Moonparser.Core;


namespace Moonparser.Core
{
    static class ParserManager
    {
        static List<Article> StorageArticles = new List<Article>();

        static List<Article> ParsedArticles = new List<Article>();

        static public async Task Run()
        {
            List<Article> stopgameNews = new List<Article>();
            List<Article> habraNews = new List<Article>();
            List<Article> bbcNews = new List<Article>();
            List<Article> onlinerNews = new List<Article>();


            HabraParser habraParser = new HabraParser();
            StopgameParser stopgameParser = new StopgameParser();
            BBCParser bbcParser = new BBCParser();
            OnlinerParser onlinerParser = new OnlinerParser();


            //Console.WriteLine(PageSolver.GetSolvedPage("https://habr.com/ru/"));

            Task t1 = Task.Run(() => stopgameParser.ParseAsync(stopgameNews));
            Task t2 = Task.Run(() => habraParser.ParseAsync(habraNews));
            //Task t3 = Task.Run(() => bbcParser.ParseAsync(bbcNews));
            Task t4 = Task.Run(() => onlinerParser.ParseAsync(onlinerNews));


            await Task.WhenAll(new[] {t1,t2,t4});

            ParsedArticles.AddRange(stopgameNews);
            ParsedArticles.AddRange(habraNews);
            //ParsedArticles.AddRange(bbcNews);
            ParsedArticles.AddRange(onlinerNews);

        }

        static public void Push()
        { 

            using (AppContext context = new AppContext())
            {
                StorageArticles = context.Articles.ToList();

                List<Article> newArticles = new List<Article>();

                int parsCountMax = ParsedArticles.Count;
                int strCoutnMax = StorageArticles.Count;


                //Проверка статей на наличие в базе данных
                bool isNew = true;

                for (int parsCount = 0; parsCount < parsCountMax; parsCount++)
                {
                    for (int strCoutn = 0; strCoutn < strCoutnMax; strCoutn++)
                    {
                        if (String.Equals(StorageArticles[strCoutn].Url, ParsedArticles[parsCount].Url, StringComparison.Ordinal))
                        {
                            isNew = false;

                                //Обновление количества просмотров статьи
                               // var article = context.Articles
                               //     .Where(u => u.Url == ParsedArticles[parsCount].Url)
                               //     .FirstOrDefault();

                               //article.Views = ParsedArticles[parsCount].Views;

                           break;
                        }
                    }

                    if (isNew)
                    {
                        newArticles.Add(ParsedArticles[parsCount]);
                    }

                    isNew = true;
                }

                context.Articles.AddRange(newArticles);
                context.SaveChanges();
            }
        }
    }
}