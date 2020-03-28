using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;


namespace Moonpaper.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Body { get; set; }
        public Source Source { get; set; }
        public string Url { get; set; }
        public string UrlMainImg { get; set; }
        public int Views { get; set; }
        public List<ArticleTag> ArticleTags { get; set; }

        public DateTime DateTime { get; set; }

        public Article()
        {
            ArticleTags = new List<ArticleTag>();
            Source = new Source();
        }
    }
}
