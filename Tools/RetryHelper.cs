using System;
using System.Collections.Generic;
using System.Threading;

namespace DeploySQL.Tools
{
    /// <summary>
    /// Retry 模組
    /// ===============================
    /// Example 1 :
    ///    RetryHelper.Do(() => SomeFunctionThatCanFail(), TimeSpan.FromSeconds(1));
    /// Example 2 :
    ///    RetryHelper.Do(SomeFunctionThatCanFail, TimeSpan.FromSeconds(1));
    /// Example 3 :
    ///    int result = RetryHelper.Do(SomeFunctionWhichReturnsInt, TimeSpan.FromSeconds(1), 4);
    /// </summary>
    public static class RetryHelper
    {
        /// <summary>
        /// 無需回傳參數
        /// </summary>
        /// <param name="action">方法(無需回傳結果)</param>
        /// <param name="retryInterval">重試等候時間</param>
        /// <param name="maxAttemptCount">重試次數</param>
        public static void DoAction(Action action, TimeSpan retryInterval, int maxAttemptCount = 3)
        {
            DoFunc<object>(() =>
            {
                if (action != null)
                    action();
                return null;
            }, retryInterval, maxAttemptCount);
        }

        /// <summary>
        /// 要回傳參數
        /// </summary>
        /// <param name="action">方法(需要回傳結果)</param>
        /// <param name="retryInterval">重試等候時間</param>
        /// <param name="maxAttemptCount">重試次數</param>
        public static T DoFunc<T>(Func<T> action, TimeSpan retryInterval, int maxAttemptCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int attempted = 1; attempted <= maxAttemptCount; attempted++)
            {
                try
                {
                    if (attempted > 0)
                        Thread.Sleep(retryInterval);

                    if (action != null)
                        return action();
                }
                catch (Exception ex)
                {
                    LogHelper.doLog("發生錯誤，重新執行" + attempted + "/" + maxAttemptCount);
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}