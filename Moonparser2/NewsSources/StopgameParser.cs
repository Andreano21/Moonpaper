﻿//Предварительный парсер. Получает доступные названия статей и прямые ссылки на них

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
    class StopgameParser : Parser
    {
        protected override void SetSettings()
        {
            startUrls = new string[] { "https://stopgame.ru" };
            sourceName = "stopgame.ru";
            sourceUrl = "https://stopgame.ru";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            List<IElement> items = new List<IElement>();

            foreach (var d in documents)
            {
                var v1 = d.QuerySelectorAll("div").Where(item => item.ClassName != null && item.ClassName.Contains("item article-summary article-summary-card"));

                items.AddRange(v1);
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = startUrls[0] + reducedArticle.QuerySelector("a").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            _article.Body = fullArticle.QuerySelector("section.article").TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = fullArticle.QuerySelector("h1.article-title").TextContent;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = fullArticle.QuerySelector("section.article").QuerySelector("p").TextContent;
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {

            var t1 = reducedArticle.QuerySelector(".image.lazy");


            string imgurl = t1.Attributes["data-src"].Value;
            //imgurl = imgurl.Replace("background-image: url(","");
            //imgurl = imgurl.Replace(");", "");

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
                string strViews = fullArticle.QuerySelectorAll("div").Where(item2 => item2.ClassName != null && item2.ClassName.Contains("article-info-item")).ToArray()[3].TextContent;
                _article.Views = Helper.ParseViews(strViews);
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                string strViews = fullArticle.QuerySelectorAll("div").Where(item2 => item2.ClassName != null && item2.ClassName.Contains("article-info-item")).ToArray()[3].TextContent;
                _article.Views = Helper.ParseViews(strViews);
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Tags.Add(new Tag("Games"));
        }
    }
}
