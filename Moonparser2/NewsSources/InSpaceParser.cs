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
    class InSpaceParser : Parser
    {
        protected override void SetSettings()
        {
            startUrls = new string[] { "https://in-space.ru/news/" };
            pageSolverType = PageSolverType.Not;
            sourceName = "in-space.ru";
            sourceUrl = "https://in-space.ru";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            for (int d = 0; d < documents.Length; d++)
            {
                var curentitems = documents[d].QuerySelector("div.content").QuerySelectorAll("article.news_post_container");

                items.AddRange(curentitems);
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = reducedArticle.QuerySelector("h2").QuerySelector("a").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            _article.Body = fullArticle.QuerySelector("div.section-content").QuerySelector("div.content").TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = reducedArticle.QuerySelector("h2").QuerySelector("a").TextContent;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = reducedArticle.QuerySelector("div.news_post_excerpt").TextContent.Trim();
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string imgurl = fullArticle.QuerySelector("div.section-content").QuerySelector("figure.msnry_item").QuerySelector("img").Attributes["src"].Value;

            _article.UrlMainImg = imgurl;
        }

        protected override void GetDateTime(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string dateSource = fullArticle.QuerySelector("div.post-container").QuerySelector("div.post-pub").QuerySelector("time").Attributes["datetime"].Value;

            _article.DateTime = DateTime.Parse(dateSource);
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {
                fullArticle.QuerySelector("div.post-container").QuerySelector("div.post-pub").QuerySelector("i.single_view_count").Remove();

                string views = fullArticle.QuerySelector("div.post-container").QuerySelector("div.post-pub").QuerySelector("i.single_view_count").TextContent;

                _article.Views = Helper.ParseViews(views);
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                fullArticle.QuerySelector("div.post-container").QuerySelector("div.post-pub").QuerySelector("i.single_view_count").Remove();

                string views = fullArticle.QuerySelector("div.post-container").QuerySelector("div.post-pub").QuerySelector("i.single_view_count").TextContent;

                _article.Views = Helper.ParseViews(views);
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var tags = reducedArticle.QuerySelector("div.news_post_info_container").QuerySelector("div.single_category").QuerySelectorAll("a");

            foreach (var tag in tags)
            {
                string result = tag.TextContent;

                if (result.Length < Settings.TagLength)
                    _article.Tags.Add(new Tag(result));
            }
        }
    }
}
