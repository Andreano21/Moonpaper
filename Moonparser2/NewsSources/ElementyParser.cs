using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Moonparser.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonparser.NewsSources
{
    class ElementyParser : Parser
    {
        protected override void GetStartUrls()
        {
            startUrls = new string[] { "https://elementy.ru/novosti_nauki", "https://elementy.ru/nauchno-populyarnaya_biblioteka" };
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            var curentitems = documents[0].QuerySelector("div.clblock").QuerySelectorAll("div").Where(item => item.ClassName != null && item.ClassName.Contains("img_block32"));

            items.AddRange(curentitems);

            curentitems = documents[1].QuerySelector("div.clblock").QuerySelectorAll("div").Where(item => item.ClassName != null && item.ClassName.Contains("img_block32"));

            items.AddRange(curentitems);

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = "https://elementy.ru" + reducedArticle.QuerySelector("a.nohover").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            _article.Body = fullArticle.QuerySelector("div.memo").TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = reducedArticle.QuerySelector("div.title").TextContent;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var summ = fullArticle.QuerySelector("div.memo");

            try
            {
                _article.Summary = summ.QuerySelector("p.Intro").TextContent;
            }
            catch
            {
                try 
                { 
                    _article.Summary = summ.QuerySelector("p.intro").TextContent;
                }
                catch
                {
                    try { summ.QuerySelector("div").Remove(); } catch { }
                    try { summ.QuerySelector("a").Remove(); } catch { }
                    try { summ.QuerySelector("h1").Remove(); } catch { }
                    try { summ.QuerySelector("h2").Remove(); } catch { }

                    _article.Summary = summ.QuerySelector("p").TextContent;
                }
            }
        }

        protected override void GetSource(Article _article)
        {
            _article.Source.Name = "elementy.ru";
        }

        protected override void GetUrlSource(Article _article)
        {
            _article.Source.Url = "https://elementy.ru/";
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string imgurl = "https://elementy.ru/" + reducedArticle.QuerySelector("img").Attributes["src"].Value;

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
                string strViews = fullArticle.QuerySelector("div.forum_icon").QuerySelector("span.fmcounter").TextContent;

                _article.Views = Helper.ParseViews(strViews) * 1049;
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                string strViews = fullArticle.QuerySelector("div.forum_icon").QuerySelector("span.fmcounter").TextContent;

                _article.Views = Helper.ParseViews(strViews) * 1049;
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IHtmlDocument fullArticle)
        {
            var tagConteiner = fullArticle.QuerySelector("div.sublink");
            var tags = tagConteiner.QuerySelectorAll("a").ToArray();

            //первый и последний элементы не являются тэгами
            for (int i = 1; i < tags.Length - 1; i++)
            {
                string result = tags[i].TextContent;

                if (result.Length < Settings.TagLength)
                    _article.Tags.Add(new Tag(result));
            }
        }
    }
}
