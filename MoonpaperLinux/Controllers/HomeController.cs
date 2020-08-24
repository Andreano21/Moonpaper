using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using MoonpaperLinux.Models;
using MoonpaperLinux.Data;
using Microsoft.EntityFrameworkCore;
using MoonpaperLinux.ViewModels;

namespace MoonpaperLinux.Controllers
{
    public class HomeController : Controller
    {
        UserManager<User> _userManager;

        private readonly ILogger<HomeController> _logger;

        private ApplicationDbContext db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _logger = logger;
            db = context;
        }

        public IActionResult Index() //Main
        {
            var articles = db.Articles.Include(at => at.ArticleTags)
                          .ThenInclude(t => t.Tag)
                          .OrderByDescending(a => a.Views)
                          .ToArray();

            return View(articles);
        }


        private List<ArticlePersonal> PreparePersonalArticles(List<Article> _articles, string _userId)
        {
            //Список персоналезированных тегов
            List<ArticlePersonal> articlesPersonal = new List<ArticlePersonal>();

            //Получение списка тегов пользователя
            List<UserTag> userTags;
            userTags = db.UserTags.Where(ut => ut.UserId == _userId).ToList();

            //Получение списка лайков пользователя
            List<Star> stars;
            stars = db.Stars.Where(s => s.UserId == _userId).ToList();

            //Получение списка источников пользователя
            List<UserSource> userSource;
            userSource = db.UserSource.Where(us => us.UserId == _userId).ToList();

            foreach (var article in _articles)
            {
                bool isStar = false;

                var ps = stars.FirstOrDefault(s => s.ArticleId == article.Id);

                if (ps != null)
                {
                    isStar = true;
                }

                int sourceRating = 0;

                var us = userSource.FirstOrDefault(us => us.SourceId == article.Source.Id);

                if (us != null)
                {
                    sourceRating = us.Rating;
                }

                articlesPersonal.Add(new ArticlePersonal(article, userTags, sourceRating, isStar));
            }

            return articlesPersonal;
        }

        public IActionResult All(string SortedBy, string Time, int Pages, int Page)
        {
            if (SortedBy == null)
                SortedBy = "views";

            if (Time == null)
                Time = "day";

            if (Pages == 0)
                Pages = 15;


            ViewBag.UserId = _userManager.GetUserId(HttpContext.User);
            ViewBag.SortedBy = SortedBy;
            ViewBag.Time = Time;
            ViewBag.Pages = Pages;
            ViewBag.Page = Page;


            List<Article> articles = null;

            switch (SortedBy)
            {
                case "time":
                    articles = db.Articles
                        .OrderByDescending(a => a.DateTime)
                        .ToList();

                    db.Sources.Load();
                    db.ArticleTag.Load();
                    db.Tags.Load();
                    break;

                case "views":
                    articles = db.Articles
                           .OrderByDescending(a => a.Views)
                           .ToList();

                    db.Sources.Load();
                    db.ArticleTag.Load();
                    db.Tags.Load();
                    break;

                case "rating":
                    articles = db.Articles
                        .OrderByDescending(a => a.Stars)
                        .ToList();

                    db.Sources.Load();
                    db.ArticleTag.Load();
                    db.Tags.Load();
                    break;
            }

            switch (Time)
            {
                case "day":
                    articles = articles.Where(a => a.DateTime > DateTime.Now.AddDays(-1d)).ToList();
                    break;

                case "week":
                    articles = articles.Where(a => a.DateTime > DateTime.Now.AddDays(-7d)).ToList();
                    break;

                case "month":
                    articles = articles.Where(a => a.DateTime > DateTime.Now.AddDays(-30d)).ToList();
                    break;
            }

            var articlesToSkip = Page * Pages;

            articles = articles.Skip(articlesToSkip).Take(Pages).ToList();

            ViewBag.ArticlePersonals = PreparePersonalArticles(articles, _userManager.GetUserId(HttpContext.User));

            bool IsAjaxRequest = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (IsAjaxRequest)
            {
                return PartialView("_Articles");
            }

            return View();
        }

        public IActionResult My(string SortedBy, string Time, int Pages, int Page)
        {
            if (SortedBy == null)
                SortedBy = "views";

            if (Time == null)
                Time = "day";

            if (Pages == 0)
                Pages = 15;


            ViewBag.UserId = _userManager.GetUserId(HttpContext.User);
            ViewBag.SortedBy = SortedBy;
            ViewBag.Time = Time;
            ViewBag.Pages = Pages;
            ViewBag.Page = Page;


            List<Article> articles = null;

            switch (SortedBy)
            {
                case "time":
                    articles = db.Articles
                        .OrderByDescending(a => a.DateTime)
                        .ToList();

                    db.Sources.Load();
                    db.ArticleTag.Load();
                    db.Tags.Load();
                    break;

                case "views":
                    articles = db.Articles
                           .OrderByDescending(a => a.Views)
                           .ToList();

                    db.Sources.Load();
                    db.ArticleTag.Load();
                    db.Tags.Load();
                    break;

                case "rating":
                    articles = db.Articles
                        .OrderByDescending(a => a.Stars)
                        .ToList();

                    db.Sources.Load();
                    db.ArticleTag.Load();
                    db.Tags.Load();
                    break;
            }

            switch (Time)
            {
                case "day":
                    articles = articles.Where(a => a.DateTime > DateTime.Now.AddDays(-1d)).ToList();
                    break;

                case "week":
                    articles = articles.Where(a => a.DateTime > DateTime.Now.AddDays(-7d)).ToList();
                    break;

                case "month":
                    articles = articles.Where(a => a.DateTime > DateTime.Now.AddDays(-30d)).ToList();
                    break;
            }

            List<ArticlePersonal> articlePersonals = PreparePersonalArticles(articles, _userManager.GetUserId(HttpContext.User));

            //Получение статей исключая отписанные теги
            articlePersonals = articlePersonals.Where(ap => ap.SubscriptionRating > 0).ToList();

            int articlesToSkip = Page * Pages;

            articlePersonals = articlePersonals.Skip(articlesToSkip).Take(Pages).ToList();

            ViewBag.ArticlePersonals = articlePersonals;

            bool IsAjaxRequest = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (IsAjaxRequest)
            {
                return PartialView("_Articles");
            }

            return View();
        }

