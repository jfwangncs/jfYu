using System;
using System.IO;
using System.Threading;

namespace jfYu.Core.Common.Utilities
{
    public class WriteFileLog
    {
        readonly ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();
        public void WriteLog(string strLog, string filename = "")
        {
            string LogsDir = AppContext.BaseDirectory + "logs\\";
            if (!Directory.Exists(LogsDir))//验证路径是否存在
            {
                Directory.CreateDirectory(LogsDir);
                //不存在则创建
            }
            string sFileName = $"{LogsDir}\\{DateTime.Now:yyyy-MM-dd}{filename}.log";
            try
            {
                LogWriteLock.EnterWriteLock();
                if (!strLog.StartsWith("error"))
                    File.AppendAllText(sFileName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "   ---   " + strLog + "\r\n");
            }
            catch (Exception ex)
            {
                throw new Exception("写入出现异常", ex);
            }
            finally
            {
                LogWriteLock.ExitWriteLock();
            }
        }
    }
}
