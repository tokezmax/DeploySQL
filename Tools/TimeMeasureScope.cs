using System;
using System.Diagnostics;

namespace DeploySQL.Tools
{
    /// <inheritdoc />
    /// <summary>
    /// 執行時間測量範圍(自動使用Stopwatch計時並寫Log)
    /// </summary>
    public class TimeMeasureScope : IDisposable
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly string _title;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="title">範圍標題</param>
        public TimeMeasureScope(string title)
        {
            _title = title;
            LogHelper.doLog(string.Format(_title, "開始", ""));
            stopwatch.Start();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            stopwatch.Stop();
            LogHelper.doLog(string.Format(_title, "完成", "(花費:" + stopwatch.ElapsedMilliseconds + "ms)"));
        }
    }
}