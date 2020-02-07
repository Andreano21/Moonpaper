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
    class HabraParser : Parser
    {
        protected override void GetStartUrl()
        {
            startUrl = "https://habr.com/ru/top/";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            var items = document.QuerySelectorAll("li").Where(item => item.ClassName != null && item.ClassName.Contains("content-list__item content-list__item_post shortcuts_item"));
            return items;
        }

        protected override void GetTitle(Article _article, IElement _item)
        {
            _article.Title = _item.QuerySelector("a.post__title_link").TextContent;
        }

        protected override void GetSummary(Article _article, IElement _item)
        {
            _article.Summary = _item.QuerySelector("div.post__text").TextContent;
        }

        protected override void GetSource(Article _article)
        {
            _article.Source = "habr.com";
        }

        protected override void GetUrl(Article _article, IElement _item)
        {
            _article.Url = _item.QuerySelector("a.post__title_link").Attributes["href"].Value;
        }

        protected override void GetUrlSource(Article _article)
        {
            _article.UrlSource = "https://habr.com/ru";
        }

        protected override void GetUrlMainImg(Article _article, IElement _item)
        {
            _article.UrlMainImg = _item.QuerySelector("div.post__text").QuerySelector("img").Attributes["src"].Value;
        }

        protected override void GetDateTime(Article _article)
        {
            _article.DateTime = DateTime.Now;
        }

        protected override void GetBody(Article _article, IHtmlDocument _document)
        {
            _article.Body = _document.QuerySelector("div.post__text").TextContent;
        }

        protected override void GetViews(Article _article, IHtmlDocument _document)
        {
            string strViews = _document.QuerySelector("span.post-stats__views-count").TextContent;
            int StrToInt;

            bool isParsed = Int32.TryParse(strViews, out StrToInt);

            if (!isParsed)
            {
                strViews = strViews.Replace("k", "000");
                strViews = strViews.Replace("K", "000");
                strViews = strViews.Replace(",", "");
                strViews = strViews.Replace(".", "");
            }

            Int32.TryParse(strViews, out StrToInt);

            _article.Views = StrToInt;

        }

        protected override void GetTags(Article _article, IHtmlDocument _document)
        {
            _article.Tags += "IT;";
            var tags = document.QuerySelectorAll("a").Where(item => item.ClassName != null && item.ClassName.Contains("inline-list__item-link hub-link "));

            foreach (var tag in tags)
            {
                if(tag.TextContent.Length < 11)
                    _article.Tags += tag.TextContent + ";"; 
            }
        }
    }
}
