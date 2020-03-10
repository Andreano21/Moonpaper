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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult All()
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
