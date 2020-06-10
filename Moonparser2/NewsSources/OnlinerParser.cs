﻿//Предварительный парсер. Получает доступные названия статей и прямые ссылки на них

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
        protected override void GetStartUrls()
        {
            startUrls = new string[] { "https://tech.onliner.by", "https://people.onliner.by", "https://realt.onliner.by", "https://auto.onliner.by"};
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            for (int d = 0; d < documents.Length; d++)
            {
                var curentitems = documents[d].QuerySelectorAll("div").Where(item => item.ClassName != null && item.ClassName.Contains("news-tidings__item"));

                foreach (var _item in curentitems)
                {
                    try
                    {
                        string fullurl = startUrls[d] + _item.QuerySelector("a.news-tidings__link").Attributes["href"].Value;
                        _item.QuerySelector("a.news-tidings__link").Attributes["href"].Value = fullurl;
                    }
                    catch
                    {

                    }
                }

                items.AddRange(curentitems);
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = reducedArticle.QuerySelector("a.news-tidings__link").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            //_article.Body = _document.QuerySelector("div.news-tidings__speech").TextContent;
            //_article.Body.Trim();
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = reducedArticle.QuerySelector("span.news-helpers_hide_mobile-small").TextContent;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = reducedArticle.QuerySelector("div.news-tidings__speech").TextContent;

            //Обрезка пробелов в начале и конце строки
            _article.Summary.Trim();
        }

        protected override void GetSource(Article _article)
        {
            _article.Source.Name = "onliner.by";
        }

        protected override void GetUrlSource(Article _article)
        {
            _article.Source.Url = "https://onliner.by/";
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string imgurl = reducedArticle.QuerySelector("div.news-tidings__image").Attributes["style"].Value;
            imgurl = imgurl.Replace("background-image: url(", "");
            imgurl = imgurl.Replace(");", "");

            _article.UrlMainImg = imgurl;
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
                string strViews = reducedArticle.QuerySelector("div.news-tidings__button_views").TextContent;
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

        protected override void GetTags(Article _article, IHtmlDocument fullArticle)
        {

            var tags = fullArticle.QuerySelectorAll("a").Where(item => item.ClassName != null && item.ClassName.Contains("news-reference__link news-reference__link_secondary"));

            foreach (var tag in tags)
            {
                string t = tag.TextContent.Trim();

                if (t.Length < Settings.TagLength)
                    _article.Tags.Add(new Tag(t));
            }
        }
    }
}
