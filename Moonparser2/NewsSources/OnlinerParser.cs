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
        protected override void GetStartUrl()
        {
            startUrl = "https://tech.onliner.by/";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            var items = document.QuerySelectorAll("div").Where(item => item.ClassName != null && item.ClassName.Contains("news-tidings__item"));
            int x = items.Count();

            return items;
        }

        protected override void GetUrl(Article _article, IElement _item)
        {
            _article.Url = startUrl + _item.QuerySelector("a.news-tidings__link").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument _document)
        {
            //_article.Body = _document.QuerySelector("div.news-tidings__speech").TextContent;
            //_article.Body.Trim();
        }

        protected override void GetTitle(Article _article, IElement _item, IHtmlDocument _document)
        {
            _article.Title = _item.QuerySelector("span.news-helpers_hide_mobile-small").TextContent;
        }

        protected override void GetSummary(Article _article, IElement _item, IHtmlDocument _document)
        {
            _article.Summary = _item.QuerySelector("div.news-tidings__speech").TextContent;

            //Обрезка пробелов в начале и конце строки
            _article.Summary.Trim();
        }

        protected override void GetSource(Article _article)
        {
            _article.Source = "onliner.by";
        }

        protected override void GetUrlSource(Article _article)
        {
            _article.UrlSource = "https://onliner.by/";
        }

        protected override void GetUrlMainImg(Article _article, IElement _item, IHtmlDocument _document)
        {
            string imgurl = _item.QuerySelector("div.news-tidings__image").Attributes["style"].Value;
            imgurl = imgurl.Replace("background-image: url(", "");
            imgurl = imgurl.Replace(");", "");

            _article.UrlMainImg = imgurl;
        }

        protected override void GetDateTime(Article _article)
        {
            _article.DateTime = DateTime.Now;
        }

        protected override void GetViews(Article _article, IElement _item, IHtmlDocument _document)
        {
            string strViews = _document.QuerySelector("div.news-tidings__button_views").TextContent;
            strViews.Trim();

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
            //_article.Tags += "IT;";
            //var tags = document.QuerySelectorAll("a").Where(item => item.ClassName != null && item.ClassName.Contains("inline-list__item-link hub-link "));

            //foreach (var tag in tags)
            //{
            //    if(tag.TextContent.Length < 11)
            //        _article.Tags += tag.TextContent + ";"; 
            //}
        }
    }
}