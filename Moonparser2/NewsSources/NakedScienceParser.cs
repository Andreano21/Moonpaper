using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Moonparser.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moonparser.NewsSources
{
    class NakedScienceParser : Parser
    {
        protected override void SetSettings()
        {
            startUrls = new string[] { "https://naked-science.ru/" };
            pageSolverType = PageSolverType.CEF;
            sourceName = "naked-science.ru";
            sourceUrl = "https://naked-science.ru/";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            for (int d = 0; d < documents.Length; d++)
            {
                var curentitems = documents[d].QuerySelector("div.news-items").QuerySelectorAll("div.news-item");

                items.AddRange(curentitems);
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = reducedArticle.QuerySelector("a.animate-custom").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            _article.Body = fullArticle.QuerySelector("div.content").QuerySelector("div.body").TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            reducedArticle.QuerySelector("div.news-item-title").QuerySelector("span").Remove();

            _article.Title = reducedArticle.QuerySelector("div.news-item-title").QuerySelector("a").TextContent;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = reducedArticle.QuerySelector("div.news-item-excerpt").QuerySelector("p").TextContent;
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string imgurl = fullArticle.QuerySelector("div.post-image-container").QuerySelector("div.post-image-inner").QuerySelector("noscript").QuerySelector("img").Attributes["src"].Value;

            _article.UrlMainImg = imgurl;
        }

        protected override void GetDateTime(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string dateSource = reducedArticle.QuerySelector("div.meta-items").QuerySelector("div.meta-item").TextContent;

            if (dateSource.Contains("Вчера"))
            {
                DateTime dt = DateTime.Now;
                dt.AddDays(-1);
                _article.DateTime = dt;
            }
            else
            {
                Regex regex = new Regex(@"\d");
                MatchCollection matches = regex.Matches(dateSource);

                string date = "";

                if (matches.Count > 0)
                    foreach (Match m in matches)
                        date = m.Value;


                _article.DateTime = DateTime.Now.AddHours(-Int32.Parse(date));
            }
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {
                string views = fullArticle.QuerySelector("div.content").QuerySelector("div.meta-items-r").QuerySelector("span.fvc-count").TextContent;

                var viewsStr = views.Replace(" ", "");

                _article.Views = Helper.ParseViews(viewsStr);
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                string views = fullArticle.QuerySelector("div.content").QuerySelector("div.meta-items-r").QuerySelector("span.fvc-count").TextContent;

                var viewsStr = views.Replace(" ", "");

                _article.Views = Helper.ParseViews(viewsStr);
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var tags = reducedArticle.QuerySelector("div.terms-items").QuerySelectorAll("a.animate-custom");

            foreach (var tag in tags)
            {
                string tagStr = tag.TextContent;
                string st = tagStr.Replace("# ", "");

                if (st.Length < Settings.TagLength)
                    _article.Tags.Add(new Tag(st));
            }
        }
    }
}
