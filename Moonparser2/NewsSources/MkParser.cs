using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Moonparser.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonparser.NewsSources
{
    class MkParser : Parser
    {
        protected override void SetSettings()
        {
            startUrls = new string[] { "https://www.mk.ru/news/"};
            sourceName = "mk.ru";
            sourceUrl = "https://www.mk.ru/";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            for (int d = 0; d < documents.Length; d++)
            {
                var curentitems = documents[d].QuerySelector("ul.news_list").QuerySelectorAll("li");

                 items.AddRange(curentitems);
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = reducedArticle.QuerySelector("a").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            _article.Body = fullArticle.QuerySelector("div.inread-content").TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = reducedArticle.QuerySelector("a").TextContent;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = fullArticle.QuerySelector("div.inread-content").QuerySelector("p").TextContent;
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string imgurl = fullArticle.QuerySelector("div.content").QuerySelector("div.big_image").QuerySelector("img").Attributes["src"].Value;

            _article.UrlMainImg = imgurl;
        }

        protected override void GetDateTime(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string dateSource = fullArticle.QuerySelector("span.date").QuerySelector("[itemprop = datePublished]").Attributes["content"].Value;

            _article.DateTime = DateTime.Parse(dateSource);
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {
                string strViews = fullArticle.QuerySelector("span.date").TextContent;
                strViews.Trim();

                string[] separator = { "просмотров: " };
                string[] strs = strViews.Split(separator, StringSplitOptions.None);

                strViews = strs[strs.Length - 1];

                strViews.Trim();


                _article.Views = Helper.ParseViews(strViews);
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                string strViews = fullArticle.QuerySelector("span.date").TextContent;
                strViews.Trim();

                string[] separator = { "просмотров: " };
                string[] strs = strViews.Split(separator, StringSplitOptions.None);

                strViews = strs[strs.Length - 1];

                strViews.Trim();


                _article.Views = Helper.ParseViews(strViews);
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var tagConteiner = fullArticle.QuerySelector("div.article_info").QuerySelector("span.tags");
            var tags = tagConteiner.QuerySelectorAll("a");

            foreach (var tag in tags)
            {
                string result = tag.TextContent;
                result = result.Replace(",","");

                if (result.Length < Settings.TagLength)
                    _article.Tags.Add(new Tag(result));
            }
        }
    }
}
