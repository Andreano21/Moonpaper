using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace Moonparser.Core
{
    abstract class NewParser
    {
        protected List<Article> articles = new List<Article>();
        protected string[] startUrl = null;
        protected string[] sources = null;
        protected IHtmlDocument[] documents = null;
        protected string source = null;
        protected IHtmlDocument document = null;

        //Установка ссылок на первичные страницы
        protected abstract void GetStartUrl();

        protected abstract IEnumerable<IElement> GetItems();
        protected abstract void GetUrl(Article _article, IElement _item);
        protected abstract void GetBody(Article _article, IHtmlDocument _document);
        protected abstract void GetTitle(Article _article, IElement _item, IHtmlDocument _document);
        protected abstract void GetSummary(Article _article, IElement _item, IHtmlDocument _document);
        protected abstract void GetSource(Article _article);
        protected abstract void GetUrlSource(Article _article);
        protected abstract void GetUrlMainImg(Article _article, IElement _item, IHtmlDocument _document);
        protected abstract void GetDateTime(Article _article);
        protected abstract void GetViews(Article _article, IElement _item, IHtmlDocument _document);
        protected abstract void GetTags(Article _article, IHtmlDocument _document);

        public async Task ParseAsync(List<Article> _articles)
        {
            ParseBasePage(_articles);

            foreach (Article article in _articles)
            {
                ParsePage(article);
            }   

        }

        //Получает список статей с базовой страницы, а также может заполнять некоторые поля статьи
        private void ParseBasePage(List<Article> _articles)
        { 
            
        }

        //Заполняет все поля статьи полученные из конкретной страницы
        private void ParsePage(Article article)
        {

        }

        //Обновляет данные статьи (просмотры, лайки, и т.п.)
        public async Task UpdateArticleAsync(Article article)
        {

        }
    }
}
