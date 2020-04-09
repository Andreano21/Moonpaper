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
    /// <summary>
    /// Обеспечивает парсинг статей
    /// </summary>
    abstract class Parser
    {
        protected List<Article> articles = new List<Article>();
        protected string[] startUrl = null;
        protected string[] sources = null;
        protected IHtmlDocument[] documents = null;
        protected IHtmlDocument document = null;
        protected string source = null;

        /// <summary>
        /// Задает ссылки на первичные страницы
        /// </summary>
        protected abstract void GetStartUrl();

        /// <summary>
        /// Получает список html блоков содержащих укороченные статьи
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<IElement> GetItems();

        /// <summary>
        /// Получает ссылку на статью из html блока
        /// </summary>
        /// <param name="_article">Статья(объект) которой присваивается ссылка</param>
        /// <param name="reducedArticle">html блок с уменьшенной статьёй полученной со стартовой страницы</param>
        /// <returns></returns>
        protected abstract void GetUrl(Article _article, IElement reducedArticle);

        /// <summary>
        /// Получает тело статьи
        /// </summary>
        /// <param name="_article">Статья(объект) которой присваивается тело статьи</param>
        /// <param name="fullArticle">html страница с полной статьёй полученная по прямой ссылке</param>
        /// <returns></returns>
        protected abstract void GetBody(Article _article, IHtmlDocument fullArticle);

        /// <summary>
        /// Получает заглавие статьи
        /// </summary>
        /// <param name="_article">Статья(объект) которой присваивается заглавие статьи</param>
        /// <param name="reducedArticle">html блок с уменьшенной статьёй полученной со стартовой страницы</param>
        /// <param name="fullArticle">html страница с полной статьёй полученная по прямой ссылке</param>
        /// <returns></returns>
        protected abstract void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle);

        /// <summary>
        /// Получает краткое описание статьи
        /// </summary>
        /// <param name="_article">Статья(объект) которой присваивается краткое описание статьи</param>
        /// <param name="reducedArticle">html блок с уменьшенной статьёй полученной со стартовой страницы</param>
        /// <param name="fullArticle">html страница с полной статьёй полученная по прямой ссылке</param>
        /// <returns></returns>
        protected abstract void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle);

        /// <summary>
        /// Получает название ресурса с которого получена статься
        /// </summary>
        /// <param name="_article">Статья(объект) которой присваивается название источника</param>
        /// <returns></returns>
        protected abstract void GetSource(Article _article);

        /// <summary>
        /// Получает ссылку на ресурс с которого получена статься
        /// </summary>
        /// <param name="_article">Статья(объект) которой присваивается ссылка на ресурс</param>
        /// <returns></returns>
        protected abstract void GetUrlSource(Article _article);

        /// <summary>
        /// Получает ссылку на КДПВ
        /// </summary>
        /// <param name="_article">Статья(объект) которой присваивается ссылка на КДПВ</param>
        /// <param name="reducedArticle">html блок с уменьшенной статьёй полученной со стартовой страницы</param>
        /// <param name="fullArticle">html страница с полной статьёй полученная по прямой ссылке</param>
        /// <returns></returns>
        protected abstract void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle);

        /// <summary>
        /// Получает время создания статьи
        /// </summary>
        /// <param name="_article">Статья(объект) которой присваивается время</param>
        /// <returns></returns>
        protected abstract void GetDateTime(Article _article);

        /// <summary>
        /// Получает количество просмотров статьи
        /// </summary>
        /// <param name="_article">Статья(объект) которой присваивается количество просмотров</param>
        /// <param name="reducedArticle">html блок с уменьшенной статьёй полученной со стартовой страницы</param>
        /// <param name="fullArticle">html страница с полной статьёй полученная по прямой ссылке</param>
        /// <returns></returns>
        protected abstract void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle);

        /// <summary>
        /// Получает теги статьи
        /// </summary>
        /// <param name="_article">Статья(объект) которой присваиваются теги</param>
        /// <param name="fullArticle">html страница с полной статьёй полученная по прямой ссылке</param>
        /// <returns></returns>
        protected abstract void GetTags(Article _article, IHtmlDocument fullArticle);

        /// <summary>
        /// Выполняет парсинг статей
        /// </summary>
        /// <param name="_articles">Список в который записываются новые спаршенные статьи</param>
        /// <returns></returns>
        public async Task ParseAsync(List<Article> _articles)
        {
            //Определение главной страницы с которой будут загружены превью
            GetStartUrl();

            HtmlParser htmlParser = new HtmlParser();

            sources = new string[startUrl.Length];
            documents = new IHtmlDocument[startUrl.Length];

            //Получение страниц в виде строк
            for (int i = 0; i < startUrl.Length; i++)
            { 
                sources[i] = await HtmlLoader.LoadAsync(startUrl[i]);
                //sources[i] = PageSolver.GetSolvedPage(startUrl[i]);

                documents[i] = await htmlParser.ParseDocumentAsync(sources[i]);
            }

            var items = GetItems();

            int succesArt = 0;
            int totalArt = items.Count();

            foreach (var item in items)
            {
                Article article = new Article();

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
                    source = await HtmlLoader.LoadAsync(article.Url);
                    //source = PageSolver.GetSolvedPage(startUrl);
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
                    GetTitle(article, item, document);
                }
                catch
                {
                    //Console.WriteLine(DateTime.Now.ToString() + "; Ошибка при парсинге GetTitle. Источник: " + startUrl);
                }

                try
                {
                    GetSummary(article, item, document);

                    //Обрезка символов
                    string sum = article.Summary;
                    article.Summary = new string(sum.Take(Settings.SummaryLength).ToArray()) + "...";
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
                    GetUrlMainImg(article, item, document);
                }
                catch
                {
                    //Console.WriteLine(DateTime.Now.ToString() + "; Ошибка при парсинге GetUrlMainImg. Источник: " + startUrl);
                }

                try
                {
                    GetViews(article, item, document);
                }
                catch
                {
                    //Console.WriteLine(DateTime.Now.ToString() + "; Ошибка при парсинге GetViews. Источник: " + startUrl);
                }
                try
                {
                    GetTags(article, document);
                }
                catch
                {
                    //Console.WriteLine(DateTime.Now.ToString() + "; Ошибка при парсинге GetTags. Источник: " + startUrl);
                }

                if (ArticleIsFull(article))
                {
                    _articles.Add(article);
                    succesArt++;

                    //Console.WriteLine(DateTime.Now.ToString() + "; Статья получена. Источник: " + startUrl);
                }
            }
            
            Console.WriteLine(DateTime.Now.ToString() + ": Получено статей: " + succesArt + "/" + totalArt + " из " + startUrl[0]);
        }

        /// <summary>
        /// Обновляет некоторые данные в уже существующих статьях
        /// </summary>
        /// <param name="_articles">Список статей в которых обновляются данные</param>
        /// <returns></returns>
        public async Task UpdateAsync(List<Article> _articles)
        {
            HtmlParser htmlParser = new HtmlParser();

            sources = new string[_articles.Count];
            documents = new IHtmlDocument[_articles.Count];

            foreach (Article article in _articles)
            {
                string sourcehtml = await HtmlLoader.LoadAsync(article.Url);

                IHtmlDocument doc = await htmlParser.ParseDocumentAsync(sourcehtml);

                try
                {
                    GetViews(article, null, doc);
                }
                catch
                {
                    //Console.WriteLine(DateTime.Now.ToString() + "; Ошибка при парсинге GetViews. Источник: " + startUrl);
                }
            }
        }

        /// <summary>
        /// Определяет насколько статья заполнена данными полученными при парсинге.
        /// </summary>
        /// <param name="article">Статья проверяемая на наличие необходимых данных</param>
        /// <returns></returns>
        private bool ArticleIsFull(Article article)
        {
            if (article.Title != null && article.Summary != null && article.Url != null && article.Source.Name != null && article.Source.Url != null && article.UrlMainImg != null && article.Views != 0)
                return true;
            else
                return false;
        }
    }
}
