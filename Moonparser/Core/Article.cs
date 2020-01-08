﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;


namespace Moonparser.Core
{
    class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Body { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public string UrlSource { get; set; }
        public string UrlMainImg { get; set; }

        public int Views { get; set; }

        public DateTime DateTime { get; set; }

        public bool isFull()
        {
            if (Title != null && Summary != null && Body != null && Url != null && Source != null && UrlSource != null && UrlMainImg != null && Views != 0)
                return true;
            else
                return false;
        }
    }
}
