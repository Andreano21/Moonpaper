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
            startUrl = "https://stopgame.ru";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            var items = document.QuerySelectorAll("div").Where(item => item.ClassName != null && item.ClassName.Contains("lent-block lent-main"));
            return items;
        }

        protected override void GetUrl(Article _article, IElement _item)
        {
            _article.Url = startUrl + _item.QuerySelector("a").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument _document)
        {
            _article.Body = _document.QuerySelector("div.main_text").TextContent;
        }

        protected override void GetTitle(Article _article, IElement _item, IHtmlDocument _document)
        {
            _article.Title = _item.QuerySelector("img").Attributes["alt"].Value;
        }

        protected override void GetSummary(Article _article, IElement _item, IHtmlDocument _document)
        {
            _article.Summary = _item.QuerySelector("div.brief").QuerySelector("p").TextContent;
        }

        protected override void GetSource(Article _article)
        {
            _article.Source = "stopgame.ru";
        }

        protected override void GetUrlSource(Article _article)
        {
            _article.UrlSource = "https://stopgame.ru";
        }

        protected override void GetUrlMainImg(Article _article, IElement _item, IHtmlDocument _document)
        {
            _article.UrlMainImg = _item.QuerySelector("img").Attributes["srcset"].Value;
            _article.UrlMainImg = _article.UrlMainImg.Replace(" 2x", "");
        }

        protected override void GetDateTime(Article _article)
        {
            _article.DateTime = DateTime.Now;
        }

        protected override void GetViews(Article _article, IElement _item, IHtmlDocument _document)
        {
            string strViews = _document.QuerySelectorAll("div").Where(item2 => item2.ClassName != null && item2.ClassName.Contains("lent-views pubinfo-div")).ToArray()[0].TextContent;

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
            _article.Tags += "Games;";


            var DivTags = _document.QuerySelector("div.tags");
            var Tags = DivTags.QuerySelectorAll("a");
            
            foreach (var tag in Tags)
            {
                if (tag.TextContent.Length < 11)
                    _article.Tags += tag.TextContent + ";";
            }
        }

    }
}
