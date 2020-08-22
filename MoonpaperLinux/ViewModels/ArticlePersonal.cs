﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoonpaperLinux.Models;

namespace MoonpaperLinux.ViewModels
{
    public class ArticlePersonal : Article
    {
        public List<ArticleTagPersonal> ArticleTagPersonals { get; set; }
        public int SubscriptionRating { get; set; }
        public bool IsStar { get; set; }

        public ArticlePersonal(Article _article, List<UserTag> _userTags, bool _isStar)
        {
            ArticleTagPersonals = new List<ArticleTagPersonal>();

            Id = _article.Id;
            Title = _article.Title;
            Summary = _article.Summary;
            Body = _article.Body;
            Source = _article.Source;
            Url = _article.Url;
            UrlMainImg = _article.UrlMainImg;
            Views = _article.Views;
            Rating = _article.Rating;
            ArticleTags = _article.ArticleTags; //Можно удалить, проверив производительность
            DateTime = _article.DateTime;

            IsStar = _isStar;

            foreach (var at in ArticleTags)
            {
                var ut = _userTags.FirstOrDefault(user => user.TagId == at.TagId);

                int rating = 0;

                if (ut != null)
                { 
                    rating = ut.Rating;
                    SubscriptionRating += rating;
                }

                ArticleTagPersonal atp = new ArticleTagPersonal(at, rating);

                ArticleTagPersonals.Add(atp);
            }
        }
    }
}