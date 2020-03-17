using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Moonpaper.Models;
using Moonpaper.Data;
using Microsoft.EntityFrameworkCore;

namespace Moonpaper.Controllers
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

        //public IActionResult All()
        //{
        //    ViewBag.UserId = _userManager.GetUserId(HttpContext.User);
        //    ViewBag.SotredBy = "views";
        //    ViewBag.Time = "day";
        //    ViewBag.Pages = 25;
        //    ViewBag.Articles = db.Articles.Include(at => at.ArticleTags)
        //                              .ThenInclude(t => t.Tag)
        //                              .OrderByDescending(a => a.Views)
        //                              .ToArray();
        //    return View();
        //}

        public IActionResult All(string SortedBy, string Time, int Pages)
        {
            if (SortedBy == null)
                SortedBy = "time";

            if (Time == null)
                Time = "day";

            if (Pages == 0)
                Pages = 25;


            ViewBag.UserId = _userManager.GetUserId(HttpContext.User);
            ViewBag.SortedBy = SortedBy;
            ViewBag.Time = Time;
            ViewBag.Pages = Pages;


            List<Article> articles = null;

            switch (SortedBy)
            {
                case "time":
                    articles = db.Articles.Include(at => at.ArticleTags)
                           .ThenInclude(t => t.Tag)
                           .OrderByDescending(a => a.DateTime)
                           .ToList();
                    break;

                case "views":
                    articles = db.Articles.Include(at => at.ArticleTags)
                           .ThenInclude(t => t.Tag)
                           .OrderByDescending(a => a.Views)
                           .ToList();
                    break;

                case "rating":
                    articles = db.Articles.Include(at => at.ArticleTags)
                        .ThenInclude(t => t.Tag)
                        .OrderByDescending(a => a.Views)
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

            articles = articles.Take(Pages).ToList();


            ViewBag.Articles = articles;

            return View();
        }

        public IActionResult My()
        {
            var articles = db.Articles.Include(at => at.ArticleTags)
                                      .ThenInclude(t => t.Tag)
                                      .OrderByDescending(a => a.Views)
                                      .ToArray();

            return View(articles);
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
