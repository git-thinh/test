using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

namespace tests
{
    public class clsGlobal : System.Web.HttpApplication
    {
        static ICache m_cache;
        static IApi m_api;

        #region [ JOB ]

        static IScheduler m_scheduler;
        static ConcurrentDictionary<string, IJobDetail> m_job = new ConcurrentDictionary<string, IJobDetail>();
        static ConcurrentDictionary<string, ITrigger> m_trigger = new ConcurrentDictionary<string, ITrigger>();

        static void _init_job()
        {
            NameValueCollection configuration = new NameValueCollection
            {
                { "quartz.jobStore.type", "Quartz.Simpl.RAMJobStore, Quartz" },
                { "quartz.threadPool.threadCount", "3" },
                { "quartz.serializer.type", "binary" }
            };
            ISchedulerFactory factory = new StdSchedulerFactory(configuration);
            m_scheduler = factory.GetScheduler().GetAwaiter().GetResult();
            //m_scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();
            m_scheduler.ListenerManager.AddJobListener(new JobListener());
            m_scheduler.Start().Wait();
            m_scheduler.Context.Put("ILOG___", new LogRedis());
            m_scheduler.Context.Put("SCOPE_NAME___", "CKV_IIS");
        }

        public void clearAllJobs() {
            m_scheduler.Shutdown();
            m_scheduler.Start();
            m_job.Clear();
            m_trigger.Clear();
        }

        public static string[] list_jobs() {
            var a = m_scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup())
                .GetAwaiter().GetResult().Select(x=>x.Name).ToArray();
            return a;
        }

        public static void resume_job(string job_name)
        {
            if (m_scheduler.CheckExists(new JobKey(job_name)).GetAwaiter().GetResult())
                m_scheduler.ResumeJob(new JobKey(job_name)).GetAwaiter().GetResult();
        }

        public static void pause_job(string job_name)
        {
            if (m_scheduler.CheckExists(new JobKey(job_name)).GetAwaiter().GetResult())
                m_scheduler.PauseJob(new JobKey(job_name)).GetAwaiter().GetResult();
        }

        public static bool remove_job(string job_name)
        {
            if (m_scheduler.CheckExists(new JobKey(job_name)).GetAwaiter().GetResult())
            {
                //var ok = m_scheduler.Interrupt(new JobKey(job_name)).GetAwaiter().GetResult();
                var ok = m_scheduler.DeleteJob(new JobKey(job_name)).GetAwaiter().GetResult();
                if (ok)
                {
                    IJobDetail j;
                    ITrigger t;
                    m_job.TryRemove(job_name, out j);
                    m_trigger.TryRemove(job_name, out t);
                }
                return ok;
            }
            return false;
        }

        public static bool exist_job(string job_name) { return m_job.ContainsKey(job_name); }

        public static string create_job(JOB_TYPE type, string group_name, Dictionary<string, object> para = null, string schedule = null)
        {
            if (para == null) para = new Dictionary<string, object>() { };
            group_name = group_name.ToLower();
            string name = group_name + "." + DateTime.Now.ToString("yyMMdd-HHmmss-fff");
            if (!string.IsNullOrEmpty(schedule)) name += "." + schedule;

            JobDataMap m = new JobDataMap();
            m.Put("ID___", name);
            m.Put("SCHEDULE___", schedule);
            m.Put("TYPE___", type);
            m.Put("CURRENT_ID___", 0);
            m.Put("COUNTER___", new ConcurrentDictionary<long, bool>());
            m.Put("PARA___", para);

            JobBuilder job = null;
            TriggerBuilder trigger = null;

            switch (type)
            {
                case JOB_TYPE.CRAWLER_NET:
                case JOB_TYPE.CRAWLER_CURL:
                    job = JobBuilder.Create<JobCrawler>();
                    break;
                case JOB_TYPE.API_JS:
                    job = JobBuilder.Create<JobApiJS>();
                    break;
            }

            if (job != null)
            {
                job = job.WithIdentity(name, group_name).UsingJobData(m);
                trigger = TriggerBuilder.Create();

                if (!string.IsNullOrEmpty(schedule))
                    trigger = trigger.WithSchedule(CronScheduleBuilder.CronSchedule(schedule));
                trigger = trigger.StartNow();

                var j = job.Build();
                var t = trigger.Build();

                m_scheduler.ScheduleJob(j, t);

                m_job.TryAdd(name, j);
                m_trigger.TryAdd(name, t);

                m_scheduler.ScheduleJob(j, t).Wait();
                return name;
            }

            return string.Empty;
        }

