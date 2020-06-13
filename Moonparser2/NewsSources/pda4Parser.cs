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
    class Pda4Parser : Parser
    {
        protected override void SetSettings()
        {
            startUrls = new string[] { "https://4pda.ru/" };
            sourceName = "4pda.ru";
            sourceUrl = "https://4pda.ru/";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            for (int d = 0; d < documents.Length; d++)
            {
                var curentitems = documents[d].QuerySelectorAll("article.post");

                 items.AddRange(curentitems);
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = reducedArticle.QuerySelector("a").Attributes["href"].Value;
            _article.Url = _article.Url.Replace("//", "");
            _article.Url = "https://" + _article.Url;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            var t1 = fullArticle.QuerySelector("div.container");
            var t2 = t1.QuerySelector("div.content");
            _article.Body = t2.QuerySelector("div.content-box").TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = reducedArticle.QuerySelector("a").Attributes["title"].Value;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = reducedArticle.QuerySelector("div.description").QuerySelector("[itemprop = description]").TextContent;
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string imgurl = reducedArticle.QuerySelector("img").Attributes["src"].Value;

            _article.UrlMainImg = imgurl;

            _article.UrlMainImg = _article.UrlMainImg.Replace("//", "");
            _article.UrlMainImg = "https://" + _article.UrlMainImg;
        }

        protected override void GetDateTime(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string dateSource = fullArticle.QuerySelector("div.container").QuerySelector("[itemprop = datePublished]").Attributes["content"].Value;

            _article.DateTime = DateTime.Parse(dateSource);
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {
                string strViews = fullArticle.QuerySelector("div.container").QuerySelector("div.more-box").QuerySelector("a.number").TextContent;

                _article.Views = Helper.ParseViews(strViews) * 1076;
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                string strViews = fullArticle.QuerySelector("div.container").QuerySelector("div.more-box").QuerySelector("a.number").TextContent;

                _article.Views = Helper.ParseViews(strViews) * 1076;
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var tagConteiner = fullArticle.QuerySelector("div.container").QuerySelector("div.more-box").QuerySelector("div.meta");
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
