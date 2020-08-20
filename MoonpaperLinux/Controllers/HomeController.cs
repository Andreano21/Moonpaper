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
                        .OrderByDescending(a => a.Rating)
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

            //Список персоналезированных тегов
            List<ArticlePersonal> articlesPersonal = new List<ArticlePersonal>();

            //Получения списка тегов пользователя
            List<UserTag> userTags;
            userTags = db.UserTags.Where(ut => ut.UserId == _userManager.GetUserId(HttpContext.User)).ToList();

            foreach (var article in articles)
            {
                articlesPersonal.Add(new ArticlePersonal(article, userTags, false));
            }

            ViewBag.Articles = articlesPersonal;

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
                        .OrderByDescending(a => a.Rating)
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

            //Список персоналезированных тегов
            List<ArticlePersonal> articlesPersonal = new List<ArticlePersonal>();

            //Получения списка тегов пользователя
            List<UserTag> userTags;
            userTags = db.UserTags.Where(ut => ut.UserId == _userManager.GetUserId(HttpContext.User)).ToList();

            foreach (var article in articles)
            {
                articlesPersonal.Add(new ArticlePersonal(article, userTags, false));
            }

            //Получение статей исключая отписанные теги
            articlesPersonal = articlesPersonal.Where(ap => ap.SubscriptionRating > 0).ToList();

            int articlesToSkip = Page * Pages;

            articlesPersonal = articlesPersonal.Skip(articlesToSkip).Take(Pages).ToList();

            ViewBag.Articles = articlesPersonal;

            bool IsAjaxRequest = Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if (IsAjaxRequest)
            {
                return PartialView("_Articles");
            }

            return View();
        }

        public IActionResult Tag(string tag)
        {
            if (tag != null)
            {
                var curenttag = db.Tags.FirstOrDefault(t => t.TagValue == tag);

                var articles = db.Articles.Where(t => t.ArticleTags.Any(tt => tt.TagId == curenttag.Id))
                                          .Include(at => at.ArticleTags)
                                          .ThenInclude(t => t.Tag)
                                          .OrderByDescending(a => a.Views)
                                          .ToArray();

                return View(articles);
            }
            else
            { 
                return View();
            }
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
