using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Moonparser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moonparser.NewsSources
{
    class ProglibParser : Parser
    {
        protected override void SetSettings()
        {
            startUrls = new string[] { "https://proglib.io/" };
            pageSolverType = PageSolverType.Not;
            sourceName = "proglib.io";
            sourceUrl = "https://proglib.io";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            for (int d = 0; d < documents.Length; d++)
            {
                var v1 = documents[d].QuerySelector("div.feed__items");

                //v1.QuerySelector("article.item-pinned").Remove(); // удаление рекламного блока

                var v2 = v1.QuerySelectorAll("article.preview-card");

                items.AddRange(v2);
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = sourceUrl + reducedArticle.QuerySelector("a.no-link").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            var v1 = fullArticle.QuerySelector("main.sheet__main");
            var v2 = v1.QuerySelector("article");
            var v3 = v2.QuerySelector("div.block__content");

            _article.Body = v3.TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string str = reducedArticle.QuerySelector("a.no-link").QuerySelector("h2.preview-card__title").TextContent;

            _article.Title = Regex.Replace(str, @"\p{Cs}", "");
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = reducedArticle.QuerySelector("a.no-link").QuerySelector("div.preview-card__text").TextContent;
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var v1 = fullArticle.QuerySelector("main.sheet__main");
            var v2 = v1.QuerySelector("article");
            var v3 = v2.QuerySelector("header");
            var v4 = v3.QuerySelector("div.block__full");
            var v5 = v4.QuerySelector("img");

            string imgurl = v5.Attributes["src"].Value;

            _article.UrlMainImg = imgurl;
        }

        protected override void GetDateTime(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var v1 = fullArticle.QuerySelector("main.sheet__main");
            var v2 = v1.QuerySelector("article");
            var v3 = v2.QuerySelector("header");
            var v4 = v3.QuerySelector("div.flex-wrap");
            var v5 = v4.QuerySelector("span.publish-info");

            string dateSource = v5.Attributes["title"].Value;

            dateSource = dateSource.Replace(" в ", " ");

            _article.DateTime = DateTime.Parse(dateSource);
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {
                var v1 = fullArticle.QuerySelector("main.sheet__main");
                var v2 = v1.QuerySelector("article");
                var v3 = v2.QuerySelector("header");
                var v4 = v3.QuerySelector("div.mv-4");
                var v5 = v4.QuerySelector("div.reactions-bar__info");

                string views = v5.QuerySelector("div.views").QuerySelector("span.ml-1").TextContent;

                string comments = v5.QuerySelector("div.reactions-bar__reactions").QuerySelector("a.reaction").QuerySelector("span.reaction__count").TextContent;

                string likes = v5.QuerySelector("div.reactions-bar__reactions").QuerySelector("button.reaction").QuerySelector("span.reaction__count").TextContent;

                _article.Views = Helper.ParseViews(views) + Helper.ParseViews(likes) * 346 + Helper.ParseViews(comments) * 516;
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                var v1 = fullArticle.QuerySelector("main.sheet__main");
                var v2 = v1.QuerySelector("article");
                var v3 = v2.QuerySelector("header");
                var v4 = v3.QuerySelector("div.mv-4");
                var v5 = v4.QuerySelector("div.reactions-bar__info");

                string views = v5.QuerySelector("div.views").QuerySelector("span.ml-1").TextContent;

                string comments = v5.QuerySelector("div.reactions-bar__reactions").QuerySelector("a.reaction").QuerySelector("span.reaction__count").TextContent;

                string likes = v5.QuerySelector("div.reactions-bar__reactions").QuerySelector("button.reaction").QuerySelector("span.reaction__count").TextContent;

                _article.Views = Helper.ParseViews(views) + Helper.ParseViews(likes) * 346 + Helper.ParseViews(comments) * 516;
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var v1 = fullArticle.QuerySelector("main.sheet__main");
            var v2 = v1.QuerySelector("article");
            var v3 = v2.QuerySelector("footer.block__footer");

            v3.QuerySelector("div.border-v").Remove();

            var v4 = v3.QuerySelector("div.valign-center");
            var tags = v4.QuerySelectorAll("a");

            foreach (var tag in tags)
            {
                string result = tag.TextContent;

                if (result.Length < Settings.TagLength)
                    _article.Tags.Add(new Tag(result));
            }
        }
    }
}
