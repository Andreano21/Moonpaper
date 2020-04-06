using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Moonparser.Core
{
    public class Source
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int MaxViews { get; set; }
        public float AdminFactor { get; set; }

        public List<Article> Articles { get; set; }

        public Source()
        {
            Articles = new List<Article>();
            AdminFactor = 1f;
        }
    }
}
