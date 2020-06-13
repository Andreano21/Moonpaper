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
    class KpParser : Parser
    {
        protected override void SetSettings()
        {
            startUrls = new string[] { "https://www.msk.kp.ru/", "https://www.kp.by/" };
            sourceName = "kp.ru";
            sourceUrl = "https://www.kp.ru/";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            var curentitems = documents[0].QuerySelector("div.container").QuerySelectorAll("div.digest");
            var curentitems2 = documents[0].QuerySelector("div.boxPage").QuerySelector("div.wSection").QuerySelectorAll("article.digest");
            var curentitems3 = documents[1].QuerySelector("div.container2").QuerySelectorAll("div.digest");
            var curentitems4 = documents[1].QuerySelector("div.boxPage").QuerySelector("div.wSection").QuerySelectorAll("article.digest");

            items.AddRange(curentitems);
            items.AddRange(curentitems2);
            items.AddRange(curentitems3);
            items.AddRange(curentitems4);
            
            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = reducedArticle.QuerySelector("a").Attributes["href"].Value;

            _article.Url = _article.Url.Replace("https://www.msk.kp.ru", "");
            _article.Url = _article.Url.Replace("https://www.kp.by", "");
            _article.Url = _article.Url.Replace("https://www.kp.ru", "");

            _article.Url = _article.Url.Replace("http://www.msk.kp.ru", "");
            _article.Url = _article.Url.Replace("http://www.kp.by", "");
            _article.Url = _article.Url.Replace("http://www.kp.ru", "");

            _article.Url = _article.Url.Replace("http://msk.kp.ru", "");
            _article.Url = _article.Url.Replace("http://kp.by", "");
            _article.Url = _article.Url.Replace("http://kp.ru", "");

            _article.Url = "https://www.kp.ru" + _article.Url;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            _article.Body = fullArticle.QuerySelector("div.boxPage").QuerySelector("div.js-mediator-article").TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = fullArticle.QuerySelector("article.article").QuerySelector("h1").TextContent;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = fullArticle.QuerySelector("div.ArticleDescription").TextContent;
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string imgurl = fullArticle.QuerySelector("div.photo").QuerySelector("img").Attributes["src"].Value;

            _article.UrlMainImg = imgurl;
        }

        protected override void GetDateTime(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string dateSource = fullArticle.QuerySelector("div.boxPage").QuerySelector("time").Attributes["datetime"].Value;

            _article.DateTime = DateTime.Parse(dateSource);
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {
                string strViews = "";
                try
                {
                    strViews = fullArticle.QuerySelector("div.boxPage").QuerySelector("div.comments").QuerySelectorAll("span")[1].TextContent;
                }
                catch
                {

                }

                if (strViews == "")
                {
                    Random rnd = new Random();
                    int v = rnd.Next(350, 2500);
                    _article.Views = v;
                }
                else
                { 
                    _article.Views = Helper.ParseViews(strViews) * 1057;
                }
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                string strViews = "";
                try
                {
                    strViews = fullArticle.QuerySelector("div.boxPage").QuerySelector("div.comments").QuerySelectorAll("span")[1].TextContent;
                }
                catch
                {

                }

                if (strViews == "")
                {
                    Random rnd = new Random();
                    int v = rnd.Next(350, 2500);
                    _article.Views = v;
                }
                else
                {
                    _article.Views = Helper.ParseViews(strViews) * 1057;
                }
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var tagConteiner = fullArticle.QuerySelector("div.boxPage").QuerySelector("div.tags");
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
