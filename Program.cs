using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeploySQL.Models;
using DeploySQL.Tools;

namespace DeploySQL
{
    internal static class Program
    {
        public static string WelcomeText =
            "\r\n=======任務開始=========\r\n" +
            "專案名稱 : {0} \r\n" +
            "應用程式資料夾位置 : {1} \r\n" +
            "log位置 : {2}\r\n" +
            "DBScript位置 : {3}\r\n" +
            "db連線資訊 : {4} \r\n";

        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        /// <param name="args">todo: describe args parameter on Main</param>
        [STAThread]
        private static int Main(string[] args)
        {
            var res = 0;
            try
            {
                Configs.load();
                LogHelper.FilePath = Application.StartupPath.Trim('\\') + "\\" + Configs.cfg.LogPath + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";

                var ProjectName = args[0].Trim();
                var CurrentDirectory = Application.StartupPath.Trim('\\') + "\\";
                var ProjectDirectory = CurrentDirectory + ProjectName.Trim('\\') + "\\";
                var cfg = Configs.cfg.Project.First(x => x.Name.ToLower() == args[0].ToLower().Trim());

                var DatabaseDirectory = CurrentDirectory + cfg.ScriptPath.Trim('\\') + "\\";
                LogHelper.doLog(
                    string.Format(WelcomeText, ProjectName,
                                    CurrentDirectory, LogHelper.FilePath,
                                    DatabaseDirectory, cfg.ConnectString
                                    )
                    );

                var Strategys = new List<string>();
                if (args.Length > 1)
                    Strategys.AddRange(args[1].Trim().Split(','));
                else
                    Strategys = cfg.Strategy;
                Strategys = Strategys.ConvertAll(d => d.ToLower());

                var AllRunDowns = Configs.cfg.ActPlans.Where(x => Strategys.Contains(x.Name.ToLower())).ToList().SelectMany(x => x.RunDowns).ToList();

                if (cfg.ClearAllSp)
                {
                    using (var scopeOldData = new TimeMeasureScope("****{0}清除所有SP{1}*****"))
                        RemoveHelper.removeAllSp(cfg.ConnectString);

                    LogHelper.doLog("");
                }

                if (cfg.ClearAllTable)
                {
                    using (var scopeOldData = new TimeMeasureScope("****{0}清除所有Table{1}*****"))
                        RemoveHelper.removeAllTable(cfg.ConnectString);

                    LogHelper.doLog("");
                }

                LogHelper.doLog("****執行項目(" + AllRunDowns.Count() + "項)*****");
                LogHelper.doLog(string.Join("\r\n", AllRunDowns.ToArray()));

                var ignoreFile = new List<string>();
                if (null != cfg.ignoreFile && cfg.ignoreFile.Count > 0)
                {
                    ignoreFile.AddRange(cfg.ignoreFile);
                    LogHelper.doLog("****忽略檔案(" + ignoreFile.Count() + "項)*****");
                    LogHelper.doLog(string.Join("\r\n", ignoreFile.ToArray()));

                    LogHelper.doLog("");
                }

                var RootPath = cfg.ScriptPath;
                long ok = 0;
                long err = 0;
                long pass = 0;

                foreach (var subDir in AllRunDowns)
                {
                    var ExecDir = RootPath.TrimEnd('\\') + "\\" + subDir + "\\";
                    LogHelper.doLog("");
                    using (var scopeOldData = new TimeMeasureScope("****{0}執行[" + subDir + "]{1}*****"))
                    {
                        if (!System.IO.Directory.Exists(ExecDir))
                        {
                            LogHelper.doLog("查無資料夾");
                            continue;
                        }

                        var RunItem = System.IO.Directory.GetFiles(ExecDir, "*.sql");
                        if (RunItem.Count() <= 0)
                        {
                            LogHelper.doLog("未發現任何.sql檔案");
                            continue;
                        }
                        var totalFileCount = RunItem.Count();
                        var ExecutedFileCount = 1;

                        var watch = new Stopwatch();
                        var OutputMsg = "({0})({1}ms)({2}/{3}){4}";
                        foreach (var dir in RunItem)
                        {
                            watch.Restart();
                            var fname = System.IO.Path.GetFileName(dir).ToLower();
                            try
                            {
                                if (ignoreFile.Contains(fname))
                                {
                                    LogHelper.doLog(string.Format(OutputMsg, "Pass", watch.ElapsedMilliseconds, ExecutedFileCount, totalFileCount, fname));
                                    pass++;
                                    continue;
                                }

                                var script = System.IO.File.ReadAllText(dir);
                                DbHelper.executeSqlFile(cfg.ConnectString, script);
                                LogHelper.doLog(string.Format(OutputMsg, "Done", watch.ElapsedMilliseconds, ExecutedFileCount, totalFileCount, fname));
                                ok++;
                            }
                            catch (Exception e)
                            {
                                LogHelper.doLog(string.Format(OutputMsg, "Error", watch.ElapsedMilliseconds, ExecutedFileCount, totalFileCount, fname));
                                LogHelper.doLog(e.StackTrace);
                                err++;
                            }
                            finally
                            {
                                ExecutedFileCount++;
                            }
                        }
                    }
                }
                LogHelper.doLog("");
                LogHelper.doLog("任務完成，執行結果： 完成 :" + ok + " , 錯誤 :" + err + ", 略過 :" + pass);

                if (!Configs.cfg.AutoCloseConsole)
                {
                    LogHelper.doLog("==請按任意鍵離開==");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                LogHelper.doLog("Error : " + ex.Message + "\r\n" + ex.StackTrace);
                res = 99;
            }

            return res;
        }
    }
}