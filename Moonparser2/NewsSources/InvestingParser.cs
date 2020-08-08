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
    class InvestingParser : Parser
    {
        protected override void SetSettings()
        {
            startUrls = new string[] { "https://ru.investing.com/news/most-popular-news"};
            pageSolverType = PageSolverType.IE;
            sourceName = "investing.com";
            sourceUrl = "https://ru.investing.com";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            for (int d = 0; d < documents.Length; d++)
            {
                var t1 = documents[d].QuerySelector("div.largeTitle");

                Console.WriteLine(t1.InnerHtml);

                var curentitems = documents[d].QuerySelector("div.largeTitle").QuerySelectorAll("article.js-article-item");

                 items.AddRange(curentitems);
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = sourceUrl + reducedArticle.QuerySelector("a").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            _article.Body = fullArticle.QuerySelector("div.articlePage").TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = reducedArticle.QuerySelector("div.textDiv").QuerySelector("a").TextContent;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = reducedArticle.QuerySelector("div.textDiv").QuerySelector("p").TextContent.Replace("Investing.com — ", "");
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string imgurl = fullArticle.QuerySelector("div.articlePage").QuerySelector("div.imgCarousel").QuerySelector("img").Attributes["src"].Value;

            _article.UrlMainImg = imgurl;
        }

        protected override void GetDateTime(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string dateSource = fullArticle.QuerySelector("div.contentSectionDetails").QuerySelector("span").TextContent;

            Regex regex = new Regex(@"\d{2}.\d{2}.\d{4} \d{2}:\d{2}");

            MatchCollection matches = regex.Matches(dateSource);

            string date = "";

            if (matches.Count > 0)
                foreach (Match m in matches)
                    date = m.Value;

            _article.DateTime = DateTime.Parse(date);
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {
                string strViews = fullArticle.QuerySelector("div.articleFooter").QuerySelector("i.js-counter").TextContent;

                _article.Views = Helper.ParseViews(strViews) * 1347;
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                string strViews = fullArticle.QuerySelector("div.articleFooter").QuerySelector("i.js-counter").TextContent;

                _article.Views = Helper.ParseViews(strViews) * 1347;
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var tagConteiner = fullArticle.QuerySelector("div.contentSectionDetails");
            var tags = tagConteiner.QuerySelectorAll("a");

            foreach (var tag in tags)
            {
                string result = tag.TextContent;

                if (result.Length < Settings.TagLength)
                    _article.Tags.Add(new Tag(result));
                
            }
        }
    }
}
