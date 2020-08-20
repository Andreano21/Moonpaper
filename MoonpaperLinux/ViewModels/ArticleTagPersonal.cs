using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoonpaperLinux.Models;

namespace MoonpaperLinux.ViewModels
{
    public class ArticleTagPersonal : ArticleTag
    {
        public int Rating { get; set; }

        public ArticleTagPersonal(ArticleTag _articleTag, int _rating) 
        {
            ArticleId = _articleTag.ArticleId;
            Article = _articleTag.Article;
            TagId = _articleTag.TagId;
            Tag = _articleTag.Tag;

            Rating = _rating;
        }
    }
}
