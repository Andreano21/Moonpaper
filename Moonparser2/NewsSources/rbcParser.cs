//Предварительный парсер. Получает доступные названия статей и прямые ссылки на них

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Moonparser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonparser.NewsSources
{
    class rbcParser : Parser
    {
        protected override void GetStartUrls()
        {
            startUrls = new string[] { "https://www.rbc.ru/" };
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            foreach (var d in documents)
            {
                items.AddRange(d.QuerySelectorAll("a").Where(item => item.ClassName != null && item.ClassName.Contains("main__feed__link")));
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = reducedArticle.Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {

        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = reducedArticle.QuerySelector("span.main__feed__title").TextContent.Replace("/n","");
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = fullArticle.QuerySelector("div.article__text__overview").QuerySelector("span").TextContent;

            //Обрезка пробелов в начале и конце строки
            //_article.Summary.Trim();
        }

        protected override void GetSource(Article _article)
        {
            _article.Source.Name = "rbc.ru";
        }

        protected override void GetUrlSource(Article _article)
        {
            _article.Source.Url = "https://www.rbc.ru/";
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string imgurl = fullArticle.QuerySelector("img.article__main-image__image").Attributes["src"].Value;

            _article.UrlMainImg = imgurl;
        }

        protected override void GetDateTime(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.DateTime = DateTime.Now;
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {

                //string strViews1 = fullArticle.Body.Text();
                //Console.WriteLine(strViews1);

                string strViews = fullArticle.QuerySelector("div.article__header__counter-block").QuerySelector("span.article__header__counter").TextContent;

                int StrToInt;

                bool isParsed = Int32.TryParse(strViews, out StrToInt);

                if (!isParsed)
                {
                    strViews = strViews.Replace(" ", "");
                    strViews = strViews.Replace("k", "000");
                    strViews = strViews.Replace("K", "000");
                    strViews = strViews.Replace(",", "");
                    strViews = strViews.Replace(".", "");
                }

                Int32.TryParse(strViews, out StrToInt);

                _article.Views = StrToInt;
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                string strViews = fullArticle.QuerySelector("div.article__header__counter-block").QuerySelector("span.article__header__counter js-insert-views-count").TextContent;

                int StrToInt;

                bool isParsed = Int32.TryParse(strViews, out StrToInt);

                if (!isParsed)
                {
                    strViews = strViews.Replace(" ", "");
                    strViews = strViews.Replace("k", "000");
                    strViews = strViews.Replace("K", "000");
                    strViews = strViews.Replace(",", "");
                    strViews = strViews.Replace(".", "");
                }

                Int32.TryParse(strViews, out StrToInt);

                _article.Views = StrToInt;
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IHtmlDocument fullArticle)
        {

            var tags = fullArticle.QuerySelector("div.article__tags").QuerySelectorAll("a").Where(item => item.ClassName != null && item.ClassName.Contains("article__tags__link"));

            foreach (var tag in tags)
            {
                string t = tag.TextContent;
                t = t.Replace(",", "");
                t = t.Trim();

                if (t.Length < Settings.TagLength)
                    _article.Tags.Add(new Tag(t));
            }
        }
    }
}
