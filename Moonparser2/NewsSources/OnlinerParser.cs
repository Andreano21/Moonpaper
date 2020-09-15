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
    class OnlinerParser : Parser
    {
        protected override void SetSettings()
        {
            startUrls = new string[] { "https://tech.onliner.by", "https://people.onliner.by", "https://realt.onliner.by", "https://auto.onliner.by"};
            pageSolverType = PageSolverType.IE;
            sourceName = "onliner.by";
            sourceUrl = "https://onliner.by/";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            for (int d = 0; d < documents.Length; d++)
            {
                var curentitems = documents[d].QuerySelectorAll("a.news-tiles__stub");
                var curentitems2 = documents[d].QuerySelectorAll("a.news-tidings__stub");

                foreach (var _item in curentitems)
                {
                    string url = _item.Attributes["href"].Value;

                    if (!url.Contains("onliner.by"))
                    {
                        string fullurl = startUrls[d] + url;
                        _item.Attributes["href"].Value = fullurl;
                    }
                }

                foreach (var _item in curentitems2)
                {
                    string fullurl = startUrls[d] + _item.Attributes["href"].Value;
                    _item.Attributes["href"].Value = fullurl;
                }

                items.AddRange(curentitems);
                items.AddRange(curentitems2);

            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = reducedArticle.Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            _article.Body = fullArticle.QuerySelector("div.news-text").TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var v1 = fullArticle.QuerySelector("div.news-header__title").TextContent;

            v1 = v1.Trim();

            _article.Title = v1;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var v1 = fullArticle.QuerySelector("div.news-entry__speech");
            string summ;

            if (v1 != null)
            {
                summ = v1.TextContent;
            }
            else
            { 
                v1 = fullArticle.QuerySelector("div.news-text").QuerySelector("p");
                summ = v1.TextContent;

            }

            summ = summ.Trim();

            _article.Summary = summ;
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string imgurl = fullArticle.QuerySelector("div.news-header__image").Attributes["style"].Value;
            imgurl = imgurl.Replace("background-image: url(", "");
            imgurl = imgurl.Replace(");", "");
            imgurl = imgurl.Replace("'", "");

            _article.UrlMainImg = imgurl;
        }

        protected override void GetDateTime(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var v1 = fullArticle.QuerySelector("meta[property=\"article:published_time\"]").Attributes["content"].Value;

            if(v1 != null)
                _article.DateTime = DateTime.Parse(v1);
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {
                string strViews = fullArticle.QuerySelector("div.news-header__button_views").TextContent;

                strViews = strViews.Replace("/n", "");
                strViews = strViews.Trim();

                int StrToInt;

                bool isParsed = Int32.TryParse(strViews, out StrToInt);

                if (!isParsed)
                {
                    strViews = strViews.Replace(" ", "");
                    strViews = strViews.Replace("k", "000");
                    strViews = strViews.Replace("K", "000");
                    strViews = strViews.Replace(",", "");
                    strViews = strViews.Replace(".", "");
                }

                Int32.TryParse(strViews, out StrToInt);

                _article.Views = StrToInt;
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                string strViews = fullArticle.QuerySelector("div.news-header__button_views").TextContent;
                strViews.Trim();

                int StrToInt;

                bool isParsed = Int32.TryParse(strViews, out StrToInt);

                if (!isParsed)
                {
                    strViews = strViews.Replace(" ", "");
                    strViews = strViews.Replace("k", "000");
                    strViews = strViews.Replace("K", "000");
                    strViews = strViews.Replace(",", "");
                    strViews = strViews.Replace(".", "");
                }

                Int32.TryParse(strViews, out StrToInt);

                _article.Views = StrToInt;
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var tags = fullArticle.QuerySelector("div.news-reference__list").QuerySelectorAll("a");

            foreach (var tag in tags)
            {
                string t = tag.TextContent.Trim();

                if (t.Length < Settings.TagLength && t != "Onliner")
                    _article.Tags.Add(new Tag(t));
            }
        }
    }
}
