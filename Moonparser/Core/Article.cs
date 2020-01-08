using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;


namespace Moonparser.Core
{
    class Article
    {
        public string Title = null;
        public string Summary = null;
        public string Body = null;
        public string Source = null;
        public string Url = null;
        public string UrlSource = null;
        public string UrlMainImg = null;

        public int Views = 0;

        public DateTime DateTime;

        public bool isFull()
        {
            if (Title != null && Summary != null && Body != null && Url != null && Source != null && UrlSource != null && UrlMainImg != null && Views != 0)
                return true;
            else
                return false;
        }
    }
}
