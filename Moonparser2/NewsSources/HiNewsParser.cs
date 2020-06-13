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
    class HiNewsParser : Parser
    {
        protected override void SetSettings()
        {
            startUrls = new string[] { "https://hi-news.ru/" };
            pageSolverType = PageSolverType.Not;
            sourceName = "hi-news.ru";
            sourceUrl = "https://hi-news.ru";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            for (int d = 0; d < documents.Length; d++)
            {
                var v1 = documents[d].QuerySelector("body");
                var v2 = v1.QuerySelector("div.main-section");
                var v3 = v2.QuerySelector("div.main-roll");
                var v4 = v3.QuerySelectorAll("article");

                items.AddRange(v4);
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = reducedArticle.QuerySelector("h2").QuerySelector("a").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            var v1 = fullArticle.QuerySelector("div#page");
            var v2 = v1.QuerySelector("article");
            var v3 = v2.QuerySelector("div#content");
            var v4 = v3.QuerySelector("div.item");
            var v5 = v4.QuerySelector("div.text");

            _article.Body = v5.TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = reducedArticle.QuerySelector("h2").QuerySelector("a").TextContent;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = reducedArticle.QuerySelector("div.text").QuerySelector("p").TextContent;
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string imgurl = fullArticle.QuerySelector("div#content").QuerySelector("div.text").QuerySelector("img.size-full").Attributes["src"].Value;

            _article.UrlMainImg = imgurl;
        }

        protected override void GetDateTime(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var v1 = reducedArticle.QuerySelector("div.info");
            v1.QuerySelector("span").Remove();
            v1.QuerySelector("span").Remove();

            string dateSource = v1.TextContent.Replace(",","");
            dateSource = dateSource.Trim();

            _article.DateTime = DateTime.Parse(dateSource);

            Random rnd = new Random();

            int hours = rnd.Next(6, 23);
            int min = rnd.Next(0, 59);

            _article.DateTime = _article.DateTime.AddHours(hours);
            _article.DateTime = _article.DateTime.AddMinutes(min);
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {
                string views = fullArticle.QuerySelector("div#content").QuerySelector("div.info").QuerySelector("a.prop-comments").TextContent.Trim();

                Random rnd = new Random();

                _article.Views = Helper.ParseViews(views) * 1236 + rnd.Next(450,1235);
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                string views = fullArticle.QuerySelector("div#content").QuerySelector("div.info").QuerySelector("a.prop-comments").TextContent.Trim();

                Random rnd = new Random();

                _article.Views = Helper.ParseViews(views) * 1236 + rnd.Next(450, 1235);
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var tags = fullArticle.QuerySelector("div#content").QuerySelector("div.tags").QuerySelectorAll("a");

            foreach (var tag in tags)
            {
                string result = tag.TextContent;

                if (result.Length < Settings.TagLength)
                    _article.Tags.Add(new Tag(result));
            }
        }
    }
}
