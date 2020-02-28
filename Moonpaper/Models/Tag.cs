using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moonpaper.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string TagValue { get; set; }
        public List<ArticleTag> ArticleTags { get; set; }
        public List<UserTag> UserTags { get; set; }

        public Tag()
        {
            ArticleTags = new List<ArticleTag>();
            UserTags = new List<UserTag>();
        }
    }
}
