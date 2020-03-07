using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moonparser.Core
{
    public class Tag
    {
        public int Id { get; set; }
        public string TagValue { get; set; }
        public List<Article> Articles { get; set; }

        public Tag()
        {
            TagValue = "";
            Articles = new List<Article>();
        }

        public Tag(string _tag)
        {
            TagValue = "";
            TagValue = _tag;
            Articles = new List<Article>();
        }
    }
}
