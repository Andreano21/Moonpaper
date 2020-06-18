using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Moonparser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moonparser.NewsSources
{
    class InmyroomParser : Parser
    {
        protected override void SetSettings()
        {
            startUrls = new string[] { "https://www.inmyroom.ru/posts" };
            pageSolverType = PageSolverType.Not;
            sourceName = "inmyroom.ru";
            sourceUrl = "https://www.inmyroom.ru";

        }
        protected override IEnumerable<IElement> GetItems()
        {
            //Получение тематических блоков
            List<IElement> items = new List<IElement>();

            for (int d = 0; d < documents.Length; d++)
            {
                var v1 = documents[d].QuerySelector("div.pb-Section");
                var v2 = v1.QuerySelector("div.b-InfinityList");
                var v3 = v2.QuerySelector("div.b-Layout__content_narrow");
                var v4 = v3.QuerySelectorAll("a.s-PostPreview");

                items.AddRange(v4);
            }

            return items;
        }

        protected override void GetUrl(Article _article, IElement reducedArticle)
        {
            _article.Url = reducedArticle.Attributes["href"].Value;
        }

        protected override void GetBody(Article _article, IHtmlDocument fullArticle)
        {
            var v1 = fullArticle.QuerySelector("div.b-Article");
            var v2 = v1.QuerySelector("div.b-Article_container");
            var v3 = v2.QuerySelector("div.b-Article_content");

            _article.Body = v3.TextContent;
        }

        protected override void GetTitle(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            string str = reducedArticle.QuerySelector("div.s-PostPreview_b-Content").QuerySelector("div.s-PostPreview_b-Title").TextContent;

            _article.Title = str;
        }

        protected override void GetSummary(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var v1 = fullArticle.QuerySelector("div.b-Article");
            var v2 = v1.QuerySelector("div.b-Article_container");
            var v3 = v2.QuerySelector("div.b-Article_cover");
            var v4 = v3.QuerySelector("div.s-ArticleCover_b-main");
            var v5 = v4.QuerySelector("div.s-ArticleCover_b-announce");
            var v6 = v5.QuerySelector("p");

            _article.Summary = v6.TextContent;
        }

        protected override void GetUrlMainImg(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var v1 = fullArticle.QuerySelector("div.b-Article");
            var v2 = v1.QuerySelector("div.b-Article_container");
            var v3 = v2.QuerySelector("div.b-Article_cover");
            var v4 = v3.QuerySelector("div.s-ArticleCover_b-teaser");
            var v5 = v4.QuerySelector("img.s-ArticleCover_b-teaser_img");

            string imgurl = v5.Attributes["src"].Value;

            _article.UrlMainImg = imgurl;
        }

        protected override void GetDateTime(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            var v1 = fullArticle.QuerySelector("div.b-Article");
            var v2 = v1.QuerySelector("div.b-Article_container");
            var v3 = v2.QuerySelector("div.b-Article_cover");
            var v4 = v3.QuerySelector("div.s-ArticleCover_b-main");
            var v5 = v4.QuerySelector("[itemprop = dateModified]");

            string dateSource = v5.Attributes["content"].Value;

            _article.DateTime = DateTime.Parse(dateSource);
        }

        protected override void GetViews(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //Загрузка данных из источника по умолчанию
            if (reducedArticle != null && fullArticle != null)
            {
                var v2 = fullArticle.QuerySelector("div.b-Article_container");
                var v3 = v2.QuerySelector("div.b-Article_cover");
                var v4 = v3.QuerySelector("div.s-ArticleCover_b-main");
                var v5 = v4.QuerySelector("div.s-ArticleCover_b-Info");


                //string views = v5.QuerySelector("div.s-ArticleCover_b-Info_counters").QuerySelector("div.b-Counter__views").TextContent;

                //string comments = v5.QuerySelector("div.s-ArticleCover_b-Info_counters").QuerySelector("div.b-Counter b-Counter__comments").TextContent;

                string likes = v5.QuerySelector("div.s-ArticleCover_b-Info_rating").QuerySelector("div.b-ReviewsStat_count").TextContent;

                likes = likes.Replace(" отзыва", "");
                likes = likes.Replace("– ", "");

                int likesParsed = Helper.ParseViews(likes);

                if (likesParsed != 0)
                { 
                    _article.Views = likesParsed * 1346;
                }
                else
                {
                    Random rnd = new Random();
                    _article.Views = rnd.Next(350, 1345);
                }
            }
            //Загрузка данных из полноценной страницы статьи
            else if (reducedArticle == null)
            {

            }
            //Загрузка данных из сокращенной страницы
            else
            {

            }
        }

        protected override void GetTags(Article _article, IElement reducedArticle, IHtmlDocument fullArticle)
        {
            //var v1 = fullArticle.QuerySelector("div.b-Article__standart");
            //var v2 = v1.QuerySelector("div.b-Article_bottom");
            //var v3 = v2.QuerySelector("div.b-Article_tags");

            var v3 = fullArticle.QuerySelector("div.b-Article_tags");

            var tags = v3.QuerySelectorAll("a");


            string[] poorTag = new string[] { "Более 90 метров", "Проект недели", "INMYROOM TV", "Советы", "Все теги", "handmade декор", "бюджетный декор", "Панельный дом", "2 комнаты", "40-60 метров", "4 и больше", "Более 90 метров" };
            List<string> goodTags = new List<string>();

            foreach (var tag in tags)
            {
                string result = tag.TextContent;

                if (result.Length < Settings.TagLength)
                    goodTags.Add(result);
            }

            goodTags = Helper.TagCheker(goodTags.ToArray(), poorTag);

            foreach (var tag in goodTags)
            {
                _article.Tags.Add(new Tag(tag));
            }
        }
    }
}
