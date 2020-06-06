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
    class StopgameParser : Parser
    {
        protected override void GetStartUrl()
        {
            startUrls = new string[] { "https://stopgame.ru" };
        }
        protected override IEnumerable<IElement> GetItems()
        {
            List<IElement> items = new List<IElement>();

            foreach (var d in documents)
            {
                items.AddRange(d.QuerySelectorAll("div").Where(item => item.ClassName != null && item.ClassName.Contains("lent-block lent-main")));
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = startUrls[0] + reducedArticle.QuerySelector("a").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            _article.Body = fullArticle.QuerySelector("div.main_text").TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = reducedArticle.QuerySelector("img").Attributes["alt"].Value;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = reducedArticle.QuerySelector("div.brief").QuerySelector("p").TextContent;
        }

        protected override void GetSource(Article _article)
        {
            _article.Source.Name = "stopgame.ru";
        }

        protected override void GetUrlSource(Article _article)
        {
            _article.Source.Url = "https://stopgame.ru";
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.UrlMainImg = reducedArticle.QuerySelector("img").Attributes["srcset"].Value;
            _article.UrlMainImg = _article.UrlMainImg.Replace(" 2x", "");
        }

        protected override void GetDateTime(Article _article)
        {
            _article.DateTime = DateTime.Now;
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {
                string strViews = fullArticle.QuerySelectorAll("div").Where(item2 => item2.ClassName != null && item2.ClassName.Contains("lent-views pubinfo-div")).ToArray()[0].TextContent;
                _article.Views = Helper.ParseViews(strViews);
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                string strViews = fullArticle.QuerySelectorAll("div").Where(item2 => item2.ClassName != null && item2.ClassName.Contains("lent-views pubinfo-div")).ToArray()[0].TextContent;
                _article.Views = Helper.ParseViews(strViews);
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IHtmlDocument fullArticle)
        {
            _article.Tags.Add(new Tag("Games"));

            var DivTags = fullArticle.QuerySelector("div.tags");
            var Tags = DivTags.QuerySelectorAll("a");

            foreach (var tag in Tags)
            {
                if (tag.TextContent.Length < Settings.TagLength)
                    _article.Tags.Add(new Tag(tag.TextContent));
            }
        }

    }
}
