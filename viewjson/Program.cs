using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace viewjson
{
    #region [ MODELS ]

    class oPermistion
    {
        public string[] users { set; get; }
        public string[] groups { set; get; }
        public oPermistion()
        {
            this.users = new string[] { "*" };
            this.groups = new string[] { "*" };
        }
    }

    class oAction
    {
        public int order { set; get; }
        public string type { set; get; }
        public string icon { set; get; }
        public string style { set; get; }
        public string css { set; get; }
        public string title { set; get; }
        public string code { set; get; }
        public oPermistion permistion { set; get; }
    }

    class oGroup
    {
        public string code { set; get; }
        public string title { set; get; }
    }

    class oSetting
    {
        public bool active { set; get; }
        public string title { set; get; }
        public string screen { set; get; }
        public oGroup[] groups { set; get; }
        public oAction[] actions { set; get; }

        public oSetting() {
            this.actions = new oAction[] { };
            this.active = false;
            this.groups = new oGroup[] { };
            this.screen = string.Empty;
            this.title = string.Empty;
        }
    }

    class oWidget
    {
        public bool enable { set; get; }
        public string root { set; get; }
        public string scope { set; get; }
        public string name { set; get; }
        public string key { set; get; }
        public string title { set; get; }
        public string error { set; get; }
        public string[] files { set; get; }
        public oSetting setting { set; get; }
        public oWidget()
        {
            this.enable = false;
            this.root = string.Empty;
            this.scope = string.Empty;
            this.name = string.Empty;
            this.key = string.Empty;
            this.title = string.Empty;
            this.error = string.Empty;
            this.files = new string[] { };
        }

        public override string ToString()
        {
            return string.Format("{0}_{1}_{2}", root, scope, name);
        }
    }

    #endregion

    class Program
    {
        static void Main(string[] args)
        {
            write_fileListJson("widget");
            write_fileListJson("kit");
        }

        static void write_fileListJson(string dirName = "widget")
        {
            List<oWidget> list = new List<oWidget>();
            if (Directory.Exists(dirName))
            {
                string[] dirs = Directory.GetDirectories(dirName);
                for (var i = 0; i < dirs.Length; i++)
                {
                    string path_dir = dirs[i];
                    string scope = Path.GetFileName(path_dir);

                    string[] apps = Directory.GetDirectories(path_dir);
                    foreach (var path_app in apps)
                    {
                        oWidget o = new oWidget();
                        o.root = dirName;
                        o.scope = scope;
                        o.name = Path.GetFileName(path_app);
                        o.enable = true;
                        o.setting = new oSetting() { };

                        string fileSetting = string.Format("{0}/{1}/{2}/setting.json", o.root, o.scope, o.name);
                        if (File.Exists(fileSetting))
                        {
                            string textSetting = File.ReadAllText(fileSetting);
                            oSetting st = null;
                            try
                            {
                                st = Newtonsoft.Json.JsonConvert.DeserializeObject<oSetting>(textSetting);
                                o.setting = st;
                            }
                            catch (Exception ex)
                            {
                                o.error = string.Format("{0}/{1}/{2}", o.root, o.scope, o.name) + " is not valid, because convert JSON into file setting.json occur error: " + ex.Message;
                                o.enable = false;
                            }
                        }

                        if (o.scope.Contains("_"))
                        {
                            o.error = string.Format("{0}/{1}/{2}", o.root, o.scope, o.name) + " is not valid, because [" + o.scope + "] has character [_]";
                            o.enable = false;
                        }

                        if (o.name.Contains("_"))
                        {
                            o.error = string.Format("{0}/{1}/{2}", o.root, o.scope, o.name) + " is not valid, because [" + o.name + "] has character [_]";
                            o.enable = false;
                        }

                        o.key = string.Format("{0}_{1}_{2}", o.root, o.scope, o.name);

                        var fs = Directory.GetFiles(path_app);
                        List<string> lfs = new List<string>();
                        foreach (var fi in fs) lfs.Add(Path.GetFileName(fi));
                        o.files = lfs.ToArray();

                        list.Add(o);
                    }
                }
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(list, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(dirName + "/list.json", json);
        }
    }
}
