using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp.WinForms;
using CefSharp;
using System.Threading;
namespace Boom
{
    class BrowserLoad
    {
        public void InitBrowser(string url, string sock5,int sleep)
        {
            Cef.EnableHighDPISupport();
            CefSettings cfsettings = new CefSettings();
            cfsettings.CefCommandLineArgs.Add("proxy-server", "socks5://" + sock5);
            cfsettings.UserAgent = "My/Custom/User-Agent-AndStuff";
            Cef.Initialize(cfsettings,performDependencyCheck: true, browserProcessHandler: null);

            ChromiumWebBrowser browser;
            browser = new ChromiumWebBrowser(url);
            Thread.Sleep(sleep);
            //browser.Dispose();
            //Cef.ShutdownWithoutChecks();


        }
    }
}