        public IActionResult Tag(string Tag, string SortedBy, string Time, int Pages, int Page)
        {
            if (SortedBy == null)
                SortedBy = "views";

            if (Time == null)
                Time = "day";

            if (Pages == 0)
                Pages = 15;

            ViewBag.UserId = _userManager.GetUserId(HttpContext.User);
            ViewBag.Tag = Tag;
            ViewBag.SortedBy = SortedBy;
            ViewBag.Time = Time;
            ViewBag.Pages = Pages;
            ViewBag.Page = Page;

            List<Article> articles = null;

            if (Tag != null)
            {
                var curenttag = db.Tags.FirstOrDefault(t => t.TagValue == Tag);

                articles = db.Articles.Where(t => t.ArticleTags.Any(tt => tt.TagId == curenttag.Id))
                                          .OrderByDescending(a => a.Views)
                                          .ToList();

                db.Sources.Load();
                db.ArticleTag.Load();
                db.Tags.Load();
            }

            switch (SortedBy)
            {
                case "time":
                    articles = articles
                        .OrderByDescending(a => a.DateTime)
                        .ToList();
                    break;

                case "views":
                    articles = articles
                           .OrderByDescending(a => a.Views)
                           .ToList();
                    break;

                case "rating":
                    articles = articles
                            .OrderByDescending(a => a.Stars)
                            .ToList();
                    break;
            }

            switch (Time)
            {
                case "day":
                    articles = articles.Where(a => a.DateTime > DateTime.Now.AddDays(-1d)).ToList();
                    break;

                case "week":
                    articles = articles.Where(a => a.DateTime > DateTime.Now.AddDays(-7d)).ToList();
                    break;

                case "month":
                    articles = articles.Where(a => a.DateTime > DateTime.Now.AddDays(-30d)).ToList();
                    break;
            }

            var articlesToSkip = Page * Pages;

            articles = articles.Skip(articlesToSkip).Take(Pages).ToList();

            ViewBag.ArticlePersonals = PreparePersonalArticles(articles, _userManager.GetUserId(HttpContext.User));

            bool IsAjaxRequest = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (IsAjaxRequest)
            {
                return PartialView("_Articles");
            }

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void SourceUp(string UserId, int SourceId)
        {
            var UserSourceId = db.UserSource.FirstOrDefault(us => us.UserId == UserId && us.SourceId == SourceId);

            if (UserSourceId != null)
            {
                UserSourceId.Rating = 1;
            }
            else
            {
                UserSource us = new UserSource();
                us.UserId = UserId;
                us.SourceId = SourceId;
                us.Rating = 1;
                db.UserSource.Add(us);
            }

            db.SaveChanges();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void SourceDown(string UserId, int SourceId)
        {
            var UserSourceId = db.UserSource.FirstOrDefault(us => us.UserId == UserId && us.SourceId == SourceId);

            if (UserSourceId != null)
            {
                UserSourceId.Rating = -1;
            }
            else
            {
                UserSource us = new UserSource();
                us.UserId = UserId;
                us.SourceId = SourceId;
                us.Rating = -1;
                db.UserSource.Add(us);
            }

            db.SaveChanges();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void TagUp(string UserId, int TagId)
        {
            var UserTagId = db.UserTags.FirstOrDefault(ut => ut.UserId == UserId && ut.TagId == TagId);

            if (UserTagId != null)
            { 
                UserTagId.Rating = 1;
            }
            else
            {
                UserTag ut = new UserTag();
                ut.UserId = UserId;
                ut.TagId = TagId;
                ut.Rating = 1;
                db.UserTags.Add(ut);
            }

            db.SaveChanges();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void TagDown(string UserId, int TagId)
        {
            var UserTagId = db.UserTags.FirstOrDefault(ut => ut.UserId == UserId && ut.TagId == TagId);

            if (UserTagId != null)
            {
                UserTagId.Rating = -1;
            }
            else
            {
                UserTag ut = new UserTag();
                ut.UserId = UserId;
                ut.TagId = TagId;
                ut.Rating = -1;
                db.UserTags.Add(ut);
            }

            db.SaveChanges();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void StarUp(string UserId, int ArticleId)
        {
            var star = db.Stars.FirstOrDefault(s => s.UserId == UserId && s.ArticleId == ArticleId);

            if (star == null)
            {
                Star s = new Star();
                s.UserId = UserId;
                s.ArticleId = ArticleId;
                db.Stars.Add(s);
                db.Articles.FirstOrDefault(a => a.Id == ArticleId).Stars++;

                db.SaveChanges();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void StarDown(string UserId, int ArticleId)
        {
            var star = db.Stars.FirstOrDefault(s => s.UserId == UserId && s.ArticleId == ArticleId);

            if (star != null)
            {
                db.Stars.Remove(star);
                db.Articles.FirstOrDefault(a => a.Id == ArticleId).Stars--;

                db.SaveChanges();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
