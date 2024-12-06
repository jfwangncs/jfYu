using System;

namespace jfYu.Core.Common.Utilities
{
    public class UnixTime
    {
        //北京时间
        private static readonly DateTime localTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 8, 0, 0), TimeZoneInfo.Local);

        /// <summary>  
        /// 获取当前本地时间戳  
        /// </summary>  
        /// <returns></returns>        
        public static int GetUnixTime()
        {
            TimeSpan cha = DateTime.Now - localTime;
            int t = (int)cha.TotalSeconds;
            return t;
        }
        /// <summary>
        ///获取时间戳
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetUnixTime(DateTime date)
        {
            TimeSpan cha = date - localTime;
            int t = (int)cha.TotalSeconds;
            return t;
        }


        /// <summary>  
        /// 获取当前本地时间戳  
        /// </summary>  
        /// <returns></returns>        
        public static long GetUnixTimeMillis()
        {
            TimeSpan cha = DateTime.Now - localTime;
            var t = (long)cha.TotalMilliseconds;
            return t;
        }
        /// <summary>
        ///获取时间戳
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long GetUnixTimeMillis(DateTime date)
        {
            TimeSpan cha = date - localTime;
            var t = (long)cha.TotalMilliseconds;
            return t;
        }
        /// <summary>  
        /// 时间戳转换为本地时间对象  
        /// </summary>  
        /// <returns></returns>        
        public static DateTime GetDateTime(int unixTime)
        {
            DateTime newTime = localTime.AddSeconds(unixTime);
            return newTime;
        }

    }
}