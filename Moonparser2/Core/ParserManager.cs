//Класс отвечает за переодичность парсинга новостных сайтов
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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

            List<Parser> parsers = new List<Parser>();

            //parsers.Add(new HabraParser());
            //parsers.Add(new StopgameParser());
            //parsers.Add(new OnlinerParser());
            //parsers.Add(new rbcParser());
            //parsers.Add(new TutbyParser());
            //parsers.Add(new RiaParser());
            //parsers.Add(new ElementyParser());
            //parsers.Add(new MkParser());
            //parsers.Add(new KpParser());
            //parsers.Add(new Pda4Parser());
            //parsers.Add(new InvestingParser());
            //parsers.Add(new IgromaniaParser());
            //parsers.Add(new NakedScienceParser());
            //parsers.Add(new InSpaceParser());
            parsers.Add(new HiNewsParser());


            List<Article>[] articleLists = new List<Article>[parsers.Count];

            for (int i = 0; i < articleLists.Length; i++)
            {
                articleLists[i] = new List<Article>();
            }

            Task[] tasksForParsers = new Task[parsers.Count];

            for (int i = 0; i < articleLists.Length;++i)
            {
                tasksForParsers[i] = Task.Run(() => parsers[i].ParseAsync(articleLists[i]));
                
                //Грязный хак.Иначе цикл for игнорирует ограничения и выходит за пределы массива
                Thread.Sleep(100);
            }

            await Task.WhenAll(tasksForParsers);

            foreach (var art in articleLists)
            { 
                ParsedArticles.AddRange(art);
            }
        }

        /// <summary>
        /// Осуществляет парсинг статей из всех источников и сохраняет их в базу данных, предварительно проверив на наличие статьи в базе данных
        /// </summary>
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

        /// <summary>
        /// Осуществляет парсинг уже имеющихся в БД статей и обновляет данные по ним
        /// </summary>
        static public async Task Update()
        {
            List<Parser> parsers = new List<Parser>();

            parsers.Add(new HabraParser());
            parsers.Add(new StopgameParser());
            parsers.Add(new OnlinerParser());
            parsers.Add(new rbcParser());
            parsers.Add(new TutbyParser());
            parsers.Add(new RiaParser());
            parsers.Add(new ElementyParser());
            parsers.Add(new MkParser());
            parsers.Add(new KpParser());
            parsers.Add(new Pda4Parser());
            parsers.Add(new InvestingParser());
            parsers.Add(new IgromaniaParser());
            parsers.Add(new NakedScienceParser());

            List<Article>[] articleLists = new List<Article>[parsers.Count];

            for (int i = 0; i < articleLists.Length; i++)
            {
                articleLists[i] = new List<Article>();
            }

            using (AppContext context = new AppContext())
            {
                StorageArticles = context.Articles.ToList();
                context.Sources.Load();

                for (int i = 0; i < parsers.Count; i++)
                {
                    articleLists[i] = StorageArticles.Where(a => a.Source.Name == parsers[i].sourceName).ToList();
                }

                Task[] tasksForParsers = new Task[parsers.Count];

                for (int i = 0; i < articleLists.Length; ++i)
                {
                    tasksForParsers[i] = Task.Run(() => parsers[i].UpdateAsync(articleLists[i]));

                    //Грязный хак.Иначе цикл for игнорирует ограничения и выходит за пределы массива
                    Thread.Sleep(100);
                }

                await Task.WhenAll(tasksForParsers);

                context.SaveChanges();
            }
        }
    }
}
