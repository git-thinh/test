using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace viewjson
{
    class app
    {
        public string scope { set; get; }
        public string name { set; get; }
        public string key { set; get; }
        public string title { set; get; }
        public string[] files { set; get; }
        public app()
        {
            this.scope = string.Empty;
            this.name = string.Empty;
            this.key = string.Empty;
            this.title = string.Empty;
            this.files = new string[] { };
        }

        public override string ToString()
        {
            return string.Format("{0}___{1}", scope, name);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<app> list = new List<app>();
            if (Directory.Exists("app"))
            {
                string[] dirs = Directory.GetDirectories("app");
                foreach (var path_dir in dirs)
                {
                    string scope = Path.GetFileName(path_dir);

                    string[] apps = Directory.GetDirectories(path_dir);
                    foreach (var path_app in apps)
                    {
                        app o = new app();
                        o.scope = scope;
                        o.name = Path.GetFileName(path_app);
                        o.key = string.Format("{0}___{1}", o.scope, o.name);

                        var fs = Directory.GetFiles(path_app);
                        List<string> lfs = new List<string>();
                        foreach (var fi in fs) lfs.Add(Path.GetFileName(fi));
                        o.files = lfs.ToArray();

                        list.Add(o);
                    }
                }
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("app/list.json", json);
        }
    }
}
