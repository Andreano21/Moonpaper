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
    class TutbyParser : Parser
    {
        protected override void GetStartUrl()
        {
            startUrls = new string[] { "https://news.tut.by/?sort=time#sort", "https://news.tut.by/?sort=reads#sort"};
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            for (int d = 0; d < documents.Length; d++)
            {
                var curentitems = documents[d].QuerySelector("div.b-news").QuerySelectorAll("div").Where(item => item.ClassName != null && item.ClassName.Contains("news-entry small pic time"));

                 items.AddRange(curentitems);
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = reducedArticle.QuerySelector("a.entry__link").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            _article.Body = fullArticle.QuerySelector("div#article_body").TextContent;
            //_article.Body.Trim();
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = reducedArticle.QuerySelector("span._title").TextContent;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = fullArticle.QuerySelector("div#article_body").QuerySelector("strong").TextContent;
        }

        protected override void GetSource(Article _article)
        {
            _article.Source.Name = "tut.by";
        }

        protected override void GetUrlSource(Article _article)
        {
            _article.Source.Url = "https://www.tut.by/";
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string imgurl = reducedArticle.QuerySelector("img._image").Attributes["src"].Value;

            _article.UrlMainImg = imgurl;
        }

        protected override void GetDateTime(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string dateSource = fullArticle.QuerySelector("[itemprop = datePublished]").Attributes["datetime"].Value;

            _article.DateTime = DateTime.Parse(dateSource);
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {
                string strViews = fullArticle.QuerySelector("[itemprop = commentCount]").TextContent;

                _article.Views = Helper.ParseViews(strViews) * 1000;
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                string strViews = fullArticle.QuerySelector("[itemprop = commentCount]").TextContent;

                _article.Views = Helper.ParseViews(strViews) * 1000;
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IHtmlDocument fullArticle)
        {
            var tagConteiner = fullArticle.QuerySelector("ul.b-article-info-tags");
            var tags = tagConteiner.QuerySelectorAll("a");

            foreach (var tag in tags)
            {
                string t = tag.TextContent;
                string result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(t);

                if (result.Length < Settings.TagLength && !result.Contains("TUT") && !result.Contains("tut"))
                    _article.Tags.Add(new Tag(result));
                
            }
        }
    }
}
