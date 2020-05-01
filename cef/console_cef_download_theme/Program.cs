// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using CefSharp.Handler;
using CefSharp.OffScreen;
using CefSharp.ResponseFilter;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks; 

namespace CefSharp.MinimalExample.OffScreen
{
    public class CefBasicRequestHandler : RequestHandler
    {
        readonly oSite m_site;
        public CefBasicRequestHandler(oSite site) : base() { m_site = site; }

        protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            string url = request.Url.ToLower();
            return new ExampleResourceRequestHandler(m_site);
            //return null;
        }
    }

    public class ExampleResourceRequestHandler : ResourceRequestHandler
    {
        readonly oSite m_site;
        public ExampleResourceRequestHandler(oSite site) : base() { m_site = site; }

        private MemoryStream memoryStream;
        protected override IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            var url = new Uri(request.Url);

            //Only called for our customScheme
            memoryStream = new MemoryStream();
            return new StreamResponseFilter(memoryStream);

            //return null;
        }

        protected override void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            string url = request.Url;
            var uri = new Uri(url);
            string type = response.MimeType;

            switch (type)
            {
                case "text/html":
                    break;
                case "text/css":
                case "text/javascript":
                case "application/javascript":
                    break;
                case "image/jpeg":
                case "image/gif":
                case "image/png":
                case "image/svg+xml":
                    break;
                case "font/woff2":
                    break;
                default:
                    Console.WriteLine(type, "\t", url);
                    break;
            }

            if (memoryStream != null)
            {
                var s = Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
    }

    public class oSite
    {
        public string NAME { get; set; }
        public Uri URL { get; set; }
        public string PATH_STORE { get; set; }
    }

    public class Program
    {
        private static ChromiumWebBrowser browser;
        private static oSite SITE;

        public static int Main(string[] args)
        {
            SITE = new oSite();

            if (args.Length == 0)
                args = new string[] { "https://pipeline.mediumra.re" };
            
            string URL = args[0];
            string PATH_STORE = DateTime.Now.ToString("yyMMdd-HHmmss");
            if (args.Length > 1) PATH_STORE = args[1];
            if (PATH_STORE.Contains(":") == false) PATH_STORE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PATH_STORE);
            if (Directory.Exists(PATH_STORE) == false) Directory.CreateDirectory(PATH_STORE);

            //Console.WriteLine(URL + " -> " + PATH_STORE + "\r\n");

            var uri = new Uri(URL);
            SITE.NAME = uri.AbsolutePath.Split('?')[0].Replace('/', '_').Trim().ToLower();
            SITE.URL = uri;
            SITE.PATH_STORE = PATH_STORE;

            var settings = new CefSettings() { CachePath = null };
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

            browser = new ChromiumWebBrowser(URL);
            browser.LoadingStateChanged += BrowserLoadingStateChanged;
            browser.RequestHandler = new CefBasicRequestHandler(SITE);

            Console.ReadKey();

            Cef.Shutdown();
            return 0;
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

                var scriptTask = browser.EvaluateScriptAsync("document.getElementById('lst-ib').value = 'CefSharp Was Here!'");

                scriptTask.ContinueWith(t =>
                {
                    //Give the browser a little time to render
                    Thread.Sleep(500);
                    // Wait for the screenshot to be taken.
                    var task = browser.ScreenshotAsync();
                    task.ContinueWith(x =>
                    {
                        // Make a file to save it to (e.g. C:\Users\jan\Desktop\CefSharp screenshot.png)
                        //var screenshotPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CefSharp screenshot.png");

                        string screenshotPath = Path.Combine(SITE.PATH_STORE, SITE.NAME + ".png");

                        //Console.WriteLine();
                        //Console.WriteLine("Screenshot ready. Saving to {0}", screenshotPath);

                        // Save the Bitmap to the path.
                        // The image type is auto-detected via the ".png" extension.
                        task.Result.Save(screenshotPath);

                        // We no longer need the Bitmap.
                        // Dispose it to avoid keeping the memory alive.  Especially important in 32-bit applications.
                        task.Result.Dispose();

                        //Console.WriteLine("Screenshot saved.  Launching your default image viewer...");

                        //// Tell Windows to launch the saved image.
                        //Process.Start(new ProcessStartInfo(screenshotPath)
                        //{
                        //    // UseShellExecute is false by default on .NET Core.
                        //    UseShellExecute = true
                        //});

                        //Console.WriteLine("Image viewer launched.  Press any key to exit.");
                    }, TaskScheduler.Default);
                });
            }
        }
    }
}