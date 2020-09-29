using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moonparser.Core
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        
        [NotMapped]
        public string Body { get; set; }
        public Source Source { get; set; }
        public string Url { get; set; }
        public string UrlMainImg { get; set; }
        public int Views { get; set; }
        public int Stars { get; set; }
        public List<Tag> Tags { get; set; }
        public DateTime DateTime { get; set; }

        public Article()
        {
            Tags = new List<Tag>();
            Source = new Source();
        }

    }
}
