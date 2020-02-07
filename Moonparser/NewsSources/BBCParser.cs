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
    class BBCParser : Parser
    {
        protected override void GetStartUrl()
        {
            startUrl = "https://www.bbc.com/russian/news";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            var items = document.QuerySelectorAll("div").Where(item => item.ClassName != null && item.ClassName.Contains("eagle-item faux-block-link"));
            return items;
        }

        protected override void GetUrl(Article _article, IElement _item)
        {
            _article.Url = "www.bbc.com" + _item.QuerySelector("a.title-link").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument _document)
        {
            var bodyElements = _document.QuerySelector("div.story-body__inner").QuerySelectorAll("p");

            foreach (var element in bodyElements)
            {
                _article.Body += element.TextContent;
            }
        }

        protected override void GetTitle(Article _article, IElement _item, IHtmlDocument _document)
        {
            _article.Title = _item.QuerySelector("span.title-link__title-text").TextContent;
        }

        protected override void GetSummary(Article _article, IElement _item, IHtmlDocument _document)
        {
            _article.Summary = _item.QuerySelector("p.eagle-item__summary").TextContent;
        }

        protected override void GetSource(Article _article)
        {
            _article.Source = "bbc.com";
        }

        protected override void GetUrlSource(Article _article)
        {
            _article.UrlSource = "https://www.bbc.com/russian/news";
        }

        protected override void GetUrlMainImg(Article _article, IElement _item, IHtmlDocument _document)
        {
            _article.UrlMainImg = _document.QuerySelector("div.story-body__inner").QuerySelector("img").Attributes["src"].Value;
        }

        protected override void GetDateTime(Article _article)
        {
            _article.DateTime = DateTime.Now;
        }

        protected override void GetViews(Article _article, IHtmlDocument _document)
        {
            //Данные на сайте не указываются
            _article.Views = 0;
        }

        protected override void GetTags(Article _article, IHtmlDocument _document)
        {
            //_article.Tags += "News;";
            var tags = document.QuerySelectorAll("li").Where(item => item.ClassName != null && item.ClassName.Contains("tags-list__tags"));

            foreach (var tag in tags)
            {
                string strTag = tag.QuerySelector("a").TextContent;

                if(strTag.Length < 11)
                    _article.Tags += strTag + ";"; 
            }
        }
    }
}
