﻿using Microsoft.ClearScript.V8;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ZaloCSharpSDK;
using ZaloDotNetSDK;

namespace weblib
{
    public class _CONFIG
    {
        const int LOG_PORT_REDIS_DEFAULT = 11111;

        public static string PATH_ROOT = string.Empty;
        public static int LOG_PORT_REDIS = LOG_PORT_REDIS_DEFAULT;
        public static string PATH_DATA_FILE = string.Empty;

        public static void _init(System.Web.HttpApplication app, NameValueCollection appSettings)
        {
            PATH_ROOT = AppDomain.CurrentDomain.BaseDirectory;

            PATH_DATA_FILE = appSettings["PATH_DATA_FILE"];
            if (string.IsNullOrWhiteSpace(PATH_DATA_FILE))
            {
                PATH_DATA_FILE = Path.Combine(PATH_ROOT, "_data");
                if (!Directory.Exists(PATH_DATA_FILE)) Directory.CreateDirectory(PATH_DATA_FILE);
            }

            string portLogRedis = appSettings["LOG_PORT_REDIS"];
            if (!string.IsNullOrWhiteSpace(portLogRedis))
            {
                int.TryParse(portLogRedis, out LOG_PORT_REDIS);
                if (LOG_PORT_REDIS <= 0) LOG_PORT_REDIS = LOG_PORT_REDIS_DEFAULT;
            }
        }
    }

    #region [ LOG ]
    public interface ILog
    {
        void write(string scope_name, string key, string text);
    }

    public class LogRedis : ILog
    {
        static long ID_INCREMENT = 0;
        //static RedisClient _redis = null;
        static bool _connected = false;

        public LogRedis(int port = 0) { _init(port == 0 ? _CONFIG.LOG_PORT_REDIS : port); }

        void _init(int port)
        {
            try
            {
                if (_connected == false)
                {
                    //_redis = new RedisClient("localhost", port);
                    _connected = true;
                }
            }
            catch
            {
            }
        }

        public void write(string scope_name, string key, string text)
        {
            if (_connected == false) return;
            if (text == null) text = string.Empty;

            try
            {
                //_redis.HSet(scope_name, key, text);
            }
            catch
            {
            }
        }

