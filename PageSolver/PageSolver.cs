using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PageSolver
{
    class PageSolver
    {
        string result = null;

        public static string GetSolvedPage(string url)
        {
            PageSolver ps = new PageSolver();

            Task<string> t1 = Task.Run(async () => await ps.GetSolvedPageAsync(url));

            return t1.Result;
        }

        public async Task<string> GetSolvedPageAsync(string url)
        {
            runBrowserThread(url);

            //Время на обработку JS
            await Task.Delay(3000);

            return result;
        }

        void runBrowserThread(string url)
        {
            var th = new Thread(() => {
                var br = new WebBrowser();
                br.DocumentCompleted += browser_DocumentCompleted;
                br.ScriptErrorsSuppressed = true;
                br.Navigate(url);
                Application.Run();
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var br = sender as WebBrowser;
            if (br.Url == e.Url)
            {
                result = br.DocumentText;
                Application.ExitThread();   // Stops the thread
            }
        }
    }
}
