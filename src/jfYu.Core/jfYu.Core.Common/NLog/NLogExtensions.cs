using NLog;
using System;

namespace jfYu.Core.Common.NLog
{
    public static class NlLogExtensions
    {

        public static void AddLog(this ILogger logger, LogLevel logLevel, string logType, string message)
        {

            LogEventInfo ei = new LogEventInfo(logLevel, logType, message);
            logger.Log(ei);
        }
        public static void AddLog(this ILogger logger, LogLevel logLevel, string logType, string message, string content)
        {

            LogEventInfo ei = new LogEventInfo(logLevel, logType, message);
            ei.Properties["content"] = content;
            logger.Log(ei);
        }
        public static void AddLog(this ILogger logger, LogLevel logLevel, string logType, string message, string content, string uri, string ip)
        {

            LogEventInfo ei = new LogEventInfo(logLevel, logType, message);
            ei.Properties["content"] = content;
            ei.Properties["url"] = uri;
            ei.Properties["ip"] = ip;
            logger.Log(ei);
        }

        public static void AddUserLog(this ILogger logger, LogLevel logLevel, string logType, string message, string userId, string userName)
        {

            LogEventInfo ei = new LogEventInfo(logLevel, logType, message);
            ei.Properties["userid"] = userId;
            ei.Properties["username"] = userName;
            logger.Log(ei);
        }
  
        public static void AddUserLog(this ILogger logger, LogLevel logLevel, string logType, string message, string content, string userId, string userName)
        {

            LogEventInfo ei = new LogEventInfo(logLevel, logType, message);
            ei.Properties["content"] = content;
            ei.Properties["userid"] = userId;
            ei.Properties["username"] = userName;
            logger.Log(ei);
        }
      
        public static void AddUserLog(this ILogger logger, LogLevel logLevel, string logType, string message, string content, string url, string ip, string userId, string userName)
        {

            LogEventInfo ei = new LogEventInfo(logLevel, logType, message);
            ei.Properties["content"] = content;
            ei.Properties["url"] = url;
            ei.Properties["ip"] = ip;
            ei.Properties["userid"] = userId;
            ei.Properties["username"] = userName;
            logger.Log(ei);
        }

    }
}
