﻿using Microsoft.ClearScript.V8;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace test
{
    public class clsEngineJS
    {
        static V8ScriptEngine m_engine;
        static clsApi m_api;
        public static void _init(clsApi api)
        {
            m_api = api;
            m_engine = new V8ScriptEngine();
            //m_engine.AddHostType("Action", typeof(Action));
            m_engine.AddHostObject("log___", new ejsConsole());
            m_engine.AddHostObject("___api", api);
        }

        public static oResult Execute(string file___api, Dictionary<string, object> parameters = null, Dictionary<string, object> request = null)
        {
            if (request == null) request = new Dictionary<string, object>();
            if (parameters == null) parameters = new Dictionary<string, object>();

            oResult r = new oResult()
            {
                ok = false,
                name = file___api,
                input = parameters,
                request = request
            };

            string file = Path.Combine(_CONFIG.PATH_ROOT, "_api\\" + file___api + ".js");
            if (File.Exists(file) == false)
            {
                r.error = "ERROR[clsEngineJS.Execute] Cannot found file: " + file;
                return r;
            }

            if (!file.Contains("___"))
            {
                r.error = "ERROR[clsEngineJS.Execute] file___api: " + file___api + " must be format cache_name___api_name ";
                return r;
            }

            string[] a = file___api.Split(new string[] { "___" }, StringSplitOptions.None);
            string cache_name = a[0], api_name = a[1];

            try
            {
                string js = string.Empty;
                js = @"
                (function() {
                    'use strict';
                    try {
                        var ___log_text = function(text){ log___.write('" + file___api + @"', '_', text); };
                        var ___log_key = function(key, text){ log___.write('" + file___api + @"', key, text); };
                        var ___api_call = function(api_name, paramenter, request){ return log___[""api_call""](JSON.stringify(paramenter), JSON.stringify(request)); };

                        var ___request = " + JsonConvert.SerializeObject(request) + @";
                        var ___parameters = " + JsonConvert.SerializeObject(parameters) + @";

                        ___log_text('This is from JavaScript...');

                        " + File.ReadAllText(file) + @"

                    } catch(e) {
                        return { ok: false, name: '" + file___api + @"', error: e.message };
                    }
                })();
                ";

                var toReturn = m_engine.Evaluate(js);
                if (toReturn is Microsoft.ClearScript.Undefined)
                {
                    r.ok = true;
                }
                else if (toReturn is oResult)
                {
                    r = (oResult)toReturn;
                    r.request = request;
                    r.input = parameters;
                    r.name = file___api;
                }
                else
                {
                    r.error = "ERROR: Expected TYPE of result but got " + toReturn.GetType().FullName;
                }
            }
            catch (Exception e)
            {
                r.error = "ERROR_THROW: " + e.Message;
            }
            return r;
        }
    }

    public class ejsConsole
    {
        readonly ILog m_log;
        public ejsConsole()
        {
            m_log = new LogRedis();
        }

        public oResult api_call(string paramenter = null, string request = null)
        {
            Dictionary<string, object> para = null;
            Dictionary<string, object> req = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(paramenter))
                    para = JsonConvert.DeserializeObject<Dictionary<string, object>>(paramenter);

                if (!string.IsNullOrWhiteSpace(request))
                    req = JsonConvert.DeserializeObject<Dictionary<string, object>>(request);
            }
            catch (Exception ex)
            {

            }

            return oResult.Ok();
        }

        public void write(string scope_name, string key, string text)
        {
            m_log.write(scope_name, key, text);
        }
    }
}