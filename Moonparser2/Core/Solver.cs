using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonparser.Core
{
    static class Solver
    {
        static public void Solve()
        {
            using (AppContext db = new AppContext())
            {
                //List<Article> articles = db.Articles.ToList();
                List<Source> sources = db.Sources.ToList();

                db.Articles.Load();

                foreach (Source source in sources)
                {
                    int maxViews = 0;
                    foreach (Article article in source.Articles)
                    {
                        if (article.Views > maxViews)
                            maxViews = article.Views;
                    }
                    source.MaxViews = maxViews;
                }

                db.SaveChanges();
            }
        }
    }
}
