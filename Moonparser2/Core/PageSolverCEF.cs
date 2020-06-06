using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;

namespace Moonparser.Core
{
    class PageSolverCEF
    {
        private readonly BlockingCollection<TaskResultCEF> blockingQueue = new BlockingCollection<TaskResultCEF>();

        private static PageSolverCEF _instance;

        private PageSolverCEF()
        {
            Task.Run(() => StartSolver());
        }

        private static readonly object _lock = new object();

        public static PageSolverCEF GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new PageSolverCEF();
                    }
                }
            }
            return _instance;
        }

        public async Task<string> GetSolvedPage(string url)
        {
            ManualResetEvent mre = new ManualResetEvent(false);

            TaskResultCEF tr = new TaskResultCEF(url, mre);

            if (!blockingQueue.TryAdd(tr))
            {
                Console.WriteLine("Невозможно добавить ссылку в очередь на обработку CEF");
            }

            await Task.Run(() => mre.WaitOne());

            return tr.value;
        }


        private static ChromiumWebBrowser browser;
        private static string html;

        private void StartSolver()
        {
            TaskResultCEF trCEF;

            using (CefSettings settings = new CefSettings())
            {
                settings.LogSeverity = LogSeverity.Disable;
                settings.CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache");
                Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            }

            browser = new ChromiumWebBrowser("https://www.google.com/");

            //Время на инициализацию браузера
            Thread.Sleep(3000);

            // An event that is fired when the first page is finished loading.
            // This returns to us from another thread.
            browser.LoadingStateChanged += BrowserLoadingStateChanged;

            //Цикл опроса очереди на наличие задачи для обработки
            while (!blockingQueue.IsCompleted)
            {
                if (blockingQueue.TryTake(out trCEF))
                {
                    browser.Load(trCEF.arg);
                    browser.LoadingStateChanged += BrowserLoadingStateChanged;

                    //Время на загрузку и обработку страницы
                    Thread.Sleep(3000);

                    trCEF.value = html;
                    trCEF.mre.Set();
                }
            }
        }

        private static void BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            // Check to see if loading is complete - this event is called twice, one when loading starts
            // second time when it's finished
            // (rather than an iframe within the main frame).
            if (!e.IsLoading)
            {
                // Remove the load event handler, because we only want one snapshot of the initial page.
                browser.LoadingStateChanged -= BrowserLoadingStateChanged;

                //Give the browser a little time to render
                Thread.Sleep(500);

                // Wait for the html to be taken.
                var task = browser.GetSourceAsync();

                task.ContinueWith(x =>
                {

                    html = task.Result;

                }, TaskScheduler.Default);
            }
        }
    }

    /// <summary>
    /// Класс служит для передачи url в очередь на обработку браузером, а также осуществляет обратный вызов с возвращением результата работы браузера
    /// </summary>
    /// <param name="_article">Статья(объект) которой присваивается название источника</param>
    /// <returns></returns>
    class TaskResultCEF
    {
        public string arg;
        public string value;
        public ManualResetEvent mre;

        public TaskResultCEF(string _arg, ManualResetEvent _mre)
        {
            arg = _arg;
            mre = _mre;
        }
    }
}
