using Microsoft.ClearScript.V8;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;

namespace test
{
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

        string js_call(string api_name, string paramenter = null, string request = null);
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
            try {
                #region [ request_async ]

                //////if (paramenter.Count > 0 && paramenter.ContainsKey("headers"))
                //////{
                //////    #region

                //////    Dictionary<string, object> headers = null;

                //////    if (paramenter.ContainsKey("headers") == false)
                //////    {
                //////        r.error = "ERROR_request_async: The paramenter [headers] not exist";
                //////        return r;
                //////    }

                //////    if (paramenter.ContainsKey("headers"))
                //////    {
                //////        try
                //////        {
                //////            headers = ((JObject)paramenter["headers"]).ToObject<Dictionary<string, object>>();
                //////        }
                //////        catch (Exception e)
                //////        {
                //////            r.error = "ERROR_request_async: The paramenter [headers] must be format JSON. " + e.Message;
                //////            return r;
                //////        }
                //////    }

                //////    if (headers.Count == 0
                //////        || headers.ContainsKey("url") == false || headers["url"] == null || string.IsNullOrEmpty(headers["url"].ToString())
                //////        || headers.ContainsKey("method") == false || headers["method"] == null || string.IsNullOrEmpty(headers["method"].ToString()))
                //////    {
                //////        r.error = "ERROR_request_async: The paramenter [headers] must be { url:..., method:... } and Value is not null or empty";
                //////        return r;
                //////    }

                //////    #endregion

                //////    try
                //////    {
                //////        string httpMethod = "GET";
                //////        string type = "v8";
                //////        string url = string.Empty;
                //////        string htm = string.Empty;

                //////        foreach (var header in headers)
                //////        {
                //////            if (header.Key.StartsWith("Content") || header.Key == "data") continue;

                //////            if (header.Key == "url")
                //////            {
                //////                url = header.Value.ToString();
                //////                continue;
                //////            }

                //////            if (header.Key == "method")
                //////            {
                //////                httpMethod = header.Value.ToString();
                //////                continue;
                //////            }

                //////            if (header.Key == "type")
                //////            {
                //////                type = header.Value.ToString();
                //////                continue;
                //////            }
                //////        }
                //////        switch (type)
                //////        {
                //////            case "curl":
                //////                htm = clsCURL.___https(url);
                //////                r.ok = true;
                //////                r.data = htm;
                //////                break;
                //////            case "v8":
                //////                try
                //////                {
                //////                    V8ScriptEngine engine = new V8ScriptEngine(V8ScriptEngineFlags.DisableGlobalMembers);
                //////                    engine.AddCOMType("XMLHttpRequest", "MSXML2.XMLHTTP");
                //////                    engine.Execute(@" function get(url) { var xhr = new XMLHttpRequest(); xhr.open('GET', url, false); xhr.send(); if (xhr.status == 200) return xhr.responseText; else return ''; }");
                //////                    htm = engine.Script.get(url);
                //////                    engine.Dispose();

                //////                    r.ok = true;
                //////                    r.data = htm;
                //////                }
                //////                catch (Exception e1)
                //////                {
                //////                    //htm = e1.Message;
                //////                    r.error = "ERROR_XHR_request_async: " + e1.Message;
                //////                    return r;
                //////                }
                //////                break;
                //////            default:
                //////                using (var httpClient = new HttpClient())
                //////                {
                //////                    foreach (var header in headers)
                //////                    {
                //////                        if (header.Key.StartsWith("Content") || header.Key == "type" || header.Key == "data" || header.Key == "url" || header.Key == "method") continue;
                //////                        if (header.Value != null) httpClient.DefaultRequestHeaders.Add(header.Key, header.Value.ToString());
                //////                    }

                //////                    //============================================================
                //////                    //ServicePointManager.CertificatePolicy = new MyPolicy();
                //////                    //ServicePointManager.Expect100Continue = true;
                //////                    //ServicePointManager.DefaultConnectionLimit = 9999;
                //////                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;

                //////                    //============================================================
                //////                    HttpResponseMessage responseMessage = null;
                //////                    switch (httpMethod)
                //////                    {
                //////                        case "DELETE":
                //////                            responseMessage = httpClient.DeleteAsync(url).Result;
                //////                            break;
                //////                        case "PATCH":
                //////                        case "POST":
                //////                            string data = string.Empty;

                //////                            if (paramenter.ContainsKey("data") && paramenter[data] != null) 
                //////                                data = paramenter["data"].ToString();

                //////                            responseMessage = httpClient.PostAsync(url, new StringContent(data)).Result;

                //////                            break;
                //////                        case "GET":
                //////                            responseMessage = httpClient.GetAsync(url).Result;
                //////                            break;
                //////                    }

                //////                    if (responseMessage != null)
                //////                        using (responseMessage)
                //////                        using (var content = responseMessage.Content)
                //////                            htm = content.ReadAsStringAsync().Result.Trim();

                //////                    if (htm.Length > 0)
                //////                    {
                //////                        r.ok = true;
                //////                        r.data = htm;
                //////                    }
                //////                }
                //////                break;
                //////        }
                //////    }
                //////    catch (Exception e)
                //////    {
                //////        r.error = "ERROR_THROW_request_async: " + e.Message;
                //////        return r;
                //////    }
                //////}

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

        public string js_call(string api_name, string paramenter = null, string request = null)
        {
            oResult r = new oResult() { ok = false };

            Dictionary<string, object> para = null;
            Dictionary<string, object> req = null;

            if (string.IsNullOrWhiteSpace(paramenter)) para = new Dictionary<string, object>();
            if (string.IsNullOrWhiteSpace(request)) req = new Dictionary<string, object>();

            try
            {
                if (!string.IsNullOrWhiteSpace(paramenter))
                    para =  JsonConvert.DeserializeObject<Dictionary<string, object>>(paramenter);
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
}