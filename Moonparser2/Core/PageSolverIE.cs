using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Moonparser.Core
{
    class PageSolverIE
    {
        string result = null;

        public static string GetSolvedPage(string url, int workTimeMs)
        {
            PageSolverIE ps = new PageSolverIE();

            Task<string> t1 = Task.Run(async () => await ps.GetSolvedPageAsync(url, workTimeMs));

            return t1.Result;
        }

        public async Task<string> GetSolvedPageAsync(string url, int workTimeMs)
        {
            RunBrowserThread(url);

            //Время на обработку JS
            await Task.Delay(workTimeMs);

            return result;
        }

        void RunBrowserThread(string url)
        {
            var th = new Thread(() => {
                var br = new WebBrowser();
                br.DocumentCompleted += Browser_DocumentCompleted;
                br.ScriptErrorsSuppressed = true;
                br.Navigate(url);
                Application.Run();
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetCurrentProcess();

        void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var br = sender as WebBrowser;
            if (br.Url == e.Url)
            {
                result = br.DocumentText;

                //Исправляет баг: утечка памяти. Актуально только для IE7
                br.Dispose();
                br = null;
                IntPtr pHandle = GetCurrentProcess();
                SetProcessWorkingSetSize(pHandle, -1, -1);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                Application.ExitThread();   // Stops the thread
            }
        }
    }
}
