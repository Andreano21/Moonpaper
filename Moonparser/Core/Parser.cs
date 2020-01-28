using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace Moonparser.Core
{
    abstract class Parser
    {
        protected IHtmlDocument document = null;
        protected List<Article> articles = new List<Article>();
        protected string source = null;
        protected string startUrl = null;

        protected abstract void GetStartUrl();
        protected abstract IEnumerable<IElement> GetItems();
        protected abstract void GetTitle(Article _article, IElement _item);
        protected abstract void GetSummary(Article _article, IElement _item);
        protected abstract void GetSource(Article _article);
        protected abstract void GetUrl(Article _article, IElement _item);
        protected abstract void GetUrlSource(Article _article);
        protected abstract void GetUrlMainImg(Article _article, IElement _item);
        protected abstract void GetDateTime(Article _article);
        protected abstract void GetBody(Article _article, IHtmlDocument _document);
        protected abstract void GetViews(Article _article, IHtmlDocument _document);


        public async Task ParseAsync(List<Article> _articles)
        {
            //Определение главной страницы с которой будут загружены превью
            GetStartUrl();

            HtmlParser htmlParser = new HtmlParser();

            //Получение страницы в виде строки
            source = await HtmlLoader.LoadAsync(startUrl);
            document = await htmlParser.ParseDocumentAsync(source);

            var items = GetItems();

            int succesArt = 0;
            int totalArt = items.Count();

            foreach (var item in items)
            {
                Article article = new Article();

                try
                {
                    GetTitle(article, item);
                }
                catch
                {
                    //Console.WriteLine(DateTime.Now.ToString() + "; Ошибка при парсинге GetTitle. Источник: " + startUrl);
                }

                try
                {
                    GetSummary(article, item);
                }
                catch
                {
                    //Console.WriteLine(DateTime.Now.ToString() + "; Ошибка при парсинге GetSummary. Источник: " + startUrl);
                }

                try
                {
                    GetSource(article);
                }
                catch
                {
                    //Console.WriteLine(DateTime.Now.ToString() + "; Ошибка при парсинге GetSource. Источник: " + startUrl);
                }

                try
                {
                    GetUrl(article, item);
                }
                catch
                {
                    //Console.WriteLine(DateTime.Now.ToString() + "; Ошибка при парсинге GetUrl. Источник: " + startUrl);
                }

                try
                {
                    GetUrlSource(article);
                }
                catch
                {
                    //Console.WriteLine(DateTime.Now.ToString() + "; Ошибка при парсинге GetUrlSource. Источник: " + startUrl);
                }

                try
                {
                    GetDateTime(article);
                }
                catch
                {
                    //Console.WriteLine(DateTime.Now.ToString() + "; Ошибка при парсинге GetDateTime. Источник: " + startUrl);
                }

                try
                {
                    GetUrlMainImg(article, item);
                }
                catch
                {
                    //Console.WriteLine(DateTime.Now.ToString() + "; Ошибка при парсинге GetUrlMainImg. Источник: " + startUrl);
                }

                try
                {
                    source = await HtmlLoader.LoadAsync(article.Url);
                    document = await htmlParser.ParseDocumentAsync(source);

                    //Загрузка полной статьи
                    GetBody(article, document);
                }
                catch
                {
                    //Console.WriteLine(DateTime.Now.ToString() + "; Ошибка при парсинге GetBody. Источник: " + startUrl);
                }

                try
                {
                    GetViews(article, document);
                }
                catch
                {
                    //Console.WriteLine(DateTime.Now.ToString() + "; Ошибка при парсинге GetViews. Источник: " + startUrl);
                }

                if (article.isFull())
                {
                    _articles.Add(article);

                    succesArt++;

                    //Console.WriteLine(DateTime.Now.ToString() + "; Статья получена. Источник: " + startUrl);
                }
            }
            
            Console.WriteLine(DateTime.Now.ToString() + ": Получено статей: " + succesArt + "/" + totalArt + " из " + startUrl);
        }
    }
}
