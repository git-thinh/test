using CefSharp.Handler;
using CefSharp.OffScreen;
using CefSharp.ResponseFilter;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CefSharp.MinimalExample.OffScreen
{
    public class Utility {
        public static string build_path_folder_save(Uri uri)
        {
            //var uri = new Uri(URL);
            //string PATH_STORE = DateTime.Now.ToString("yyMMdd-HHmmss");
            string PATH_STORE = uri.Host.Replace(':', '_');
            //if (args.Length > 1) PATH_STORE = args[1];
            if (PATH_STORE.Contains(":") == false) PATH_STORE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PATH_STORE);
            if (Directory.Exists(PATH_STORE) == false) Directory.CreateDirectory(PATH_STORE);
            return PATH_STORE;
        }

        public static string build_path_file_save(Uri uri)
        {
            string PATH_STORE = build_path_folder_save(uri);
            string path = uri.AbsolutePath == "/" ? "index.html" : uri.AbsolutePath.Replace("/", "\\");
            if (path[0] == '\\') path = path.Substring(1);
            string file = Path.Combine(PATH_STORE, path);
            string[] a = file.Split('\\');
            string dir = string.Join("\\", a.Where((x, i) => i < a.Length - 1));
            if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);
            //Console.WriteLine(dir);
            if (file.EndsWith("\\index.html")) file = file.Substring(0, file.Length - "\\index.html".Length) + "\\_index.html";
            return file;
        }
    }

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
            string url_file = url.Split('?')[0];
            var uri = new Uri(url);
            string type = response.MimeType;

            string file = Utility.build_path_file_save(uri);

            byte[] buf = null;
            if (memoryStream != null) buf = memoryStream.ToArray();
            bool ok = false;

            switch (type)
            {
                case "text/html":
                    if (file.EndsWith(".html") == false) file += ".html";
                    //Console.WriteLine("OK: " + type + " -> " + url_file + " => " + file);
                    if (buf != null)
                    {
                        ok = true;
                        File.WriteAllText(file, Encoding.UTF8.GetString(buf).Replace("index.html", "_index.html"));
                    }
                    break;
                case "text/css":
                    if (file.EndsWith(".css") == false) file += ".css";
                    //Console.WriteLine("OK: " + type + " -> " + url_file + " => " + file);
                    if (buf != null)
                    {
                        ok = true;
                        File.WriteAllText(file, Encoding.UTF8.GetString(buf));
                    }
                    break;
                case "text/javascript":
                case "application/javascript":
                    if (file.EndsWith(".js") == false) file += ".js";
                    //Console.WriteLine("OK: " + type + " -> " + url_file + " => " + file);
                    if (buf != null)
                    {
                        ok = true;
                        File.WriteAllText(file, Encoding.UTF8.GetString(buf));
                    }
                    break;
                case "image/jpeg":
                    if (file.EndsWith(".jpg") == false) file += ".jpg";
                    //Console.WriteLine("OK: " + type + " -> " + url_file + " => " + file);
                    if (buf != null)
                    {
                        ok = true;
                        File.WriteAllBytes(file, buf);
                    }
                    break;
                case "image/jpg":
                    if (file.EndsWith(".jpg") == false) file += ".jpg";
                    //Console.WriteLine("OK: " + type + " -> " + url_file + " => " + file);
                    if (buf != null)
                    {
                        ok = true;
                        File.WriteAllBytes(file, buf);
                    }
                    break;
                case "image/gif":
                    if (file.EndsWith(".gif") == false) file += ".gif";
                    //Console.WriteLine("OK: " + type + " -> " + url_file + " => " + file);
                    if (buf != null)
                    {
                        ok = true;
                        File.WriteAllBytes(file, buf);
                    }
                    break;
                case "image/png":
                    if (file.EndsWith(".png") == false) file += ".png";
                    //Console.WriteLine("OK: " + type + " -> " + url_file + " => " + file);
                    if (buf != null)
                    {
                        ok = true;
                        File.WriteAllBytes(file, buf);
                    }
                    break;
                case "image/svg+xml":
                    if (file.EndsWith(".svg") == false) file += ".svg";
                    //Console.WriteLine("OK: " + type + " -> " + url_file + " => " + file);
                    if (buf != null)
                    {
                        ok = true;
                        File.WriteAllBytes(file, buf);
                    }
                    break;
                case "font/woff2":
                    if (file.EndsWith(".woff2") == false) file += ".woff2";
                    //Console.WriteLine("OK: " + type + " -> " + url_file + " => " + file);
                    if (buf != null)
                    {
                        ok = true;
                        File.WriteAllBytes(file, buf);
                    }
                    break;
                default:
                    //Console.WriteLine("NO: " + type + " -> " + url);
                    // The number of bytes read
                    try
                    {
                        new WebClient().DownloadFile(url, file);
                        ok = true;
                    }
                    catch { }

                    break;
            }

            if (ok)
                m_site.LOG_OK.Append(type + " -> " + url + ": " + Path.GetFileName(url_file) + " | " + file + Environment.NewLine);
            else
                m_site.LOG_ERROR.Append(type + " -> " + url + ": " + Path.GetFileName(url_file) + " | " + file + Environment.NewLine);
        }
    }

    public class oSite
    {
        public string NAME { get; set; }
        public Uri URL { get; set; }
        public string PATH_STORE { get; set; }

        public StringBuilder LOG_OK = new StringBuilder();
        public StringBuilder LOG_ERROR = new StringBuilder();
    }

    public class Program
    {
        private static ChromiumWebBrowser browser;
        private static oSite SITE;
        private static ManualResetEvent signal = new ManualResetEvent(false);
        static string URL_NEXT = null;

        public static int Main(string[] args)
        {
            SITE = new oSite();

            if (args.Length == 0)
                args = new string[] {
                    //"https://pipeline.mediumra.re",
                    //"https://pipeline.mediumra.re/pages-app.html",
                    "https://pipeline.mediumra.re/nav-side-kanban-board.html",
                    //Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pipeline.mediumra.re")
                };

            string URL = args[0];
            Uri uri = new Uri(URL);
            string PATH_STORE = Utility.build_path_folder_save(uri);
            string file = Utility.build_path_file_save(uri);
            Console.Title = URL + " -> " + file;

            if (File.Exists(file))
            {
                string f_links = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, uri.Host.Replace(':', '_') + ".txt");
                if (File.Exists(f_links))
                {
                    var lines = File.ReadAllLines(f_links).Where(o => o.Trim().Length > 0).ToList();
                    if (lines.Count > 0)
                    {
                        int index = lines.FindIndex(x => x == URL);
                        if (index == -1)
                            URL_NEXT = lines[0];
                        else if (index < lines.Count - 1) 
                            URL_NEXT = lines[index + 1];

                        if (!string.IsNullOrEmpty(URL_NEXT))
                            return Main(new string[] { URL_NEXT });
                    }
                }
            }


            SITE.NAME = uri.AbsolutePath.Split('?')[0].Replace('/', '_').Trim().ToLower();
            if (SITE.NAME[0] == '_') SITE.NAME = SITE.NAME.Substring(1);
            if (SITE.NAME.Length == 0) SITE.NAME = "index.html";
            SITE.URL = uri;
            SITE.PATH_STORE = PATH_STORE;

            var settings = new CefSettings() { CachePath = null };
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

            browser = new ChromiumWebBrowser(URL);
            browser.Size = new System.Drawing.Size(1366, 3500);
            browser.LoadingStateChanged += BrowserLoadingStateChanged;
            browser.RequestHandler = new CefBasicRequestHandler(SITE);

            //Console.ReadKey();
            signal.WaitOne();

            Cef.Shutdown();

            if (string.IsNullOrEmpty(URL_NEXT))
                MessageBox.Show("Download theme: " + SITE.URL.ToString() + " complete");
            else
            {
                Process.Start(Application.ExecutablePath, URL_NEXT);
            }

            Environment.Exit(0);
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

                //var scriptTask = browser.EvaluateScriptAsync("document.getElementById('lst-ib').value = 'CefSharp Was Here!'");
                var scriptTask = browser.EvaluateScriptAsync("var el = document.getElementById('lst-ib'); if(el) el.value = 'CefSharp Was Here!'");

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

                        string path = Path.Combine(SITE.PATH_STORE, "_log");
                        if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
                        string log = Path.Combine(SITE.PATH_STORE, "log\\" + SITE.NAME + ".txt");
                        string s = SITE.LOG_OK.ToString() +
                            Environment.NewLine +
                            "--------------------" +
                            Environment.NewLine +
                            "ERROR:" +
                            Environment.NewLine +
                            SITE.LOG_ERROR.ToString();
                        File.WriteAllText(log, SITE.NAME + " -> " + SITE.URL.ToString() + "\r\n\r\n" + s + "\r\n[DONE]");
                        //Console.WriteLine(s);
                        Console.WriteLine("[DONE]");

                        const string scriptet = @"(function() { 
                                var rs = [];
                                var links = document.querySelectorAll('a');
                                links.forEach(a=>{
                                    if(a.hasAttribute('href')) rs.push(a.getAttribute('href'));
                                });
                                return rs.join('^');
                            })();
                        ";

                        browser.GetMainFrame().EvaluateScriptAsync(scriptet).ContinueWith(y =>
                        {
                            var response = y.Result;
                            if (response.Success && response.Result != null)
                            {
                                var links = response.Result.ToString()
                                .Split('^')
                                .Select(o => o.Trim())
                                .Where(o => o.Length > 0 && o[0] != '#')
                                .Select(o => o.Trim().Split('?')[0].Split('#')[0])
                                .Where(o => o.Length > 3)
                                .Select(o => o[0] == '/' ? o.Substring(1) : o)
                                .Distinct()
                                .Select(o => o.StartsWith("http") ? o : SITE.URL.Scheme + "://" + SITE.URL.Host + "/" + o)
                                .Select(o => o.Replace("/../", "/"))
                                .Where(o => o != SITE.URL.ToString())
                                .Where(o => o.Contains("/" + SITE.URL.Host + "/"))
                                .ToList();

                                links.Insert(0, SITE.URL.ToString());

                                string f_links = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SITE.URL.Host.Replace(':', '_') + ".txt");
                                if (File.Exists(f_links)) links.AddRange(File.ReadAllLines(f_links).Where(o => o.Trim().Length > 0));

                                var a = links.Distinct().ToArray();
                                File.WriteAllLines(f_links, a);

                                if (a.Length > 1) 
                                    URL_NEXT = a[1];
                            }
                        });

                        signal.Set();

                        //Console.WriteLine("Image viewer launched.  Press any key to exit.");
                    }, TaskScheduler.Default);
                });
            }
        }
    }
}