        #endregion

        protected void Application_Start(object sender, EventArgs e)
        {
            _init_job();
            _CONFIG._init();
            m_cache = new clsCache();
            m_api = new clsApi(m_cache);
            clsEngineJS._init(m_api);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Uri uri = Request.Url;

            string path = uri.AbsolutePath.Substring(1);
            if (path == "favicon.ico") { Response.End(); return; }
            string[] a = path.Split('/');

            string domain = uri.Host;
            if (domain == "localhost" || domain == "127.0.0.1") domain = _CONFIG.DOMAIN_LOCALHOST;

            //[1] api/{cache_name}/{api_name}/{id}
            if (a[0] == "api" && a.Length > 2)
            {
                Response.ContentType = "application/json";
                if (m_cache.existCache_ApiJS(a[1], a[2]))
                {
                    Response.Write(JsonConvert.SerializeObject(oResult.Error("Cannot found cache name: " + a[1])));
                    Response.End();
                    return;
                }

                oResult rv = null;
                oResult rp = get_api_parameters();
                if (rp.ok)
                    rv = clsEngineJS.Execute(a[1] + "___" + a[2], (Dictionary<string, object>)rp.data, null);
                else
                    rv = rp;

                Response.Write(JsonConvert.SerializeObject(rv));
                Response.End();
                return;
            }

            //[2] Files
            string file = string.Empty;
            bool isHome = false;
            if (path.Length == 0) { isHome = true; path = "index.html"; }
            if (path.EndsWith(".html")) path = "_site\\" + domain + "\\" + path;
            if (path == "admin") path = "admin.html";

            string contentType = MimeMapping.GetMimeMapping(path);
            if (path[0] != '_' // break dirs: _lib, _view,..
                && contentType != "text/html" // break files *.html
                && Request.UrlReferrer != null) // Ref from other request as request on page html
                path = "_site\\" + domain + "\\" + path;

            file = Path.Combine(_CONFIG.PATH_ROOT, path).Replace("/", "\\");

            if (File.Exists(file))
            {
                Response.ContentType = contentType;
                Response.TransmitFile(file);
            }
            else
            {
                if (isHome)
                {
                    Response.ContentType = contentType;
                    Response.Write("<h1>Cannot found page " + domain + "</h1>");
                }
                else
                    Response.StatusCode = 404;
            }

            Response.End();
        }

        oResult get_api_parameters()
        {
            oResult r = oResult.Error();

            string path = Request.Url.AbsolutePath.Substring(1).ToLower();
            Dictionary<string, object> para = new Dictionary<string, object>() {
                { "___domain", Request.Url.Host },
                { "___port", Request.Url.Port },
                { "___url", path },
                { "___method", Request.HttpMethod },
                { "___token", string.Empty },
            };

            try
            {
                var a = Request.QueryString.Keys.Cast<string>().ToArray();
                foreach (var key in a) if (!para.ContainsKey(key)) para.Add(key, Uri.UnescapeDataString(Request.QueryString[key]));

                if (Request.HttpMethod == "POST")
                {
                    string s = new StreamReader(Request.InputStream).ReadToEnd();
                    if (s == null || string.IsNullOrWhiteSpace(s))
                        return oResult.Error("Body of POST is not null or emtpy");
                    else
                    {
                        s = s.Trim();
                        if (s.Length > 1 && s[0] == '{' && s[s.Length - 1] == '}')
                        {
                            try
                            {
                                var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(s);
                                foreach (var kv in d) para.Add(kv.Key, kv.Value);
                            }
                            catch (Exception e)
                            {
                                return oResult.Error("Convert to json of body error: " + e.Message);
                            }
                        }
                        else
                        {
                            para.Add("___BODY", s);
                        }
                    }
                }

                r.ok = true;
                r.data = para;
            }
            catch (Exception e)
            {
                r.error = "ERROR[clsRouter.get_api_parameters()] " + e.Message;
            }

            return r;
        }
    }
}