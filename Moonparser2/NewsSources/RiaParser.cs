//Предварительный парсер. Получает доступные названия статей и прямые ссылки на них

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
    class RiaParser : Parser
    {
        protected override void SetSettings()
        {
            startUrls = new string[] { "https://ria.ru/" };
            sourceName = "ria.ru";
            sourceUrl = "https://ria.ru/";
            pageSolverType = PageSolverType.CEF;
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            foreach (var d in documents)
            {
                items.AddRange(d.QuerySelectorAll("a").Where(item => item.ClassName != null && item.ClassName.Contains("cell-list__item-link color-font-hover-only")));
                items.AddRange(d.QuerySelectorAll("a").Where(item => item.ClassName != null && item.ClassName.Contains("cell-main-photo__link")));
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = reducedArticle.Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            fullArticle.QuerySelector("div.article__body").QuerySelector("strong").Remove();
            _article.Body = fullArticle.QuerySelector("div.article__body").TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = reducedArticle.QuerySelector("span.cell-list__item-title").TextContent;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = fullArticle.QuerySelector("div.article__body").QuerySelector("div.article__text").TextContent;
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.UrlMainImg = fullArticle.QuerySelector("div.article__header").QuerySelector("div.photoview__open").QuerySelector("img").Attributes["src"].Value;
        }

        protected override void GetDateTime(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string dateSource = fullArticle.QuerySelector("div.article__info-date").QuerySelector("a").TextContent;

            _article.DateTime = DateTime.Parse(dateSource);
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {
                var v1 = fullArticle.QuerySelector("div.article__header");
                var v2 = v1.QuerySelector("div.article__info-statistic");
                var v3= v2.QuerySelector("span.statistic__item");

                string strViews = v3.TextContent;
                
                _article.Views = Helper.ParseViews(strViews);
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                var v1 = fullArticle.QuerySelector("div.article__header");
                var v2 = v1.QuerySelector("div.article__info-statistic");
                var v3 = v2.QuerySelector("span.statistic__item");

                string strViews = v3.TextContent;

                _article.Views = Helper.ParseViews(strViews);
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {

            var tags = fullArticle.QuerySelector("div.article__tags").QuerySelectorAll("a");

            foreach (var tag in tags)
            {
                string t = tag.TextContent;

                if (t.Length < Settings.TagLength)
                    _article.Tags.Add(new Tag(t));
            }
        }
    }
}
