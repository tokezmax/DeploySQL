using System;
using System.IO;
using System.Text;
using System.Threading;

namespace DeploySQL.Tools
{
    public sealed class LogHelper
    {
        public static Mutex LogLock = new Mutex();
        public static Mutex BackLogLock = new Mutex();
        public static bool PrintToConsole = true;
        public static string FilePath = "";

        static LogHelper()
        {
        }

        /// <summary>
        /// 寫入log
        /// </summary>
        /// <param name="Forder">分類名稱</param>
        /// <param name="Message"></param>
        public static void doLog(string Message)
        {
            if (PrintToConsole)
                Console.WriteLine(Message);

            FileWrite(FilePath, Message);
        }

        public static void FileWrite(string FilePath, string Message, bool timestemp = false)
        {
            try
            {
                LogLock.WaitOne();

                if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                }

                if (!File.Exists(FilePath))
                {
                    using (StreamWriter f = File.CreateText(FilePath))
                    {
                        f.Flush();
                        f.Close();
                    }
                }
                using (StreamWriter sw = new StreamWriter(FilePath, true))
                {
                    if (timestemp)
                        Message = DateTime.Now.ToString("HH:mm:ss fff") + " : " + Message;

                    sw.WriteLine(Message);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch
            {
            }
            finally
            {
                LogLock.ReleaseMutex();
            }
        }
    }
}