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
    class TopcorParser : Parser
    {
        protected override void SetSettings()
        {
            startUrls = new string[] { "https://topcor.ru/news/", "https://topcor.ru/politics/", "https://topcor.ru/economy/", "https://topcor.ru/technology/", "https://topcor.ru/society/" };
            pageSolverType = PageSolverType.Not;
            sourceName = "topcor.ru";
            sourceUrl = "https://topcor.ru";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            for (int d = 0; d < documents.Length; d++)
            {
                var v1 = documents[d].QuerySelectorAll("article.post");

                items.AddRange(v1);
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = reducedArticle.QuerySelector("h2.post__title").QuerySelector("a.post__title_link").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            var v1 = fullArticle.QuerySelector("div.showfull__content");

            _article.Body = v1.TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = reducedArticle.QuerySelector("h2.post__title").QuerySelector("a.post__title_link").TextContent;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = reducedArticle.QuerySelector("div.post__description").TextContent;
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string imgurl = reducedArticle.QuerySelector("div.post__media").QuerySelector("div.cover").Attributes["style"].Value;

            imgurl = imgurl.Replace("background-image:url(","");
            imgurl = imgurl.Replace(");", "");
            imgurl = imgurl.Replace(")", "");

            _article.UrlMainImg = imgurl;
        }

        protected override void GetDateTime(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string dateSource = fullArticle.QuerySelector("time.post__meta_time.font-weight-bold").Attributes["datetime"].Value;

            _article.DateTime = DateTime.Parse(dateSource);
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {
                var views = fullArticle.QuerySelector("span.post__meta_comments.font-weight-bold");
                views.QuerySelector("svg").Remove();

                string viewsStr = views.TextContent;

                _article.Views = Helper.ParseViews(viewsStr) * 978;
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                var views = fullArticle.QuerySelector("div.post__meta_comments.font-weight-bold");
                views.QuerySelector("svg").Remove();

                string viewsStr = views.TextContent;

                _article.Views = Helper.ParseViews(viewsStr) * 978;
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var tags = fullArticle.QuerySelector("ul.breadcrumbs.text-truncate").QuerySelectorAll("a");

            foreach (var tag in tags)
            {
                string result = tag.TextContent;

                if (result.Length < Settings.TagLength && result != "Репортёр")
                    _article.Tags.Add(new Tag(result));
            }
        }
    }
}
