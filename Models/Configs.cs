using DeploySQL.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploySQL.Models
{
    public class Configs
    {
        public static Configs cfg = new Configs();

        public static void load()
        {
            if (!System.IO.File.Exists("Z_Config.json"))
                throw new Exception("找不到參數設定檔[Z_Config.json]");
            try
            {
                lock (cfg)
                    cfg = JsonConvert.DeserializeObject<Configs>(File.ReadAllText("Z_Config.json"));
            }
            catch (Exception e)
            {
                LogHelper.doLog("載入參數設定檔錯誤" + e.Message + "\r\n" + e.StackTrace);
            }
        }

        public int DbRetryCount { get; set; }
        public bool AutoCloseConsole { get; set; }
        public string LogPath { get; set; }
        public List<Project> Project { get; set; }
        public List<Actplan> ActPlans { get; set; }
    }

    public class Project
    {
        public Project()
        {
            ClearAllSp = false;
            ClearAllTable = false;
        }

        public string Name { get; set; }
        public string ScriptPath { get; set; }
        public string ConnectString { get; set; }
        public List<string> Strategy { get; set; }

        public List<string> ignoreFile { get; set; }
        public bool ClearAllSp { get; set; }
        public bool ClearAllTable { get; set; }
    }

    public class Actplan
    {
        public string Name { get; set; }
        public string[] RunDowns { get; set; }
    }
}