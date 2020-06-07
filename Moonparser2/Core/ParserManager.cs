//Класс отвечает за переодичность парсинга новостных сайтов
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonparser.NewsSources;
using Moonparser.Core;
using System.Data.Entity;

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
            List<Article> onlinerNews = new List<Article>();
            List<Article> rbcNews = new List<Article>();
            List<Article> tutbyNews = new List<Article>();


            HabraParser habraParser = new HabraParser();
            StopgameParser stopgameParser = new StopgameParser();
            OnlinerParser onlinerParser = new OnlinerParser();
            rbcParser rbcParser = new rbcParser();
            TutbyParser tutbyParser = new TutbyParser();


            //Console.WriteLine(PageSolver.GetSolvedPage("https://habr.com/ru/"));

            //Task t1 = Task.Run(() => stopgameParser.ParseAsync(stopgameNews, PageSolverType.Not));
            //Task t2 = Task.Run(() => habraParser.ParseAsync(habraNews, PageSolverType.Not));
            //Task t3 = Task.Run(() => onlinerParser.ParseAsync(onlinerNews, PageSolverType.IE));
            //Task t4 = Task.Run(() => rbcParser.ParseAsync(rbcNews, PageSolverType.CEF));
            Task t5 = Task.Run(() => tutbyParser.ParseAsync(tutbyNews, PageSolverType.Not));


            //await Task.WhenAll(new[] {t1,t2,t3,t4 });
            await Task.WhenAll(new[] { t5 });


            ParsedArticles.AddRange(stopgameNews);
            ParsedArticles.AddRange(habraNews);
            ParsedArticles.AddRange(onlinerNews);
            ParsedArticles.AddRange(rbcNews);
            ParsedArticles.AddRange(tutbyNews);


        }

        static public void Push()
        { 

            using (AppContext context = new AppContext())
            {
                StorageArticles = context.Articles.ToList();

                context.Sources.Load();
                context.Tags.Load();

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
                            //var article = context.Articles.FirstOrDefault(u => u.Url == ParsedArticles[parsCount].Url);
                            //.Where(u => u.Url == ParsedArticles[parsCount].Url)
                            //.FirstOrDefault();

                            StorageArticles[strCoutn].Views = ParsedArticles[parsCount].Views;
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

                //Проверка тегов на наличие в базе данных
                for (int a = 0; a < newArticles.Count; a++)
                {
                    for (int t = 0; t < newArticles[a].Tags.Count; t++)
                    {
                        string curenttag = newArticles[a].Tags[t].TagValue;
                        var dbtag = context.Tags.FirstOrDefault(tt => tt.TagValue == curenttag);

                        if (dbtag == null)
                        {
                            context.Tags.Add(newArticles[a].Tags[t]);
                            context.SaveChanges();
                        }
                        else
                        {
                            newArticles[a].Tags[t] = dbtag;
                        }
                    }
                }

                //Проверка источников на наличие в базе данных
                for (int a = 0; a < newArticles.Count; a++)
                {
                    string curentSource = newArticles[a].Source.Name;
                    var dbSource = context.Sources.FirstOrDefault(s => s.Name == curentSource);

                    if (dbSource == null)
                    {
                        context.Sources.Add(newArticles[a].Source);
                        context.SaveChanges();
                    }
                    else
                    {
                        newArticles[a].Source = dbSource;
                    }
                }

                context.Articles.AddRange(newArticles);
                context.SaveChanges();
            }

            //Обнуление списка
            ParsedArticles = new List<Article>();
        }

        static public async Task Update()
        {
            List<Article> stopgameNews = new List<Article>();
            List<Article> habraNews = new List<Article>();
            List<Article> bbcNews = new List<Article>();
            List<Article> onlinerNews = new List<Article>();
            List<Article> rbcNews = new List<Article>();


            HabraParser habraParser = new HabraParser();
            StopgameParser stopgameParser = new StopgameParser();
            OnlinerParser onlinerParser = new OnlinerParser();
            rbcParser rbcParser = new rbcParser();


            using (AppContext context = new AppContext())
            {
                StorageArticles = context.Articles.ToList();
                context.Sources.Load();

                stopgameNews = StorageArticles.Where(a => a.Source.Name == "stopgame.ru").ToList();
                habraNews = StorageArticles.Where(a => a.Source.Name == "habr.com").ToList();
                onlinerNews = StorageArticles.Where(a => a.Source.Name == "onliner.by").ToList();
                rbcNews = StorageArticles.Where(a => a.Source.Name == "rbc.ru").ToList();


                Task t1 = Task.Run(() => stopgameParser.UpdateAsync(stopgameNews));
                Task t2 = Task.Run(() => habraParser.UpdateAsync(habraNews));
                Task t3 = Task.Run(() => onlinerParser.UpdateAsync(onlinerNews));
                Task t4 = Task.Run(() => rbcParser.UpdateAsync(rbcNews));


                await Task.WhenAll(new[] { t1, t2, t3, t4 });

                context.SaveChanges();
            }
        }
    }
}
