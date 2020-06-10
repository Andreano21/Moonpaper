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
    /// <summary>
    /// Обеспечивает парсинг статей из ресурса Habra.com
    /// </summary>
    class HabraParser : Parser
    {
        protected override void GetStartUrls()
        {
            startUrls = new string[] { "https://habr.com/ru/top/" };
        }
        protected override IEnumerable<IElement> GetItems()
        {
            List<IElement> items = new List<IElement>();

            foreach (var d in documents)
            {
                items.AddRange(d.QuerySelectorAll("li").Where(item => item.ClassName != null && item.ClassName.Contains("content-list__item content-list__item_post shortcuts_item")));
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = reducedArticle.QuerySelector("a.post__title_link").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            _article.Body = fullArticle.QuerySelector("div.post__text").TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = reducedArticle.QuerySelector("a.post__title_link").TextContent;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = reducedArticle.QuerySelector("div.post__text").TextContent;
        }

        protected override void GetSource(Article _article)
        {
            _article.Source.Name = "habr.com";
        }

        protected override void GetUrlSource(Article _article)
        {
            _article.Source.Url = "https://habr.com/ru";
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.UrlMainImg = reducedArticle.QuerySelector("div.post__text").QuerySelector("img").Attributes["src"].Value;
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
                string strViews = fullArticle.QuerySelector("span.post-stats__views-count").TextContent;

                _article.Views = Helper.ParseViews(strViews);
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                string strViews = fullArticle.QuerySelector("span.post-stats__views-count").TextContent;

                _article.Views = Helper.ParseViews(strViews);
            }
            //Загрузка данных из сокращенной страницы
            else
            { 
            
            }
        }

        protected override void GetTags(Article _article, IHtmlDocument fullArticle)
        {
            _article.Tags.Add(new Tag("IT"));
            var tags = document.QuerySelectorAll("a").Where(item => item.ClassName != null && item.ClassName.Contains("inline-list__item-link hub-link "));

            foreach (var tag in tags)
            {
                if (tag.TextContent.Length < Settings.TagLength)
                    _article.Tags.Add(new Tag(tag.TextContent));
            }
        }
    }
}
