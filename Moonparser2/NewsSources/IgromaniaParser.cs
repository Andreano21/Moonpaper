using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Moonparser.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moonparser.NewsSources
{
    class IgromaniaParser : Parser
    {
        protected override void SetSettings()
        {
            startUrls = new string[] { "https://www.igromania.ru/news/" };
            pageSolverType = PageSolverType.Not;
            sourceName = "igromania.ru";
            sourceUrl = "https://www.igromania.ru";
        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            for (int d = 0; d < documents.Length; d++)
            {
                var curentitems = documents[d].QuerySelector("div.aubl").QuerySelectorAll("div.aubl_item");

                items.AddRange(curentitems);
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = sourceUrl + reducedArticle.QuerySelector("a").Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            _article.Body = fullArticle.QuerySelector("div.page_news_content").TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Title = reducedArticle.QuerySelector("div.aubli_data").QuerySelector("a").TextContent;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            _article.Summary = fullArticle.QuerySelector("div.page_news_content").QuerySelector("div.universal_content").QuerySelector("div").TextContent;
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string imgurl = fullArticle.QuerySelector("div.page_news").QuerySelector("div.main_pic_container").QuerySelector("img").Attributes["src"].Value;

            _article.UrlMainImg = imgurl;
        }

        protected override void GetDateTime(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string dateSource = fullArticle.QuerySelector("div.page_news").QuerySelector("div.page_news_info").TextContent;

            Regex regex = new Regex(@"\d{2}.\d{2}.\d{4} \d{2}:\d{2}");

            MatchCollection matches = regex.Matches(dateSource);

            string date = "";

            if (matches.Count > 0)
                foreach (Match m in matches)
                    date = m.Value;

            _article.DateTime = DateTime.Parse(date);
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {
                var node = fullArticle.QuerySelector("div.page_news").QuerySelector("div.page_news_info").QuerySelector("div.info_block_botrt");

                string strComments = node.QuerySelector("div.info_block_botrt").TextContent;

                node.QuerySelector("div.info_block_botrt").Remove();

                string strViews = node.TextContent;

                strComments.Trim();
                strViews.Trim();


                _article.Views = Helper.ParseViews(strViews) + Helper.ParseViews(strComments) * 1025;
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {
                var node = fullArticle.QuerySelector("div.page_news").QuerySelector("div.page_news_info").QuerySelector("div.info_block_botrt");

                string strComments = node.QuerySelector("div.info_block_botrt").TextContent;

                node.QuerySelector("div.info_block_botrt").Remove();

                string strViews = node.TextContent;

                strComments.Trim();
                strViews.Trim();


                _article.Views = Helper.ParseViews(strViews) + Helper.ParseViews(strComments) * 1025;
            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string result = reducedArticle.QuerySelector("span.hidbl").TextContent;

            result = result.Replace("новости", "");
            result = result.Replace("Новости ", "");
            result = result.Replace("|", "");
            result = result.Replace("Игровые ", "Игры");
            result = result.Trim();


            if (result.Length < Settings.TagLength)
                _article.Tags.Add(new Tag(result));
        }
    }
}