        void write1(string scope_name, string key, params object[] paras)
        {
            Interlocked.Increment(ref ID_INCREMENT);
            if (ID_INCREMENT == int.MaxValue) ID_INCREMENT = 0;

            string id = string.Format("{0}.{1}.{2}", DateTime.Now.ToString("yyMMddHHmm"), ID_INCREMENT, key);

            StringBuilder bi = new StringBuilder(DateTime.Now.ToString("yyMMdd.HHmmss.fff"));
            bi.Append(Environment.NewLine);
            try
            {
                for (var i = 0; i < paras.Length; i++)
                {
                    string type = paras[i].GetType().Name;
                    switch (type)
                    {
                        case "Boolean":
                        case "Byte":
                        case "SByte":
                        case "Char":
                        case "Decimal":
                        case "Double":
                        case "Single":
                        case "Int32":
                        case "UInt32":
                        case "Int64":
                        case "UInt64":
                        case "Int16":
                        case "UInt16":
                        case "String":
                            bi.Append(paras[i].ToString());
                            break;
                        default:
                            string text = JsonConvert.SerializeObject(paras[i], Formatting.Indented);
                            bi.Append(text);
                            bi.Append(Environment.NewLine);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                string text = JsonConvert.SerializeObject(paras, Formatting.Indented);
                bi.Append(text);
                bi.Append(Environment.NewLine);
                bi.Append("ERROR_LOG: " + ex.Message);
            }

            try
            {
                //_redis.HSet(scope_name, id, bi.ToString());
            }
            catch { }
        }
    }

    #endregion

    #region [ EXTEND ]

    public static class JobExt
    {
        public static string getValue(this Dictionary<string, object> dic, string key)
        {
            try
            {
                if (dic != null && dic.ContainsKey(key))
                {
                    var p = dic[key];
                    if (p == null) return string.Empty;
                    return p.ToString();
                }
            }
            catch { }
            return string.Empty;
        }

        public static T getValue<T>(this Dictionary<string, object> dic, string key)
        {
            try
            {
                if (dic != null && dic.ContainsKey(key))
                {
                    var p = (T)dic[key];
                    return p;
                }
            }
            catch { }

            T v = default(T);
            return v;
        }


        public static Dictionary<string, object> getParaInput(this IJobExecutionContext context)
        {
            try
            {
                if (context != null)
                {
                    JobDataMap dataMap = context.JobDetail.JobDataMap;
                    if (dataMap.ContainsKey("PARA___"))
                    {
                        var p = (Dictionary<string, object>)dataMap["PARA___"];
                        return p;
                    }
                }
            }
            catch { }

            return null;
        }


        public static void log(this IJobExecutionContext context, string key, params object[] paras)
        {
            try
            {
                if (context != null)
                {
                    if (context.Scheduler.Context.ContainsKey("ILOG___"))
                    {
                        string server_name = string.Empty,
                            id = string.Empty,
                            schedule = string.Empty,
                            scope_name = string.Empty,
                            current_id = string.Empty,
                            para_text = string.Empty;

                        int counter = 0;

                        ILog log = (ILog)context.Scheduler.Context.Get("ILOG___");

                        if (context.Scheduler.Context.ContainsKey("SCOPE_NAME___"))
                            scope_name = context.Scheduler.Context.GetString("SCOPE_NAME___");

                        JobDataMap dataMap = context.JobDetail.JobDataMap;

                        // id = [group_name].[date_time_create = yyMMddHHmmss].[guid_id...]
                        if (dataMap.ContainsKey("ID___")) id = dataMap.Get("ID___").ToString();
                        if (dataMap.ContainsKey("CURRENT_ID___")) current_id = dataMap.Get("CURRENT_ID___").ToString();
                        if (dataMap.ContainsKey("SCHEDULE___")) schedule = dataMap.Get("SCHEDULE___").ToString();

                        if (dataMap.ContainsKey("PARA___"))
                        {
                            var p = dataMap["PARA___"];
                            if (p == null) para_text = "NULL";
                            else
                            {
                                switch (p.GetType().Name)
                                {
                                    case "Boolean":
                                    case "Byte":
                                    case "SByte":
                                    case "Char":
                                    case "Decimal":
                                    case "Double":
                                    case "Single":
                                    case "Int32":
                                    case "UInt32":
                                    case "Int64":
                                    case "UInt64":
                                    case "Int16":
                                    case "UInt16":
                                    case "String":
                                        para_text = p.ToString();
                                        break;
                                    default:
                                        para_text = JsonConvert.SerializeObject(p, Formatting.Indented);
                                        break;
                                }
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(para_text))
                            para_text += Environment.NewLine + "----------------------------" + Environment.NewLine;

                        if (dataMap.ContainsKey("COUNTER___"))
                        {
                            var state = (ConcurrentDictionary<long, bool>)dataMap["COUNTER___"];
                            counter = state.Count;
                        }

                        StringBuilder bi = new StringBuilder();
                        bi.Append(Environment.NewLine);
                        try
                        {
                            for (var i = 0; i < paras.Length; i++)
                            {
                                string type = paras[i].GetType().Name;
                                switch (type)
                                {
                                    case "Boolean":
                                    case "Byte":
                                    case "SByte":
                                    case "Char":
                                    case "Decimal":
                                    case "Double":
                                    case "Single":
                                    case "Int32":
                                    case "UInt32":
                                    case "Int64":
                                    case "UInt64":
                                    case "Int16":
                                    case "UInt16":
                                    case "String":
                                        bi.Append(paras[i].ToString());
                                        break;
                                    default:
                                        string s = JsonConvert.SerializeObject(paras[i], Formatting.Indented);
                                        bi.Append(s);
                                        bi.Append(Environment.NewLine);
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string s = JsonConvert.SerializeObject(paras, Formatting.Indented);
                            bi.Append(s);
                            bi.Append(Environment.NewLine);
                            bi.Append("ERROR_LOG: " + ex.Message);
                        }

                        string
                            l_scope = scope_name + "." + id,
                            l_key = DateTime.Now.ToString("yyMMdd-HHmmss-fff") + "." + key;

                        string text =
                            "LOG_ID: " + l_key + Environment.NewLine +
                            "JOB_ID: " + id + Environment.NewLine +
                            "CURRENT_ID: " + current_id + Environment.NewLine +
                            "SCHEDULE: " + schedule + Environment.NewLine +
                            "COUNT: " + counter.ToString() + Environment.NewLine +
                            "----------------------------" + Environment.NewLine +
                            para_text + bi.ToString();

                        log.write(l_scope, l_key, text);
                    }
                }
            }
            catch { }
        }

    }

    #endregion

    #region [ RESULT ] 

    public enum DATA_TYPE
    {
        TEXT_PLAIN = 0,
        JSON_TEXT = 1,
        JSON_RESPONSE = 2,
        HTML_TEXT = 3,
        ARRAY_LIST = 4,
        OBJECT = 5,
        BUFFER = 6
    }

    public class oResult
    {
        public bool ok { set; get; }
        public object data { set; get; }
        public Dictionary<string, object> input { set; get; }
        public string name { set; get; }
        public string error { set; get; }
        public Dictionary<string, object> request { set; get; }
        public DATA_TYPE type { set; get; }

        public oResult()
        {
            this.ok = false;
            this.data = null;
            this.name = string.Empty;
            this.error = string.Empty;
            this.type = DATA_TYPE.JSON_TEXT;
            this.request = new Dictionary<string, object>();
        }

        public static oResult Ok(object data = null, Dictionary<string, object> request = null, DATA_TYPE type = DATA_TYPE.JSON_TEXT)
        {
            if (request == null) request = new Dictionary<string, object>();
            return new oResult() { ok = true, data = data, type = type, request = request };
        }

        public static oResult Error(string message = "", Dictionary<string, object> request = null)
        {
            if (request == null) request = new Dictionary<string, object>();
            return new oResult() { ok = false, error = message, type = DATA_TYPE.JSON_TEXT };
        }
    }

    #endregion

    #region [ API ]

    public interface IApi
    {

        #region [ notify_user | notify_broadcast ]

        oResult notify_user(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult notify_broadcast(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);

        #endregion

        #region [ request_async | curl_call ( get|post|put... ) ]

        oResult request_async(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult curl_call(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);


        #endregion

        #region [ html_export_links | html_export_images | html_to_text_01 | html_clean_01 | html_remove_comment | html_remove_tag_simple | html_remove_tag_content | html_remove_tag_keep_content ]

        oResult html_export_links(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult html_export_images(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult html_to_text_01(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult html_clean_01(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult html_remove_comment(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult html_remove_tag_simple(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult html_remove_tag_content(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult html_remove_tag_keep_content(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);

        #endregion

        #region [ api_list | api_get | api_reload | api_reload_all | api_exist ]

        oResult api_list(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult api_get(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult api_reload(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult api_reload_all(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult api_exist(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);


        #endregion

        #region [ file_read_text | file_write_text | file_append_text | file_exist | file_delete | dir_get_files | dir_exist | dir_create | dir_delete ]

        oResult file_read_text(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult file_write_text(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult file_append_text(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult file_exist(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult file_delete(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult dir_get_files(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult dir_exist(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult dir_create(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult dir_delete(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);


        #endregion

        #region [ cache_addnew | cache_update | cache_remove | cache_clear_all ]

        oResult cache_addnew(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult cache_update(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult cache_remove(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult cache_clear_all(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);


        #endregion

        #region [ cache_runtime_exist |cache_runtime_set | cache_runtime_get | cache_runtime_remove ]

        oResult cache_runtime_exist(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult cache_runtime_set(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult cache_runtime_get(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult cache_runtime_remove(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);


        #endregion

        #region [ cache_search | cache_get_item_by_id | cache_get_items_by_ids ]

        oResult cache_search(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult cache_get_item_by_id(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult cache_get_items_by_ids(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);


        #endregion

        #region [ db_execute ]

        oResult db_execute(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);

        #endregion

        #region [ job_list | job_create | job_stop | job_start | job_remove ]

        oResult job_list(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult job_create(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult job_pause(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult job_resume(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);
        oResult job_remove(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null);

        #endregion

        string js_call(string api_source, string api_name, string paramenter = null, string request = null);
    }

    public class clsApi : IApi
    {
        public ICache m_cache { get; private set; }
        public clsApi(ICache cache) { m_cache = cache; }

        #region [ notify_user | notify_broadcast ]

        public oResult notify_user(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try
            {

            }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }

        public oResult notify_broadcast(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }

        #endregion

        #region [ request_async | curl_call ( get|post|put... ) ]

        public oResult request_async(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try
            {
                #region [ request_async ]

                if (paramenter.Count > 0 && paramenter.ContainsKey("headers"))
                {
                    #region

                    Dictionary<string, object> headers = null;

                    if (paramenter.ContainsKey("headers") == false)
                    {
                        r.error = "ERROR_request_async: The paramenter [headers] not exist";
                        return r;
                    }

                    if (paramenter.ContainsKey("headers"))
                    {
                        try
                        {
                            headers = ((JObject)paramenter["headers"]).ToObject<Dictionary<string, object>>();
                        }
                        catch (Exception e)
                        {
                            r.error = "ERROR_request_async: The paramenter [headers] must be format JSON. " + e.Message;
                            return r;
                        }
                    }

                    if (headers.Count == 0
                        || headers.ContainsKey("url") == false || headers["url"] == null || string.IsNullOrEmpty(headers["url"].ToString())
                        || headers.ContainsKey("method") == false || headers["method"] == null || string.IsNullOrEmpty(headers["method"].ToString()))
                    {
                        r.error = "ERROR_request_async: The paramenter [headers] must be { url:..., method:... } and Value is not null or empty";
                        return r;
                    }

                    #endregion

                    try
                    {
                        string httpMethod = "GET";
                        string type = "v8";
                        string url = string.Empty;
                        string htm = string.Empty;

                        foreach (var header in headers)
                        {
                            if (header.Key.StartsWith("Content") || header.Key == "data") continue;

                            if (header.Key == "url")
                            {
                                url = header.Value.ToString();
                                continue;
                            }

                            if (header.Key == "method")
                            {
                                httpMethod = header.Value.ToString();
                                continue;
                            }

                            if (header.Key == "type")
                            {
                                type = header.Value.ToString();
                                continue;
                            }
                        }
                        switch (type)
                        {
                            case "curl":
                                //htm = clsCURL.___https(url);
                                r.ok = true;
                                r.data = htm;
                                break;
                            case "v8":
                                try
                                {
                                    V8ScriptEngine engine = new V8ScriptEngine(V8ScriptEngineFlags.DisableGlobalMembers);
                                    engine.AddCOMType("XMLHttpRequest", "MSXML2.XMLHTTP");
                                    engine.Execute(@" function get(url) { var xhr = new XMLHttpRequest(); xhr.open('GET', url, false); xhr.send(); if (xhr.status == 200) return xhr.responseText; else return ''; }");
                                    htm = engine.Script.get(url);
                                    engine.Dispose();

                                    r.ok = true;
                                    r.data = htm;
                                }
                                catch (Exception e1)
                                {
                                    //htm = e1.Message;
                                    r.error = "ERROR_XHR_request_async: " + e1.Message;
                                    return r;
                                }
                                break;
                            default:
                                using (var httpClient = new HttpClient())
                                {
                                    foreach (var header in headers)
                                    {
                                        if (header.Key.StartsWith("Content") || header.Key == "type" || header.Key == "data" || header.Key == "url" || header.Key == "method") continue;
                                        if (header.Value != null) httpClient.DefaultRequestHeaders.Add(header.Key, header.Value.ToString());
                                    }

                                    //============================================================
                                    //ServicePointManager.CertificatePolicy = new MyPolicy();
                                    //ServicePointManager.Expect100Continue = true;
                                    //ServicePointManager.DefaultConnectionLimit = 9999;
                                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;

                                    //============================================================
                                    HttpResponseMessage responseMessage = null;
                                    switch (httpMethod)
                                    {
                                        case "DELETE":
                                            responseMessage = httpClient.DeleteAsync(url).Result;
                                            break;
                                        case "PATCH":
                                        case "POST":
                                            string data = string.Empty;

                                            if (paramenter.ContainsKey("data") && paramenter["data"] != null)
                                                data = paramenter["data"].ToString();

                                            responseMessage = httpClient.PostAsync(url, new StringContent(data)).Result;

                                            break;
                                        case "GET":
                                            responseMessage = httpClient.GetAsync(url).Result;
                                            break;
                                    }

                                    if (responseMessage != null)
                                        using (responseMessage)
                                        using (var content = responseMessage.Content)
                                            htm = content.ReadAsStringAsync().Result.Trim();

                                    if (htm.Length > 0)
                                    {
                                        r.ok = true;
                                        r.data = htm;
                                    }
                                }
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        r.error = "ERROR_THROW_request_async: " + e.Message;
                        return r;
                    }
                }

                #endregion

            }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }

        public oResult curl_call(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }


        #endregion

        #region [ html_export_links | html_export_images | html_to_text_01 | html_clean_01 | html_remove_comment | html_remove_tag_simple | html_remove_tag_content | html_remove_tag_keep_content ]

        public oResult html_export_links(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult html_export_images(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult html_to_text_01(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult html_clean_01(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult html_remove_comment(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult html_remove_tag_simple(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult html_remove_tag_content(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult html_remove_tag_keep_content(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }

        #endregion

        #region [ api_list | api_get | api_reload | api_reload_all | api_exist ]

        public oResult api_list(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult api_get(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult api_reload(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult api_reload_all(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult api_exist(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }


        #endregion

        #region [ file_read_text | file_write_text | file_append_text | file_exist | file_delete | dir_get_files | dir_exist | dir_create | dir_delete ]

        public oResult file_read_text(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult file_write_text(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult file_append_text(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult file_exist(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult file_delete(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult dir_get_files(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult dir_exist(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult dir_create(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult dir_delete(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }


        #endregion

        #region [ cache_addnew | cache_update | cache_remove | cache_clear_all ]

        public oResult cache_addnew(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult cache_update(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult cache_remove(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult cache_clear_all(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }


        #endregion

        #region [ cache_runtime_exist |cache_runtime_set | cache_runtime_get | cache_runtime_remove ]

        public oResult cache_runtime_exist(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult cache_runtime_set(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult cache_runtime_get(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult cache_runtime_remove(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }


        #endregion

        #region [ cache_search | cache_get_item_by_id | cache_get_items_by_ids ]

        public oResult cache_search(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult cache_get_item_by_id(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }
        public oResult cache_get_items_by_ids(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try { }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[] " + e.Message;
            }
            return r;
        }

        #endregion

        #region [ db_execute ]

        public oResult db_execute(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try
            {
                if (paramenter.Count == 0
                    || !paramenter.ContainsKey("connect_string")
                    || !paramenter.ContainsKey("script_command"))
                {
                    r.error = "Paramenter missing connect_string or script_command";
                    return r;
                }

                var connect_string = paramenter.getValue("connect_string");
                var script_command = paramenter.getValue("script_command");

                using (SqlConnection connection = new SqlConnection(connect_string))
                {
                    SqlCommand cmd = new SqlCommand(script_command, connection);

                    foreach (var kv in paramenter)
                        cmd.Parameters.AddWithValue("@" + kv.Key, kv.Value);

                    foreach (var kv in request)
                        cmd.Parameters.AddWithValue("@" + kv.Key, kv.Value);

                    SqlDataAdapter dataAdapt = new SqlDataAdapter();
                    dataAdapt.SelectCommand = cmd;
                    DataTable dataTable = new DataTable();
                    dataAdapt.Fill(dataTable);

                    r.ok = true;
                    r.data = dataTable;
                }
            }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[db_execute] " + e.Message;
            }
            return r;
        }

        #endregion

        #region [ job_list | job_create | job_pause | job_resume | job_remove ]

        public oResult job_list(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try
            {
                r.ok = true;
                r.data = clsGlobal.list_jobs();
            }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[job_list] " + e.Message;
            }
            return r;
        }

        public oResult job_create(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {

            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try
            {
                if (paramenter.Count == 0
                    || !paramenter.ContainsKey("type")
                    || !paramenter.ContainsKey("group_name")
                    || !paramenter.ContainsKey("schedule"))
                {
                    r.error = "Paramenter missing type or group_name or schedule";
                    return r;
                }

                var type = (JOB_TYPE)paramenter.getValue<int>("type");
                var group_name = paramenter.getValue("group_name");
                var schedule = paramenter.getValue("schedule");

                r.name = clsGlobal.create_job(type, group_name, paramenter, schedule);

                r.ok = string.IsNullOrEmpty(r.name) == false;
            }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[job_create] " + e.Message;
            }
            return r;
        }

        public oResult job_pause(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {

            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try
            {
                if (paramenter.Count == 0
                    || !paramenter.ContainsKey("job_name"))
                {
                    r.error = "Paramenter missing job_name";
                    return r;
                }
                var job_name = paramenter.getValue("job_name");
                clsGlobal.pause_job(job_name);
                r.ok = clsGlobal.exist_job(job_name);
            }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[pause_job] " + e.Message;
            }
            return r;
        }

        public oResult job_resume(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try
            {
                if (paramenter.Count == 0
                    || !paramenter.ContainsKey("job_name"))
                {
                    r.error = "Paramenter missing job_name";
                    return r;
                }
                var job_name = paramenter.getValue("job_name");
                clsGlobal.resume_job(job_name);
                r.ok = clsGlobal.exist_job(job_name);
            }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[resume_job] " + e.Message;
            }
            return r;
        }

        public oResult job_remove(Dictionary<string, object> paramenter = null, Dictionary<string, object> request = null)
        {
            if (paramenter == null) paramenter = new Dictionary<string, object>();
            if (request == null) request = new Dictionary<string, object>();
            oResult r = new oResult() { ok = false, request = request, input = paramenter };
            try
            {
                if (paramenter.Count == 0
                    || !paramenter.ContainsKey("job_name"))
                {
                    r.error = "Paramenter missing job_name";
                    return r;
                }
                var job_name = paramenter.getValue("job_name");
                r.ok = clsGlobal.remove_job(job_name);
            }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: API[job_remove] " + e.Message;
            }
            return r;
        }

        #endregion

        public string js_call(string api_source, string api_name, string paramenter = null, string request = null)
        {
            oResult r = new oResult() { ok = false };

            Dictionary<string, object> para = null;
            Dictionary<string, object> req = null;

            if (string.IsNullOrWhiteSpace(paramenter)) para = new Dictionary<string, object>();
            if (string.IsNullOrWhiteSpace(request)) req = new Dictionary<string, object>();

            try
            {
                if (!string.IsNullOrWhiteSpace(paramenter))
                    para = JsonConvert.DeserializeObject<Dictionary<string, object>>(paramenter);
            }
            catch (Exception ex)
            {
                r.error = "Convert Paramenter to type Dictionary<string, object> has error: " + ex.Message;
                return JsonConvert.SerializeObject(r);
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(request))
                    req = JsonConvert.DeserializeObject<Dictionary<string, object>>(request);
            }
            catch (Exception ex)
            {
                r.error = "Convert Request to type Dictionary<string, object> has error: " + ex.Message;
                return JsonConvert.SerializeObject(r);
            }

            bool exist = false;
            switch (api_name)
            {
                case "notify_user":
                    exist = true;
                    r = notify_user(para, req);
                    break;
                case "notify_broadcast":
                    exist = true;
                    r = notify_broadcast(para, req);
                    break;

                case "request_async":
                    exist = true;
                    r = request_async(para, req);
                    break;
                case "curl_call":
                    exist = true;
                    r = curl_call(para, req);
                    break;

                case "html_export_links":
                    exist = true;
                    r = html_export_links(para, req);
                    break;
                case "html_export_images":
                    exist = true;
                    r = html_export_images(para, req);
                    break;
                case "html_to_text_01":
                    exist = true;
                    r = html_to_text_01(para, req);
                    break;
                case "html_clean_01":
                    exist = true;
                    r = html_clean_01(para, req);
                    break;
                case "html_remove_comment":
                    exist = true;
                    r = html_remove_comment(para, req);
                    break;
                case "html_remove_tag_simple":
                    exist = true;
                    r = html_remove_tag_simple(para, req);
                    break;
                case "html_remove_tag_content":
                    exist = true;
                    r = html_remove_tag_content(para, req);
                    break;
                case "html_remove_tag_keep_content":
                    exist = true;
                    r = html_remove_tag_keep_content(para, req);
                    break;

                case "api_list":
                    exist = true;
                    r = api_list(para, req);
                    break;
                case "api_get":
                    exist = true;
                    r = api_get(para, req);
                    break;
                case "api_reload":
                    exist = true;
                    r = api_reload(para, req);
                    break;
                case "api_reload_all":
                    exist = true;
                    r = api_reload_all(para, req);
                    break;
                case "api_exist":
                    exist = true;
                    r = api_exist(para, req);
                    break;

                case "file_read_text":
                    exist = true;
                    r = file_read_text(para, req);
                    break;
                case "file_write_text":
                    exist = true;
                    r = file_write_text(para, req);
                    break;
                case "file_append_text":
                    exist = true;
                    r = file_append_text(para, req);
                    break;
                case "file_exist":
                    exist = true;
                    r = file_exist(para, req);
                    break;
                case "file_delete":
                    exist = true;
                    r = file_delete(para, req);
                    break;

                case "dir_get_files":
                    exist = true;
                    r = dir_get_files(para, req);
                    break;
                case "dir_exist":
                    exist = true;
                    r = dir_exist(para, req);
                    break;
                case "dir_create":
                    exist = true;
                    r = dir_create(para, req);
                    break;
                case "dir_delete":
                    exist = true;
                    r = dir_delete(para, req);
                    break;

                case "cache_addnew":
                    exist = true;
                    r = cache_addnew(para, req);
                    break;
                case "cache_update":
                    exist = true;
                    r = cache_update(para, req);
                    break;
                case "cache_remove":
                    exist = true;
                    r = cache_remove(para, req);
                    break;
                case "cache_clear_all":
                    exist = true;
                    r = cache_clear_all(para, req);
                    break;
                case "cache_runtime_exist":
                    exist = true;
                    r = cache_runtime_exist(para, req);
                    break;
                case "cache_runtime_set":
                    exist = true;
                    r = cache_runtime_set(para, req);
                    break;
                case "cache_runtime_get":
                    exist = true;
                    r = cache_runtime_get(para, req);
                    break;
                case "cache_runtime_remove":
                    exist = true;
                    r = cache_runtime_remove(para, req);
                    break;
                case "cache_search":
                    exist = true;
                    r = cache_search(para, req);
                    break;
                case "cache_get_item_by_id":
                    exist = true;
                    r = cache_get_item_by_id(para, req);
                    break;
                case "cache_get_items_by_ids":
                    exist = true;
                    r = cache_get_items_by_ids(para, req);
                    break;

                case "db_execute":
                    exist = true;
                    r = db_execute(para, req);
                    break;

                case "job_list":
                    exist = true;
                    r = job_list(para, req);
                    break;
                case "job_create":
                    exist = true;
                    r = job_create(para, req);
                    break;
                case "job_pause":
                    exist = true;
                    r = job_pause(para, req);
                    break;
                case "job_resume":
                    exist = true;
                    r = job_resume(para, req);
                    break;
                case "job_remove":
                    exist = true;
                    r = job_remove(para, req);
                    break;
            }

            if (exist == false)
                r.error = "Cannot found API " + api_name;

            return JsonConvert.SerializeObject(r);
        }
    }

    #endregion

    #region [ CACHE ]
    public interface ICache
    {
        void reloadCache_ApiJS(string cache_name = null, string api_name = null);
        bool existCache_ApiJS(string cache_name, string api_name);
        bool existCache_Data(string cache_name);

        void view___reload();
        string view___build(string html, Uri uri, string wroot);
    }

    public class clsCache : ICache
    {
        static long ID_INCREMENT = 0;
        const int SIZE_INDEX = 0;
        static string ERROR_MESSAGE = string.Empty;

        #region [ CONTRACTOR ]

        static readonly string[] m_names = new string[99];

        static bool b_01 = false;
        static bool b_02 = false;
        static bool b_03 = false;
        static bool b_04 = false;
        static bool b_05 = false;
        static bool b_06 = false;
        static bool b_07 = false;
        static bool b_08 = false;
        static bool b_09 = false;
        static bool b_10 = false;
        static bool b_11 = false;
        static bool b_12 = false;
        static bool b_13 = false;
        static bool b_14 = false;
        static bool b_15 = false;
        static bool b_16 = false;
        static bool b_17 = false;
        static bool b_18 = false;
        static bool b_19 = false;
        static bool b_20 = false;
        static bool b_21 = false;
        static bool b_22 = false;
        static bool b_23 = false;
        static bool b_24 = false;
        static bool b_25 = false;
        static bool b_26 = false;
        static bool b_27 = false;
        static bool b_28 = false;
        static bool b_29 = false;
        static bool b_30 = false;
        static bool b_31 = false;
        static bool b_32 = false;
        static bool b_33 = false;
        static bool b_34 = false;
        static bool b_35 = false;
        static bool b_36 = false;
        static bool b_37 = false;
        static bool b_38 = false;
        static bool b_39 = false;
        static bool b_40 = false;
        static bool b_41 = false;
        static bool b_42 = false;
        static bool b_43 = false;
        static bool b_44 = false;
        static bool b_45 = false;
        static bool b_46 = false;
        static bool b_47 = false;
        static bool b_48 = false;
        static bool b_49 = false;
        static bool b_50 = false;
        static bool b_51 = false;
        static bool b_52 = false;
        static bool b_53 = false;
        static bool b_54 = false;
        static bool b_55 = false;
        static bool b_56 = false;
        static bool b_57 = false;
        static bool b_58 = false;
        static bool b_59 = false;
        static bool b_60 = false;
        static bool b_61 = false;
        static bool b_62 = false;
        static bool b_63 = false;
        static bool b_64 = false;
        static bool b_65 = false;
        static bool b_66 = false;
        static bool b_67 = false;
        static bool b_68 = false;
        static bool b_69 = false;
        static bool b_70 = false;
        static bool b_71 = false;
        static bool b_72 = false;
        static bool b_73 = false;
        static bool b_74 = false;
        static bool b_75 = false;
        static bool b_76 = false;
        static bool b_77 = false;
        static bool b_78 = false;
        static bool b_79 = false;
        static bool b_80 = false;
        static bool b_81 = false;
        static bool b_82 = false;
        static bool b_83 = false;
        static bool b_84 = false;
        static bool b_85 = false;
        static bool b_86 = false;
        static bool b_87 = false;
        static bool b_88 = false;
        static bool b_89 = false;
        static bool b_90 = false;
        static bool b_91 = false;
        static bool b_92 = false;
        static bool b_93 = false;
        static bool b_94 = false;
        static bool b_95 = false;
        static bool b_96 = false;
        static bool b_97 = false;
        static bool b_98 = false;
        static bool b_99 = false;


        static ConcurrentDictionary<long, string> m_01 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_02 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_03 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_04 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_05 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_06 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_07 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_08 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_09 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_10 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_11 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_12 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_13 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_14 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_15 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_16 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_17 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_18 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_19 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_20 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_21 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_22 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_23 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_24 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_25 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_26 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_27 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_28 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_29 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_30 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_31 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_32 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_33 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_34 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_35 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_36 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_37 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_38 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_39 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_40 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_41 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_42 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_43 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_44 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_45 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_46 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_47 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_48 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_49 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_50 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_51 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_52 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_53 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_54 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_55 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_56 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_57 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_58 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_59 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_60 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_61 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_62 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_63 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_64 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_65 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_66 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_67 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_68 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_69 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_70 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_71 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_72 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_73 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_74 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_75 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_76 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_77 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_78 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_79 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_80 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_81 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_82 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_83 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_84 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_85 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_86 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_87 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_88 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_89 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_90 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_91 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_92 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_93 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_94 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_95 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_96 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_97 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_98 = new ConcurrentDictionary<long, string>();
        static ConcurrentDictionary<long, string> m_99 = new ConcurrentDictionary<long, string>();

        static string[] m_01_ids = new string[SIZE_INDEX];
        static string[] m_02_ids = new string[SIZE_INDEX];
        static string[] m_03_ids = new string[SIZE_INDEX];
        static string[] m_04_ids = new string[SIZE_INDEX];
        static string[] m_05_ids = new string[SIZE_INDEX];
        static string[] m_06_ids = new string[SIZE_INDEX];
        static string[] m_07_ids = new string[SIZE_INDEX];
        static string[] m_08_ids = new string[SIZE_INDEX];
        static string[] m_09_ids = new string[SIZE_INDEX];
        static string[] m_10_ids = new string[SIZE_INDEX];
        static string[] m_11_ids = new string[SIZE_INDEX];
        static string[] m_12_ids = new string[SIZE_INDEX];
        static string[] m_13_ids = new string[SIZE_INDEX];
        static string[] m_14_ids = new string[SIZE_INDEX];
        static string[] m_15_ids = new string[SIZE_INDEX];
        static string[] m_16_ids = new string[SIZE_INDEX];
        static string[] m_17_ids = new string[SIZE_INDEX];
        static string[] m_18_ids = new string[SIZE_INDEX];
        static string[] m_19_ids = new string[SIZE_INDEX];
        static string[] m_20_ids = new string[SIZE_INDEX];
        static string[] m_21_ids = new string[SIZE_INDEX];
        static string[] m_22_ids = new string[SIZE_INDEX];
        static string[] m_23_ids = new string[SIZE_INDEX];
        static string[] m_24_ids = new string[SIZE_INDEX];
        static string[] m_25_ids = new string[SIZE_INDEX];
        static string[] m_26_ids = new string[SIZE_INDEX];
        static string[] m_27_ids = new string[SIZE_INDEX];
        static string[] m_28_ids = new string[SIZE_INDEX];
        static string[] m_29_ids = new string[SIZE_INDEX];
        static string[] m_30_ids = new string[SIZE_INDEX];
        static string[] m_31_ids = new string[SIZE_INDEX];
        static string[] m_32_ids = new string[SIZE_INDEX];
        static string[] m_33_ids = new string[SIZE_INDEX];
        static string[] m_34_ids = new string[SIZE_INDEX];
        static string[] m_35_ids = new string[SIZE_INDEX];
        static string[] m_36_ids = new string[SIZE_INDEX];
        static string[] m_37_ids = new string[SIZE_INDEX];
        static string[] m_38_ids = new string[SIZE_INDEX];
        static string[] m_39_ids = new string[SIZE_INDEX];
        static string[] m_40_ids = new string[SIZE_INDEX];
        static string[] m_41_ids = new string[SIZE_INDEX];
        static string[] m_42_ids = new string[SIZE_INDEX];
        static string[] m_43_ids = new string[SIZE_INDEX];
        static string[] m_44_ids = new string[SIZE_INDEX];
        static string[] m_45_ids = new string[SIZE_INDEX];
        static string[] m_46_ids = new string[SIZE_INDEX];
        static string[] m_47_ids = new string[SIZE_INDEX];
        static string[] m_48_ids = new string[SIZE_INDEX];
        static string[] m_49_ids = new string[SIZE_INDEX];
        static string[] m_50_ids = new string[SIZE_INDEX];
        static string[] m_51_ids = new string[SIZE_INDEX];
        static string[] m_52_ids = new string[SIZE_INDEX];
        static string[] m_53_ids = new string[SIZE_INDEX];
        static string[] m_54_ids = new string[SIZE_INDEX];
        static string[] m_55_ids = new string[SIZE_INDEX];
        static string[] m_56_ids = new string[SIZE_INDEX];
        static string[] m_57_ids = new string[SIZE_INDEX];
        static string[] m_58_ids = new string[SIZE_INDEX];
        static string[] m_59_ids = new string[SIZE_INDEX];
        static string[] m_60_ids = new string[SIZE_INDEX];
        static string[] m_61_ids = new string[SIZE_INDEX];
        static string[] m_62_ids = new string[SIZE_INDEX];
        static string[] m_63_ids = new string[SIZE_INDEX];
        static string[] m_64_ids = new string[SIZE_INDEX];
        static string[] m_65_ids = new string[SIZE_INDEX];
        static string[] m_66_ids = new string[SIZE_INDEX];
        static string[] m_67_ids = new string[SIZE_INDEX];
        static string[] m_68_ids = new string[SIZE_INDEX];
        static string[] m_69_ids = new string[SIZE_INDEX];
        static string[] m_70_ids = new string[SIZE_INDEX];
        static string[] m_71_ids = new string[SIZE_INDEX];
        static string[] m_72_ids = new string[SIZE_INDEX];
        static string[] m_73_ids = new string[SIZE_INDEX];
        static string[] m_74_ids = new string[SIZE_INDEX];
        static string[] m_75_ids = new string[SIZE_INDEX];
        static string[] m_76_ids = new string[SIZE_INDEX];
        static string[] m_77_ids = new string[SIZE_INDEX];
        static string[] m_78_ids = new string[SIZE_INDEX];
        static string[] m_79_ids = new string[SIZE_INDEX];
        static string[] m_80_ids = new string[SIZE_INDEX];
        static string[] m_81_ids = new string[SIZE_INDEX];
        static string[] m_82_ids = new string[SIZE_INDEX];
        static string[] m_83_ids = new string[SIZE_INDEX];
        static string[] m_84_ids = new string[SIZE_INDEX];
        static string[] m_85_ids = new string[SIZE_INDEX];
        static string[] m_86_ids = new string[SIZE_INDEX];
        static string[] m_87_ids = new string[SIZE_INDEX];
        static string[] m_88_ids = new string[SIZE_INDEX];
        static string[] m_89_ids = new string[SIZE_INDEX];
        static string[] m_90_ids = new string[SIZE_INDEX];
        static string[] m_91_ids = new string[SIZE_INDEX];
        static string[] m_92_ids = new string[SIZE_INDEX];
        static string[] m_93_ids = new string[SIZE_INDEX];
        static string[] m_94_ids = new string[SIZE_INDEX];
        static string[] m_95_ids = new string[SIZE_INDEX];
        static string[] m_96_ids = new string[SIZE_INDEX];
        static string[] m_97_ids = new string[SIZE_INDEX];
        static string[] m_98_ids = new string[SIZE_INDEX];
        static string[] m_99_ids = new string[SIZE_INDEX];

        static string[] m_01_ascii = new string[SIZE_INDEX];
        static string[] m_02_ascii = new string[SIZE_INDEX];
        static string[] m_03_ascii = new string[SIZE_INDEX];
        static string[] m_04_ascii = new string[SIZE_INDEX];
        static string[] m_05_ascii = new string[SIZE_INDEX];
        static string[] m_06_ascii = new string[SIZE_INDEX];
        static string[] m_07_ascii = new string[SIZE_INDEX];
        static string[] m_08_ascii = new string[SIZE_INDEX];
        static string[] m_09_ascii = new string[SIZE_INDEX];
        static string[] m_10_ascii = new string[SIZE_INDEX];
        static string[] m_11_ascii = new string[SIZE_INDEX];
        static string[] m_12_ascii = new string[SIZE_INDEX];
        static string[] m_13_ascii = new string[SIZE_INDEX];
        static string[] m_14_ascii = new string[SIZE_INDEX];
        static string[] m_15_ascii = new string[SIZE_INDEX];
        static string[] m_16_ascii = new string[SIZE_INDEX];
        static string[] m_17_ascii = new string[SIZE_INDEX];
        static string[] m_18_ascii = new string[SIZE_INDEX];
        static string[] m_19_ascii = new string[SIZE_INDEX];
        static string[] m_20_ascii = new string[SIZE_INDEX];
        static string[] m_21_ascii = new string[SIZE_INDEX];
        static string[] m_22_ascii = new string[SIZE_INDEX];
        static string[] m_23_ascii = new string[SIZE_INDEX];
        static string[] m_24_ascii = new string[SIZE_INDEX];
        static string[] m_25_ascii = new string[SIZE_INDEX];
        static string[] m_26_ascii = new string[SIZE_INDEX];
        static string[] m_27_ascii = new string[SIZE_INDEX];
        static string[] m_28_ascii = new string[SIZE_INDEX];
        static string[] m_29_ascii = new string[SIZE_INDEX];
        static string[] m_30_ascii = new string[SIZE_INDEX];
        static string[] m_31_ascii = new string[SIZE_INDEX];
        static string[] m_32_ascii = new string[SIZE_INDEX];
        static string[] m_33_ascii = new string[SIZE_INDEX];
        static string[] m_34_ascii = new string[SIZE_INDEX];
        static string[] m_35_ascii = new string[SIZE_INDEX];
        static string[] m_36_ascii = new string[SIZE_INDEX];
        static string[] m_37_ascii = new string[SIZE_INDEX];
        static string[] m_38_ascii = new string[SIZE_INDEX];
        static string[] m_39_ascii = new string[SIZE_INDEX];
        static string[] m_40_ascii = new string[SIZE_INDEX];
        static string[] m_41_ascii = new string[SIZE_INDEX];
        static string[] m_42_ascii = new string[SIZE_INDEX];
        static string[] m_43_ascii = new string[SIZE_INDEX];
        static string[] m_44_ascii = new string[SIZE_INDEX];
        static string[] m_45_ascii = new string[SIZE_INDEX];
        static string[] m_46_ascii = new string[SIZE_INDEX];
        static string[] m_47_ascii = new string[SIZE_INDEX];
        static string[] m_48_ascii = new string[SIZE_INDEX];
        static string[] m_49_ascii = new string[SIZE_INDEX];
        static string[] m_50_ascii = new string[SIZE_INDEX];
        static string[] m_51_ascii = new string[SIZE_INDEX];
        static string[] m_52_ascii = new string[SIZE_INDEX];
        static string[] m_53_ascii = new string[SIZE_INDEX];
        static string[] m_54_ascii = new string[SIZE_INDEX];
        static string[] m_55_ascii = new string[SIZE_INDEX];
        static string[] m_56_ascii = new string[SIZE_INDEX];
        static string[] m_57_ascii = new string[SIZE_INDEX];
        static string[] m_58_ascii = new string[SIZE_INDEX];
        static string[] m_59_ascii = new string[SIZE_INDEX];
        static string[] m_60_ascii = new string[SIZE_INDEX];
        static string[] m_61_ascii = new string[SIZE_INDEX];
        static string[] m_62_ascii = new string[SIZE_INDEX];
        static string[] m_63_ascii = new string[SIZE_INDEX];
        static string[] m_64_ascii = new string[SIZE_INDEX];
        static string[] m_65_ascii = new string[SIZE_INDEX];
        static string[] m_66_ascii = new string[SIZE_INDEX];
        static string[] m_67_ascii = new string[SIZE_INDEX];
        static string[] m_68_ascii = new string[SIZE_INDEX];
        static string[] m_69_ascii = new string[SIZE_INDEX];
        static string[] m_70_ascii = new string[SIZE_INDEX];
        static string[] m_71_ascii = new string[SIZE_INDEX];
        static string[] m_72_ascii = new string[SIZE_INDEX];
        static string[] m_73_ascii = new string[SIZE_INDEX];
        static string[] m_74_ascii = new string[SIZE_INDEX];
        static string[] m_75_ascii = new string[SIZE_INDEX];
        static string[] m_76_ascii = new string[SIZE_INDEX];
        static string[] m_77_ascii = new string[SIZE_INDEX];
        static string[] m_78_ascii = new string[SIZE_INDEX];
        static string[] m_79_ascii = new string[SIZE_INDEX];
        static string[] m_80_ascii = new string[SIZE_INDEX];
        static string[] m_81_ascii = new string[SIZE_INDEX];
        static string[] m_82_ascii = new string[SIZE_INDEX];
        static string[] m_83_ascii = new string[SIZE_INDEX];
        static string[] m_84_ascii = new string[SIZE_INDEX];
        static string[] m_85_ascii = new string[SIZE_INDEX];
        static string[] m_86_ascii = new string[SIZE_INDEX];
        static string[] m_87_ascii = new string[SIZE_INDEX];
        static string[] m_88_ascii = new string[SIZE_INDEX];
        static string[] m_89_ascii = new string[SIZE_INDEX];
        static string[] m_90_ascii = new string[SIZE_INDEX];
        static string[] m_91_ascii = new string[SIZE_INDEX];
        static string[] m_92_ascii = new string[SIZE_INDEX];
        static string[] m_93_ascii = new string[SIZE_INDEX];
        static string[] m_94_ascii = new string[SIZE_INDEX];
        static string[] m_95_ascii = new string[SIZE_INDEX];
        static string[] m_96_ascii = new string[SIZE_INDEX];
        static string[] m_97_ascii = new string[SIZE_INDEX];
        static string[] m_98_ascii = new string[SIZE_INDEX];
        static string[] m_99_ascii = new string[SIZE_INDEX];

        static string[] m_01_utf8 = new string[SIZE_INDEX];
        static string[] m_02_utf8 = new string[SIZE_INDEX];
        static string[] m_03_utf8 = new string[SIZE_INDEX];
        static string[] m_04_utf8 = new string[SIZE_INDEX];
        static string[] m_05_utf8 = new string[SIZE_INDEX];
        static string[] m_06_utf8 = new string[SIZE_INDEX];
        static string[] m_07_utf8 = new string[SIZE_INDEX];
        static string[] m_08_utf8 = new string[SIZE_INDEX];
        static string[] m_09_utf8 = new string[SIZE_INDEX];
        static string[] m_10_utf8 = new string[SIZE_INDEX];
        static string[] m_11_utf8 = new string[SIZE_INDEX];
        static string[] m_12_utf8 = new string[SIZE_INDEX];
        static string[] m_13_utf8 = new string[SIZE_INDEX];
        static string[] m_14_utf8 = new string[SIZE_INDEX];
        static string[] m_15_utf8 = new string[SIZE_INDEX];
        static string[] m_16_utf8 = new string[SIZE_INDEX];
        static string[] m_17_utf8 = new string[SIZE_INDEX];
        static string[] m_18_utf8 = new string[SIZE_INDEX];
        static string[] m_19_utf8 = new string[SIZE_INDEX];
        static string[] m_20_utf8 = new string[SIZE_INDEX];
        static string[] m_21_utf8 = new string[SIZE_INDEX];
        static string[] m_22_utf8 = new string[SIZE_INDEX];
        static string[] m_23_utf8 = new string[SIZE_INDEX];
        static string[] m_24_utf8 = new string[SIZE_INDEX];
        static string[] m_25_utf8 = new string[SIZE_INDEX];
        static string[] m_26_utf8 = new string[SIZE_INDEX];
        static string[] m_27_utf8 = new string[SIZE_INDEX];
        static string[] m_28_utf8 = new string[SIZE_INDEX];
        static string[] m_29_utf8 = new string[SIZE_INDEX];
        static string[] m_30_utf8 = new string[SIZE_INDEX];
        static string[] m_31_utf8 = new string[SIZE_INDEX];
        static string[] m_32_utf8 = new string[SIZE_INDEX];
        static string[] m_33_utf8 = new string[SIZE_INDEX];
        static string[] m_34_utf8 = new string[SIZE_INDEX];
        static string[] m_35_utf8 = new string[SIZE_INDEX];
        static string[] m_36_utf8 = new string[SIZE_INDEX];
        static string[] m_37_utf8 = new string[SIZE_INDEX];
        static string[] m_38_utf8 = new string[SIZE_INDEX];
        static string[] m_39_utf8 = new string[SIZE_INDEX];
        static string[] m_40_utf8 = new string[SIZE_INDEX];
        static string[] m_41_utf8 = new string[SIZE_INDEX];
        static string[] m_42_utf8 = new string[SIZE_INDEX];
        static string[] m_43_utf8 = new string[SIZE_INDEX];
        static string[] m_44_utf8 = new string[SIZE_INDEX];
        static string[] m_45_utf8 = new string[SIZE_INDEX];
        static string[] m_46_utf8 = new string[SIZE_INDEX];
        static string[] m_47_utf8 = new string[SIZE_INDEX];
        static string[] m_48_utf8 = new string[SIZE_INDEX];
        static string[] m_49_utf8 = new string[SIZE_INDEX];
        static string[] m_50_utf8 = new string[SIZE_INDEX];
        static string[] m_51_utf8 = new string[SIZE_INDEX];
        static string[] m_52_utf8 = new string[SIZE_INDEX];
        static string[] m_53_utf8 = new string[SIZE_INDEX];
        static string[] m_54_utf8 = new string[SIZE_INDEX];
        static string[] m_55_utf8 = new string[SIZE_INDEX];
        static string[] m_56_utf8 = new string[SIZE_INDEX];
        static string[] m_57_utf8 = new string[SIZE_INDEX];
        static string[] m_58_utf8 = new string[SIZE_INDEX];
        static string[] m_59_utf8 = new string[SIZE_INDEX];
        static string[] m_60_utf8 = new string[SIZE_INDEX];
        static string[] m_61_utf8 = new string[SIZE_INDEX];
        static string[] m_62_utf8 = new string[SIZE_INDEX];
        static string[] m_63_utf8 = new string[SIZE_INDEX];
        static string[] m_64_utf8 = new string[SIZE_INDEX];
        static string[] m_65_utf8 = new string[SIZE_INDEX];
        static string[] m_66_utf8 = new string[SIZE_INDEX];
        static string[] m_67_utf8 = new string[SIZE_INDEX];
        static string[] m_68_utf8 = new string[SIZE_INDEX];
        static string[] m_69_utf8 = new string[SIZE_INDEX];
        static string[] m_70_utf8 = new string[SIZE_INDEX];
        static string[] m_71_utf8 = new string[SIZE_INDEX];
        static string[] m_72_utf8 = new string[SIZE_INDEX];
        static string[] m_73_utf8 = new string[SIZE_INDEX];
        static string[] m_74_utf8 = new string[SIZE_INDEX];
        static string[] m_75_utf8 = new string[SIZE_INDEX];
        static string[] m_76_utf8 = new string[SIZE_INDEX];
        static string[] m_77_utf8 = new string[SIZE_INDEX];
        static string[] m_78_utf8 = new string[SIZE_INDEX];
        static string[] m_79_utf8 = new string[SIZE_INDEX];
        static string[] m_80_utf8 = new string[SIZE_INDEX];
        static string[] m_81_utf8 = new string[SIZE_INDEX];
        static string[] m_82_utf8 = new string[SIZE_INDEX];
        static string[] m_83_utf8 = new string[SIZE_INDEX];
        static string[] m_84_utf8 = new string[SIZE_INDEX];
        static string[] m_85_utf8 = new string[SIZE_INDEX];
        static string[] m_86_utf8 = new string[SIZE_INDEX];
        static string[] m_87_utf8 = new string[SIZE_INDEX];
        static string[] m_88_utf8 = new string[SIZE_INDEX];
        static string[] m_89_utf8 = new string[SIZE_INDEX];
        static string[] m_90_utf8 = new string[SIZE_INDEX];
        static string[] m_91_utf8 = new string[SIZE_INDEX];
        static string[] m_92_utf8 = new string[SIZE_INDEX];
        static string[] m_93_utf8 = new string[SIZE_INDEX];
        static string[] m_94_utf8 = new string[SIZE_INDEX];
        static string[] m_95_utf8 = new string[SIZE_INDEX];
        static string[] m_96_utf8 = new string[SIZE_INDEX];
        static string[] m_97_utf8 = new string[SIZE_INDEX];
        static string[] m_98_utf8 = new string[SIZE_INDEX];
        static string[] m_99_utf8 = new string[SIZE_INDEX];

        #endregion

        #region [ BASE ]

        static bool m___check(string cache_name)
        {
            int index = -1;
            if (string.IsNullOrEmpty(cache_name)) return false;

            for (int i = 0; i < m_names.Length; i++) if (m_names[i] == cache_name) { index = i; break; }

            return index != -1;
        }

        static ConcurrentDictionary<long, string> m___get(string cache_name)
        {
            int index = -1;
            if (string.IsNullOrEmpty(cache_name)) return null;

            for (int i = 0; i < m_names.Length; i++) if (m_names[i] == cache_name) { index = i; break; }
            if (index == -1) return null;

            switch (index)
            {
                case 1: return m_01;
                case 2: return m_02;
                case 3: return m_03;
                case 4: return m_04;
                case 5: return m_05;
                case 6: return m_06;
                case 7: return m_07;
                case 8: return m_08;
                case 9: return m_09;
                case 10: return m_10;
                case 11: return m_11;
                case 12: return m_12;
                case 13: return m_13;
                case 14: return m_14;
                case 15: return m_15;
                case 16: return m_16;
                case 17: return m_17;
                case 18: return m_18;
                case 19: return m_19;
                case 20: return m_20;
                case 21: return m_21;
                case 22: return m_22;
                case 23: return m_23;
                case 24: return m_24;
                case 25: return m_25;
                case 26: return m_26;
                case 27: return m_27;
                case 28: return m_28;
                case 29: return m_29;
                case 30: return m_30;
                case 31: return m_31;
                case 32: return m_32;
                case 33: return m_33;
                case 34: return m_34;
                case 35: return m_35;
                case 36: return m_36;
                case 37: return m_37;
                case 38: return m_38;
                case 39: return m_39;
                case 40: return m_40;
                case 41: return m_41;
                case 42: return m_42;
                case 43: return m_43;
                case 44: return m_44;
                case 45: return m_45;
                case 46: return m_46;
                case 47: return m_47;
                case 48: return m_48;
                case 49: return m_49;
                case 50: return m_50;
                case 51: return m_51;
                case 52: return m_52;
                case 53: return m_53;
                case 54: return m_54;
                case 55: return m_55;
                case 56: return m_56;
                case 57: return m_57;
                case 58: return m_58;
                case 59: return m_59;
                case 60: return m_60;
                case 61: return m_61;
                case 62: return m_62;
                case 63: return m_63;
                case 64: return m_64;
                case 65: return m_65;
                case 66: return m_66;
                case 67: return m_67;
                case 68: return m_68;
                case 69: return m_69;
                case 70: return m_70;
                case 71: return m_71;
                case 72: return m_72;
                case 73: return m_73;
                case 74: return m_74;
                case 75: return m_75;
                case 76: return m_76;
                case 77: return m_77;
                case 78: return m_78;
                case 79: return m_79;
                case 80: return m_80;
                case 81: return m_81;
                case 82: return m_82;
                case 83: return m_83;
                case 84: return m_84;
                case 85: return m_85;
                case 86: return m_86;
                case 87: return m_87;
                case 88: return m_88;
                case 89: return m_89;
                case 90: return m_90;
                case 91: return m_91;
                case 92: return m_92;
                case 93: return m_93;
                case 94: return m_94;
                case 95: return m_95;
                case 96: return m_96;
                case 97: return m_97;
                case 98: return m_98;
                case 99: return m_99;
            }

            return null;
        }

        static void m___free_memory()
        {
            m_01.Clear();
            m_02.Clear();
            m_03.Clear();
            m_04.Clear();
            m_05.Clear();
            m_06.Clear();
            m_07.Clear();
            m_08.Clear();
            m_09.Clear();
            m_10.Clear();
            m_11.Clear();
            m_12.Clear();
            m_13.Clear();
            m_14.Clear();
            m_15.Clear();
            m_16.Clear();
            m_17.Clear();
            m_18.Clear();
            m_19.Clear();
            m_20.Clear();
            m_21.Clear();
            m_22.Clear();
            m_23.Clear();
            m_24.Clear();
            m_25.Clear();
            m_26.Clear();
            m_27.Clear();
            m_28.Clear();
            m_29.Clear();
            m_30.Clear();
            m_31.Clear();
            m_32.Clear();
            m_33.Clear();
            m_34.Clear();
            m_35.Clear();
            m_36.Clear();
            m_37.Clear();
            m_38.Clear();
            m_39.Clear();
            m_40.Clear();
            m_41.Clear();
            m_42.Clear();
            m_43.Clear();
            m_44.Clear();
            m_45.Clear();
            m_46.Clear();
            m_47.Clear();
            m_48.Clear();
            m_49.Clear();
            m_50.Clear();
            m_51.Clear();
            m_52.Clear();
            m_53.Clear();
            m_54.Clear();
            m_55.Clear();
            m_56.Clear();
            m_57.Clear();
            m_58.Clear();
            m_59.Clear();
            m_60.Clear();
            m_61.Clear();
            m_62.Clear();
            m_63.Clear();
            m_64.Clear();
            m_65.Clear();
            m_66.Clear();
            m_67.Clear();
            m_68.Clear();
            m_69.Clear();
            m_70.Clear();
            m_71.Clear();
            m_72.Clear();
            m_73.Clear();
            m_74.Clear();
            m_75.Clear();
            m_76.Clear();
            m_77.Clear();
            m_78.Clear();
            m_79.Clear();
            m_80.Clear();
            m_81.Clear();
            m_82.Clear();
            m_83.Clear();
            m_84.Clear();
            m_85.Clear();
            m_86.Clear();
            m_87.Clear();
            m_88.Clear();
            m_89.Clear();
            m_90.Clear();
            m_91.Clear();
            m_92.Clear();
            m_93.Clear();
            m_94.Clear();
            m_95.Clear();
            m_96.Clear();
            m_97.Clear();
            m_98.Clear();
            m_99.Clear();
        }

        static bool busy___get(string cache_name)
        {
            int index = -1;
            if (string.IsNullOrEmpty(cache_name)) return true;

            for (int i = 0; i < 100; i++) if (m_names[i] == cache_name) { index = i; break; }
            if (index == -1) return true;

            switch (index)
            {
                case 1: return b_01;
                case 2: return b_02;
                case 3: return b_03;
                case 4: return b_04;
                case 5: return b_05;
                case 6: return b_06;
                case 7: return b_07;
                case 8: return b_08;
                case 9: return b_09;
                case 10: return b_10;
                case 11: return b_11;
                case 12: return b_12;
                case 13: return b_13;
                case 14: return b_14;
                case 15: return b_15;
                case 16: return b_16;
                case 17: return b_17;
                case 18: return b_18;
                case 19: return b_19;
                case 20: return b_20;
                case 21: return b_21;
                case 22: return b_22;
                case 23: return b_23;
                case 24: return b_24;
                case 25: return b_25;
                case 26: return b_26;
                case 27: return b_27;
                case 28: return b_28;
                case 29: return b_29;
                case 30: return b_30;
                case 31: return b_31;
                case 32: return b_32;
                case 33: return b_33;
                case 34: return b_34;
                case 35: return b_35;
                case 36: return b_36;
                case 37: return b_37;
                case 38: return b_38;
                case 39: return b_39;
                case 40: return b_40;
                case 41: return b_41;
                case 42: return b_42;
                case 43: return b_43;
                case 44: return b_44;
                case 45: return b_45;
                case 46: return b_46;
                case 47: return b_47;
                case 48: return b_48;
                case 49: return b_49;
                case 50: return b_50;
                case 51: return b_51;
                case 52: return b_52;
                case 53: return b_53;
                case 54: return b_54;
                case 55: return b_55;
                case 56: return b_56;
                case 57: return b_57;
                case 58: return b_58;
                case 59: return b_59;
                case 60: return b_60;
                case 61: return b_61;
                case 62: return b_62;
                case 63: return b_63;
                case 64: return b_64;
                case 65: return b_65;
                case 66: return b_66;
                case 67: return b_67;
                case 68: return b_68;
                case 69: return b_69;
                case 70: return b_70;
                case 71: return b_71;
                case 72: return b_72;
                case 73: return b_73;
                case 74: return b_74;
                case 75: return b_75;
                case 76: return b_76;
                case 77: return b_77;
                case 78: return b_78;
                case 79: return b_79;
                case 80: return b_80;
                case 81: return b_81;
                case 82: return b_82;
                case 83: return b_83;
                case 84: return b_84;
                case 85: return b_85;
                case 86: return b_86;
                case 87: return b_87;
                case 88: return b_88;
                case 89: return b_89;
                case 90: return b_90;
                case 91: return b_91;
                case 92: return b_92;
                case 93: return b_93;
                case 94: return b_94;
                case 95: return b_95;
                case 96: return b_96;
                case 97: return b_97;
                case 98: return b_98;
                case 99: return b_99;
            }

            return true;
        }

        static void busy___set(string cache_name, bool busy_ = true)
        {
            int index = -1;
            if (string.IsNullOrEmpty(cache_name)) return;

            for (int i = 0; i < 100; i++) if (m_names[i] == cache_name) { index = i; break; }
            if (index == -1) return;

            switch (index)
            {
                case 1: b_01 = busy_; break;
                case 2: b_02 = busy_; break;
                case 3: b_03 = busy_; break;
                case 4: b_04 = busy_; break;
                case 5: b_05 = busy_; break;
                case 6: b_06 = busy_; break;
                case 7: b_07 = busy_; break;
                case 8: b_08 = busy_; break;
                case 9: b_09 = busy_; break;
                case 10: b_10 = busy_; break;
                case 11: b_11 = busy_; break;
                case 12: b_12 = busy_; break;
                case 13: b_13 = busy_; break;
                case 14: b_14 = busy_; break;
                case 15: b_15 = busy_; break;
                case 16: b_16 = busy_; break;
                case 17: b_17 = busy_; break;
                case 18: b_18 = busy_; break;
                case 19: b_19 = busy_; break;
                case 20: b_20 = busy_; break;
                case 21: b_21 = busy_; break;
                case 22: b_22 = busy_; break;
                case 23: b_23 = busy_; break;
                case 24: b_24 = busy_; break;
                case 25: b_25 = busy_; break;
                case 26: b_26 = busy_; break;
                case 27: b_27 = busy_; break;
                case 28: b_28 = busy_; break;
                case 29: b_29 = busy_; break;
                case 30: b_30 = busy_; break;
                case 31: b_31 = busy_; break;
                case 32: b_32 = busy_; break;
                case 33: b_33 = busy_; break;
                case 34: b_34 = busy_; break;
                case 35: b_35 = busy_; break;
                case 36: b_36 = busy_; break;
                case 37: b_37 = busy_; break;
                case 38: b_38 = busy_; break;
                case 39: b_39 = busy_; break;
                case 40: b_40 = busy_; break;
                case 41: b_41 = busy_; break;
                case 42: b_42 = busy_; break;
                case 43: b_43 = busy_; break;
                case 44: b_44 = busy_; break;
                case 45: b_45 = busy_; break;
                case 46: b_46 = busy_; break;
                case 47: b_47 = busy_; break;
                case 48: b_48 = busy_; break;
                case 49: b_49 = busy_; break;
                case 50: b_50 = busy_; break;
                case 51: b_51 = busy_; break;
                case 52: b_52 = busy_; break;
                case 53: b_53 = busy_; break;
                case 54: b_54 = busy_; break;
                case 55: b_55 = busy_; break;
                case 56: b_56 = busy_; break;
                case 57: b_57 = busy_; break;
                case 58: b_58 = busy_; break;
                case 59: b_59 = busy_; break;
                case 60: b_60 = busy_; break;
                case 61: b_61 = busy_; break;
                case 62: b_62 = busy_; break;
                case 63: b_63 = busy_; break;
                case 64: b_64 = busy_; break;
                case 65: b_65 = busy_; break;
                case 66: b_66 = busy_; break;
                case 67: b_67 = busy_; break;
                case 68: b_68 = busy_; break;
                case 69: b_69 = busy_; break;
                case 70: b_70 = busy_; break;
                case 71: b_71 = busy_; break;
                case 72: b_72 = busy_; break;
                case 73: b_73 = busy_; break;
                case 74: b_74 = busy_; break;
                case 75: b_75 = busy_; break;
                case 76: b_76 = busy_; break;
                case 77: b_77 = busy_; break;
                case 78: b_78 = busy_; break;
                case 79: b_79 = busy_; break;
                case 80: b_80 = busy_; break;
                case 81: b_81 = busy_; break;
                case 82: b_82 = busy_; break;
                case 83: b_83 = busy_; break;
                case 84: b_84 = busy_; break;
                case 85: b_85 = busy_; break;
                case 86: b_86 = busy_; break;
                case 87: b_87 = busy_; break;
                case 88: b_88 = busy_; break;
                case 89: b_89 = busy_; break;
                case 90: b_90 = busy_; break;
                case 91: b_91 = busy_; break;
                case 92: b_92 = busy_; break;
                case 93: b_93 = busy_; break;
                case 94: b_94 = busy_; break;
                case 95: b_95 = busy_; break;
                case 96: b_96 = busy_; break;
                case 97: b_97 = busy_; break;
                case 98: b_98 = busy_; break;
                case 99: b_99 = busy_; break;
            }
        }

        static string[] m___get_index_ids(string cache_name)
        {
            int index = -1;
            if (string.IsNullOrEmpty(cache_name)) return null;

            for (int i = 0; i < m_names.Length; i++) if (m_names[i] == cache_name) { index = i; break; }
            if (index == -1) return null;

            switch (index)
            {
                case 1: return m_01_ids;
                case 2: return m_02_ids;
                case 3: return m_03_ids;
                case 4: return m_04_ids;
                case 5: return m_05_ids;
                case 6: return m_06_ids;
                case 7: return m_07_ids;
                case 8: return m_08_ids;
                case 9: return m_09_ids;
                case 10: return m_10_ids;
                case 11: return m_11_ids;
                case 12: return m_12_ids;
                case 13: return m_13_ids;
                case 14: return m_14_ids;
                case 15: return m_15_ids;
                case 16: return m_16_ids;
                case 17: return m_17_ids;
                case 18: return m_18_ids;
                case 19: return m_19_ids;
                case 20: return m_20_ids;
                case 21: return m_21_ids;
                case 22: return m_22_ids;
                case 23: return m_23_ids;
                case 24: return m_24_ids;
                case 25: return m_25_ids;
                case 26: return m_26_ids;
                case 27: return m_27_ids;
                case 28: return m_28_ids;
                case 29: return m_29_ids;
                case 30: return m_30_ids;
                case 31: return m_31_ids;
                case 32: return m_32_ids;
                case 33: return m_33_ids;
                case 34: return m_34_ids;
                case 35: return m_35_ids;
                case 36: return m_36_ids;
                case 37: return m_37_ids;
                case 38: return m_38_ids;
                case 39: return m_39_ids;
                case 40: return m_40_ids;
                case 41: return m_41_ids;
                case 42: return m_42_ids;
                case 43: return m_43_ids;
                case 44: return m_44_ids;
                case 45: return m_45_ids;
                case 46: return m_46_ids;
                case 47: return m_47_ids;
                case 48: return m_48_ids;
                case 49: return m_49_ids;
                case 50: return m_50_ids;
                case 51: return m_51_ids;
                case 52: return m_52_ids;
                case 53: return m_53_ids;
                case 54: return m_54_ids;
                case 55: return m_55_ids;
                case 56: return m_56_ids;
                case 57: return m_57_ids;
                case 58: return m_58_ids;
                case 59: return m_59_ids;
                case 60: return m_60_ids;
                case 61: return m_61_ids;
                case 62: return m_62_ids;
                case 63: return m_63_ids;
                case 64: return m_64_ids;
                case 65: return m_65_ids;
                case 66: return m_66_ids;
                case 67: return m_67_ids;
                case 68: return m_68_ids;
                case 69: return m_69_ids;
                case 70: return m_70_ids;
                case 71: return m_71_ids;
                case 72: return m_72_ids;
                case 73: return m_73_ids;
                case 74: return m_74_ids;
                case 75: return m_75_ids;
                case 76: return m_76_ids;
                case 77: return m_77_ids;
                case 78: return m_78_ids;
                case 79: return m_79_ids;
                case 80: return m_80_ids;
                case 81: return m_81_ids;
                case 82: return m_82_ids;
                case 83: return m_83_ids;
                case 84: return m_84_ids;
                case 85: return m_85_ids;
                case 86: return m_86_ids;
                case 87: return m_87_ids;
                case 88: return m_88_ids;
                case 89: return m_89_ids;
                case 90: return m_90_ids;
                case 91: return m_91_ids;
                case 92: return m_92_ids;
                case 93: return m_93_ids;
                case 94: return m_94_ids;
                case 95: return m_95_ids;
                case 96: return m_96_ids;
                case 97: return m_97_ids;
                case 98: return m_98_ids;
                case 99: return m_99_ids;
            }

            return null;
        }

        static string[] m___get_index_ascii(string cache_name)
        {
            int index = -1;
            if (string.IsNullOrEmpty(cache_name)) return null;

            for (int i = 0; i < m_names.Length; i++) if (m_names[i] == cache_name) { index = i; break; }
            if (index == -1) return null;

            switch (index)
            {
                case 1: return m_01_ascii;
                case 2: return m_02_ascii;
                case 3: return m_03_ascii;
                case 4: return m_04_ascii;
                case 5: return m_05_ascii;
                case 6: return m_06_ascii;
                case 7: return m_07_ascii;
                case 8: return m_08_ascii;
                case 9: return m_09_ascii;
                case 10: return m_10_ascii;
                case 11: return m_11_ascii;
                case 12: return m_12_ascii;
                case 13: return m_13_ascii;
                case 14: return m_14_ascii;
                case 15: return m_15_ascii;
                case 16: return m_16_ascii;
                case 17: return m_17_ascii;
                case 18: return m_18_ascii;
                case 19: return m_19_ascii;
                case 20: return m_20_ascii;
                case 21: return m_21_ascii;
                case 22: return m_22_ascii;
                case 23: return m_23_ascii;
                case 24: return m_24_ascii;
                case 25: return m_25_ascii;
                case 26: return m_26_ascii;
                case 27: return m_27_ascii;
                case 28: return m_28_ascii;
                case 29: return m_29_ascii;
                case 30: return m_30_ascii;
                case 31: return m_31_ascii;
                case 32: return m_32_ascii;
                case 33: return m_33_ascii;
                case 34: return m_34_ascii;
                case 35: return m_35_ascii;
                case 36: return m_36_ascii;
                case 37: return m_37_ascii;
                case 38: return m_38_ascii;
                case 39: return m_39_ascii;
                case 40: return m_40_ascii;
                case 41: return m_41_ascii;
                case 42: return m_42_ascii;
                case 43: return m_43_ascii;
                case 44: return m_44_ascii;
                case 45: return m_45_ascii;
                case 46: return m_46_ascii;
                case 47: return m_47_ascii;
                case 48: return m_48_ascii;
                case 49: return m_49_ascii;
                case 50: return m_50_ascii;
                case 51: return m_51_ascii;
                case 52: return m_52_ascii;
                case 53: return m_53_ascii;
                case 54: return m_54_ascii;
                case 55: return m_55_ascii;
                case 56: return m_56_ascii;
                case 57: return m_57_ascii;
                case 58: return m_58_ascii;
                case 59: return m_59_ascii;
                case 60: return m_60_ascii;
                case 61: return m_61_ascii;
                case 62: return m_62_ascii;
                case 63: return m_63_ascii;
                case 64: return m_64_ascii;
                case 65: return m_65_ascii;
                case 66: return m_66_ascii;
                case 67: return m_67_ascii;
                case 68: return m_68_ascii;
                case 69: return m_69_ascii;
                case 70: return m_70_ascii;
                case 71: return m_71_ascii;
                case 72: return m_72_ascii;
                case 73: return m_73_ascii;
                case 74: return m_74_ascii;
                case 75: return m_75_ascii;
                case 76: return m_76_ascii;
                case 77: return m_77_ascii;
                case 78: return m_78_ascii;
                case 79: return m_79_ascii;
                case 80: return m_80_ascii;
                case 81: return m_81_ascii;
                case 82: return m_82_ascii;
                case 83: return m_83_ascii;
                case 84: return m_84_ascii;
                case 85: return m_85_ascii;
                case 86: return m_86_ascii;
                case 87: return m_87_ascii;
                case 88: return m_88_ascii;
                case 89: return m_89_ascii;
                case 90: return m_90_ascii;
                case 91: return m_91_ascii;
                case 92: return m_92_ascii;
                case 93: return m_93_ascii;
                case 94: return m_94_ascii;
                case 95: return m_95_ascii;
                case 96: return m_96_ascii;
                case 97: return m_97_ascii;
                case 98: return m_98_ascii;
                case 99: return m_99_ascii;
            }

            return null;
        }

        static string[] m___get_index_utf8(string cache_name)
        {
            int index = -1;
            if (string.IsNullOrEmpty(cache_name)) return null;

            for (int i = 0; i < m_names.Length; i++) if (m_names[i] == cache_name) { index = i; break; }
            if (index == -1) return null;

            switch (index)
            {
                case 1: return m_01_utf8;
                case 2: return m_02_utf8;
                case 3: return m_03_utf8;
                case 4: return m_04_utf8;
                case 5: return m_05_utf8;
                case 6: return m_06_utf8;
                case 7: return m_07_utf8;
                case 8: return m_08_utf8;
                case 9: return m_09_utf8;
                case 10: return m_10_utf8;
                case 11: return m_11_utf8;
                case 12: return m_12_utf8;
                case 13: return m_13_utf8;
                case 14: return m_14_utf8;
                case 15: return m_15_utf8;
                case 16: return m_16_utf8;
                case 17: return m_17_utf8;
                case 18: return m_18_utf8;
                case 19: return m_19_utf8;
                case 20: return m_20_utf8;
                case 21: return m_21_utf8;
                case 22: return m_22_utf8;
                case 23: return m_23_utf8;
                case 24: return m_24_utf8;
                case 25: return m_25_utf8;
                case 26: return m_26_utf8;
                case 27: return m_27_utf8;
                case 28: return m_28_utf8;
                case 29: return m_29_utf8;
                case 30: return m_30_utf8;
                case 31: return m_31_utf8;
                case 32: return m_32_utf8;
                case 33: return m_33_utf8;
                case 34: return m_34_utf8;
                case 35: return m_35_utf8;
                case 36: return m_36_utf8;
                case 37: return m_37_utf8;
                case 38: return m_38_utf8;
                case 39: return m_39_utf8;
                case 40: return m_40_utf8;
                case 41: return m_41_utf8;
                case 42: return m_42_utf8;
                case 43: return m_43_utf8;
                case 44: return m_44_utf8;
                case 45: return m_45_utf8;
                case 46: return m_46_utf8;
                case 47: return m_47_utf8;
                case 48: return m_48_utf8;
                case 49: return m_49_utf8;
                case 50: return m_50_utf8;
                case 51: return m_51_utf8;
                case 52: return m_52_utf8;
                case 53: return m_53_utf8;
                case 54: return m_54_utf8;
                case 55: return m_55_utf8;
                case 56: return m_56_utf8;
                case 57: return m_57_utf8;
                case 58: return m_58_utf8;
                case 59: return m_59_utf8;
                case 60: return m_60_utf8;
                case 61: return m_61_utf8;
                case 62: return m_62_utf8;
                case 63: return m_63_utf8;
                case 64: return m_64_utf8;
                case 65: return m_65_utf8;
                case 66: return m_66_utf8;
                case 67: return m_67_utf8;
                case 68: return m_68_utf8;
                case 69: return m_69_utf8;
                case 70: return m_70_utf8;
                case 71: return m_71_utf8;
                case 72: return m_72_utf8;
                case 73: return m_73_utf8;
                case 74: return m_74_utf8;
                case 75: return m_75_utf8;
                case 76: return m_76_utf8;
                case 77: return m_77_utf8;
                case 78: return m_78_utf8;
                case 79: return m_79_utf8;
                case 80: return m_80_utf8;
                case 81: return m_81_utf8;
                case 82: return m_82_utf8;
                case 83: return m_83_utf8;
                case 84: return m_84_utf8;
                case 85: return m_85_utf8;
                case 86: return m_86_utf8;
                case 87: return m_87_utf8;
                case 88: return m_88_utf8;
                case 89: return m_89_utf8;
                case 90: return m_90_utf8;
                case 91: return m_91_utf8;
                case 92: return m_92_utf8;
                case 93: return m_93_utf8;
                case 94: return m_94_utf8;
                case 95: return m_95_utf8;
                case 96: return m_96_utf8;
                case 97: return m_97_utf8;
                case 98: return m_98_utf8;
                case 99: return m_99_utf8;
            }

            return null;
        }

        #endregion

        static ConcurrentDictionary<string, string> m_views = new ConcurrentDictionary<string, string>();
        public void view___reload()
        {

            string sites = Path.Combine(_CONFIG.PATH_ROOT, "_site");
            if (Directory.Exists(sites))
            {
                foreach (string site in Directory.GetDirectories(sites))
                {
                    string views = Path.Combine(site, "_view");
                    string domain = Path.GetFileName(site);
                    if (Directory.Exists(views))
                    {
                        string[] fs = Directory.GetFiles(views, "*.html");
                        foreach (string fi in fs)
                        {
                            string file_name = Path.GetFileName(fi);
                            file_name = file_name.Substring(0, file_name.Length - 5);
                            string key = domain + "/" + file_name;
                            m_views.TryAdd(key.ToLower(), File.ReadAllText(fi));
                        }
                    }
                }
            }
        }

        public string view___build(string html, Uri uri, string wroot)
        {
            string s = html;

            var rs = Regex.Matches(html, @"<!--(.|\n)*?-->");
            foreach (var m in rs)
            {
                string key_replace = m.ToString();
                string key = key_replace.Substring(4, key_replace.Length - 7).Trim();
                if (key[0] == '{' && key[key.Length - 1] == '}')
                {
                    //if (m_views.ContainsKey(key))
                    //key = (uri.Host + "/" + key.Substring(1, key.Length - 2).Trim()).ToLower();
                    //if (m_views.ContainsKey(key)) s = s.Replace(key_replace, m_views[key]);
                    string file = Path.Combine(_CONFIG.PATH_ROOT + "_site\\" + wroot + "\\_view\\" + key.Substring(1, key.Length - 2).Trim()) + ".html";
                    if (File.Exists(file))
                        s = s.Replace(key_replace, File.ReadAllText(file));
                }
            }

            return s;
        }

        public bool existCache_Data(string cache_name) { return m___check(cache_name); }

        static ConcurrentDictionary<string, string> m_storeApiJS = new ConcurrentDictionary<string, string>() { };
        public void reloadCache_ApiJS(string cache_name = null, string api_name = null)
        {
            string dir = Path.Combine(_CONFIG.PATH_ROOT, "_api");
            if (Directory.Exists(dir))
            {
                var fs = Directory.GetFiles(dir, "*.js");
                foreach (var f in fs)
                {
                    string file_name = Path.GetFileName(f);
                    file_name = file_name.Substring(0, file_name.Length - 3).ToLower();
                    if (file_name.Contains("___"))
                        m_storeApiJS.TryAdd(file_name, File.ReadAllText(f));
                }
            }
        }

        public bool existCache_ApiJS(string cache_name, string api_name)
        {
            string file = Path.Combine(_CONFIG.PATH_ROOT, "_api\\" + cache_name + "___" + api_name + ".js");
            return File.Exists(file);
        }
    }

    #endregion

    #region [ JOB ]

    public enum JOB_TYPE
    {
        NONE = 0,
        CRAWLER_NET = 1,
        CRAWLER_CURL = 2,
        API_JS = 3
    }

    public class JobApiJS : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var para = context.getParaInput();

            //var api = para.getValue<string>("api___");
            //if (string.IsNullOrWhiteSpace(api)) { 

            //}

            context.log("key_name_1", "This is content of JOB. It executing...");
            await Task.FromResult(false);
        }
    }

    public class JobCrawler : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var para = context.getParaInput();

            //var api = para.getValue<string>("api___");
            //if (string.IsNullOrWhiteSpace(api)) { 

            //}

            context.log("key_name_1", "This is content of JOB. It executing...");
            await Task.FromResult(false);
        }
    }

    public class JobEmail : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            using (var message = new MailMessage("user@gmail.com", "user@live.co.uk"))
            {
                message.Subject = "Test";
                message.Body = "Test at " + DateTime.Now;
                using (SmtpClient client = new SmtpClient
                {
                    EnableSsl = true,
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential("user@gmail.com", "password")
                })
                {
                    client.Send(message);
                }
            }

            return Task.FromResult<bool>(true);
        }
    }

    public class JobListener : IJobListener
    {
        public string Name { get { return "clsJobListener"; } }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            if (dataMap.ContainsKey("CURRENT_ID___"))
            {
                long CURRENT_ID___ = long.Parse(DateTime.Now.ToString("yyMMddHHmmss"));
                dataMap.Put("CURRENT_ID___", CURRENT_ID___);

                if (dataMap.ContainsKey("COUNTER___"))
                {
                    var state = (ConcurrentDictionary<long, bool>)dataMap["COUNTER___"];
                    state.TryAdd(CURRENT_ID___, false);
                }
            }
            return Task.FromResult(false);
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            if (dataMap.ContainsKey("CURRENT_ID___"))
            {
                long CURRENT_ID___ = dataMap.GetLong("CURRENT_ID___");
                if (dataMap.ContainsKey("COUNTER___"))
                {
                    var state = (ConcurrentDictionary<long, bool>)dataMap["COUNTER___"];
                    state[CURRENT_ID___] = true;
                }
            }
            return Task.FromResult(false);
        }
    }

    #endregion

    #region [ JS ENGINE ]

    public class clsEngineJS
    {
        static V8ScriptEngine m_engine;
        static IApi m_api;
        public static void _init(IApi api)
        {
            m_api = api;
            m_engine = new V8ScriptEngine();
            m_engine.AddHostType("Action", typeof(Action));
            m_engine.AddHostObject("log___", new ejsConsole());
            m_engine.AddHostObject("___api", api);
        }

        public static string Execute(string api_name, Dictionary<string, object> parameters = null, Dictionary<string, object> request = null)
        {
            if (request == null) request = new Dictionary<string, object>();
            if (parameters == null) parameters = new Dictionary<string, object>();

            oResult r = new oResult()
            {
                ok = false,
                name = api_name,
                input = parameters,
                request = request
            };

            string file_main = Path.Combine(_CONFIG.PATH_ROOT, "_api\\app.js");
            if (File.Exists(file_main) == false)
            {
                r.error = "ERROR[clsEngineJS.Execute] Cannot found file: app.js ";
                return JsonConvert.SerializeObject(r);
            }

            string file_api = Path.Combine(_CONFIG.PATH_ROOT, "_api\\" + api_name + ".js");
            if (File.Exists(file_api) == false)
            {
                r.error = "ERROR[clsEngineJS.Execute] Cannot found file: " + file_api;
                return JsonConvert.SerializeObject(r);
            }

            try
            {
                string js_main = File.ReadAllText(file_main);
                string js_api = File.ReadAllText(file_api);

                js_main = js_main.Replace("[API_NAME]", api_name)
                    .Replace("[TEXT_REQUEST]", JsonConvert.SerializeObject(request))
                    .Replace("[TEXT_PARAMETER]", JsonConvert.SerializeObject(parameters))
                    .Replace("[EXECUTE_TEXT_JS]", js_api);

                var toReturn = m_engine.Evaluate(js_main);
                if (toReturn is Microsoft.ClearScript.Undefined)
                {
                    r.ok = true;
                }
                else
                {
                    string json = toReturn.ToString();
                    return json;
                }

            }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: " + e.Message;
            }
            return JsonConvert.SerializeObject(r);
        }
    }

    public class ejsConsole
    {
        readonly ILog m_log;
        public ejsConsole()
        {
            m_log = new LogRedis();
        }

        public void write(string scope_name, string key, string text)
        {
            m_log.write(scope_name, key, text);
        }
    }

    #endregion

    #region [ MODEL ]

    public class oUser
    {
        public string session_id { set; get; }
        public oUserZalo zalo_info { set; get; }

        [JsonIgnore]
        public Zalo3rdAppClient ZaloClient { set; get; }
    }

    public class oUserZalo
    {
        public long user_id { set; get; }
        public string name { set; get; }
        public string birthday { set; get; }
        public string code { set; get; }
        public string access_token { set; get; }
    }

    public class oZaloResult
    {
        public int error { set; get; }
        public string message { set; get; }
        public oUser user_info { set; get; }
    }

    #endregion

    public static class Utility
    {
        public static Dictionary<string, object> get_request_headers(System.Web.HttpApplication app)
        {
            var Request = app.Request;

            string path = Request.Url.AbsolutePath.Substring(1).ToLower();
            Dictionary<string, object> para = new Dictionary<string, object>() {
                { "___domain", Request.Url.Host },
                { "___port", Request.Url.Port },
                { "___url", path },
                { "___method", Request.HttpMethod },
                //{ "___token", string.Empty },
            };

            try
            {
                int key_size = Request.Headers.Keys.Count;
                for (int i = 0; i < key_size; i++)
                {
                    string key_header = Request.Headers.Keys.Get(i);
                    string val_header = Request.Headers.Get(key_header);
                    if (para.ContainsKey(key_header))
                        para[key_header] = val_header;
                    else para.Add(key_header, val_header);
                }

                var a = Request.QueryString.Keys.Cast<string>().ToArray();
                foreach (var key in a) if (!para.ContainsKey(key)) para.Add(key, Uri.UnescapeDataString(Request.QueryString[key]));
            }
            catch { }

            return para;
        }

        public static Dictionary<string, object> get_request_parameters(System.Web.HttpApplication app)
        {
            var Request = app.Request;

            string path = Request.Url.AbsolutePath.Substring(1).ToLower();
            Dictionary<string, object> para = new Dictionary<string, object>();

            try
            {
                var a = Request.QueryString.Keys.Cast<string>().ToArray();
                foreach (var key in a) if (!para.ContainsKey(key)) para.Add(key, Uri.UnescapeDataString(Request.QueryString[key]));

                if (Request.HttpMethod == "POST")
                {
                    string s = new StreamReader(Request.InputStream).ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(s))
                    {
                        s = s.Trim();
                        if (s.Length > 1 && s[0] == '{' && s[s.Length - 1] == '}')
                        {
                            try
                            {
                                var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(s);
                                foreach (var kv in d) if (!para.ContainsKey(kv.Key)) para.Add(kv.Key, kv.Value);
                            }
                            catch
                            {
                                para.Add("___BODY", s);
                            }
                        }
                        else
                        {
                            para.Add("___BODY", s);
                        }
                    }
                }
            }
            catch { }

            return para;
        }
    }

    public class clsGlobal
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

        public void clearAllJobs()
        {
            m_scheduler.Shutdown();
            m_scheduler.Start();
            m_job.Clear();
            m_trigger.Clear();
        }

        public static string[] list_jobs()
        {
            var a = m_scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup())
                .GetAwaiter().GetResult().Select(x => x.Name).ToArray();
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

        static ConcurrentDictionary<string, oUser> m_users = new ConcurrentDictionary<string, oUser>();

        static ConcurrentDictionary<string, string> m_domains = new ConcurrentDictionary<string, string>();
        static void domain___reload()
        {
            string file = Path.Combine(_CONFIG.PATH_ROOT, "_site\\site.json");
            if (File.Exists(file))
            {
                try
                {
                    var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(file));
                    foreach (var kv in dic) m_domains.TryAdd(kv.Key, kv.Value);
                }
                catch { }
            }
        }

        public static void Start(System.Web.HttpApplication app, NameValueCollection appSettings)
        {
            _CONFIG._init(app, appSettings);
            domain___reload();
            _init_job();
            m_cache = new clsCache();
            m_api = new clsApi(m_cache);
            clsEngineJS._init(m_api);
        }

        public static void BeginRequest(System.Web.HttpApplication app)
        {
            m_cache.view___reload();

            var Response = app.Response;
            var Request = app.Request;

            Uri uri = Request.Url;
            string url = uri.ToString();
            string ___DOMAIN = uri.Host, ___WROOT = string.Empty;
            if (uri.Port != 80 && uri.Port != 443) ___DOMAIN += ":" + uri.Port;
            string path = uri.AbsolutePath.Substring(1);

            #region [ ZALO ]

            if (path.StartsWith("zalo/login"))
            {
                ZaloAppInfo appInfo = new ZaloAppInfo(4493888734077794545, "2KOY8CIBqKEwbGJ7TV1k", "http://zalo.iot.vn");
                Zalo3rdAppClient appClient = new Zalo3rdAppClient(appInfo);
                string loginUrl = appClient.getLoginUrl();
                string id = "zalo-" + Guid.NewGuid().ToString().Substring(5);
                if (loginUrl.Contains("&state=") == false) loginUrl += "&state=" + id;
                m_users.TryAdd(id, new oUser() { session_id = id, ZaloClient = appClient, zalo_info = new oUserZalo() });
                Response.Redirect(loginUrl);
                return;
            }

            if (path.StartsWith("zalo/list-user"))
            {
                string json = JsonConvert.SerializeObject(m_users.Values);
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
                return;
            }

            // Callback of Zalo authentication
            if (url.Contains("uid=") && url.Contains("code=") && url.Contains("state="))
            {
                string zalo_session_id = Request.QueryString["state"],
                    zalo_code = Request.QueryString["code"],
                    zalo_uid = Request.QueryString["uid"],
                    zalo_json = "{}";

                if (!string.IsNullOrEmpty(zalo_session_id) && m_users.ContainsKey(zalo_session_id))
                {
                    try
                    {
                        oUser u = m_users[zalo_session_id];
                        u.zalo_info.access_token = zalo_code;
                        u.zalo_info.user_id = long.Parse(zalo_uid);

                        var jo_token = u.ZaloClient.getAccessToken(zalo_code);
                        var access_token = jo_token["access_token"].ToString();

                        var jo_profile = u.ZaloClient.getProfile(access_token, "id, name, birthday");
                        u.zalo_info.name = jo_profile["name"].ToString();
                        u.zalo_info.birthday = jo_profile["birthday"].ToString();

                        var jo_friends = u.ZaloClient.getFriends(access_token, 0, 3, "id, name, picture");
                        string list_friends = JsonConvert.SerializeObject(jo_friends);

                        //var jo_send_msg = u.ZaloClient.sendMessage(access_token, 7900271606406461606, Guid.NewGuid().ToString(), "https://vnexpress.net");
                        //var jo_friends = u.ZaloClient.getInvitableFriends(access_token, 0, 3, "id, name, picture");
                        //string list_friends = JsonConvert.SerializeObject(jo_friends);

                        string zalo_result = JsonConvert.SerializeObject(jo_profile);
                        var o = JsonConvert.DeserializeObject<oZaloResult>(zalo_result);
                        o.user_info = u;
                        zalo_json = JsonConvert.SerializeObject(o);
                    }
                    catch { }
                }

                Response.ContentType = "application/json";
                Response.Write(zalo_json);
                Response.End();
                return;
            }

            #endregion

            #region [ HTML, CSS, JS, IMAGES, FONTS ... ]

            if (m_domains.Count == 0 || m_domains.ContainsKey(___DOMAIN) == false)
            {
                Response.ContentType = "application/json";
                Response.Write(@"{""ok"":false,""message"":""Cannot find setting domain in site.json""}");
                Response.End();
                return;
            }
            ___WROOT = m_domains[___DOMAIN];

            if (path == "favicon.ico") { Response.End(); return; }

            //[1] api/{cache_name}/{api_name}/{id}
            if (path.StartsWith("api/"))
            {
                string[] a = path.Split('/');

                var req_headers = Utility.get_request_headers(app);
                var req_paramenters = Utility.get_request_parameters(app);
                string json = clsEngineJS.Execute(a[1] + "." + a[2], req_paramenters, req_headers);

                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();

                return;
            }

            //[2] Files
            string file = string.Empty;
            bool isHome = false;
            if (path.Length == 0) { isHome = true; path = "index.html"; }

            if (path.EndsWith(".html")
                || path.StartsWith("data/")
                || path.StartsWith("file/"))
                path = "_site\\" + ___WROOT + "\\" + path;

            if (path == "admin") path = "admin.html";

            string contentType = MimeTypes.GetMimeType(path);

            if (path[0] != '_' // break dirs: _lib, _view,..
                && contentType != "text/html" // break files *.html
                && Request.UrlReferrer != null) // Ref from other request as request on page html
                path = "_site\\" + ___WROOT + "\\" + path;

            file = Path.Combine(_CONFIG.PATH_ROOT, path).Replace("/", "\\");

            if (file.Contains("fonts"))
                ;

            if (File.Exists(file))
            {
                if (contentType == "text/html")
                {
                    string html = File.ReadAllText(file);
                    html = m_cache.view___build(html, uri, ___WROOT);
                    Response.Write(html);
                }
                else
                {
                    Response.ContentType = contentType;
                    Response.TransmitFile(file);
                }
            }
            else
            {
                if (isHome)
                {
                    Response.ContentType = contentType;
                    Response.Write("<h1>Cannot found page " + path + "</h1>");
                }
                else
                    Response.StatusCode = 404;
            }

            Response.End();

            #endregion
        }
    }
}