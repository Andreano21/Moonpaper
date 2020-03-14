using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moonpaper.Models;
using Moonpaper.Data;
using Microsoft.EntityFrameworkCore;

namespace Moonpaper.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private ApplicationDbContext db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
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

        public IActionResult All()
        {
            var articles = db.Articles.Include(at => at.ArticleTags)
                                      .ThenInclude(t => t.Tag)
                                      .OrderByDescending(a => a.Views)
                                      .ToArray();
            
            return View(articles);
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
        public void TagUp(string UserId, int TagId)
        {
            var UserTagId = db.UserTags.FirstOrDefault(ut => ut.UserId == UserId && ut.TagId == TagId);
            UserTagId.Rating = 1;
            db.SaveChanges();
        }

        [HttpPost]
        public void TagDown(string UserId, int TagId)
        {
            var UserTagId = db.UserTags.FirstOrDefault(ut => ut.UserId == UserId && ut.TagId == TagId);
            UserTagId.Rating = -1;
            db.SaveChanges();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
