using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace jfYu.Core.Common.Utilities
{
    public class EnvironmentHelper
    {
        /// <summary>
        /// 获取系统环境变量
        /// </summary>
        /// <param name="value">文本</param>
        /// <returns></returns>
        public static string GetEnvironmentVariable(string value)
        {
            //例如："${Server_IP}|127.0.0.1" 
            var result = value;
            //获取变量如Server_IP
            var param = GetParameters(result).FirstOrDefault();
            if (!string.IsNullOrEmpty(param))
            {
                //如没有取到变量值，则使用默认值127.0.0.1
                var env = Environment.GetEnvironmentVariable(param);
                result = env;
                if (string.IsNullOrEmpty(env))
                {
                    var arrayData = value.ToString().Split('|');
                    result = arrayData.Length == 2 ? arrayData[1] : env;
                }
            }
            return result;
        }
        /// <summary>
        /// 获取系统环境为true or false
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue">文本</param>
        /// <returns></returns>
        public static bool GetEnvironmentVariableAsBool(string name, bool defaultValue = false)
        {
            var str = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrEmpty(str))
            {
                return defaultValue;
            }

            switch (str.ToLowerInvariant())
            {
                case "true":
                case "1":
                case "yes":
                    return true;
                case "false":
                case "0":
                case "no":
                    return false;
                default:
                    return defaultValue;
            }
        }
        /// <summary>
        /// 获取文本里面的参数
        /// </summary>
        /// <param name="text">文本例如："${Server_IP}|127.0.0.1"</param>
        /// <returns></returns>
        private static List<string> GetParameters(string text)
        {
            var matchVale = new List<string>();
            string Reg = @"(?<=\${)[^\${}]*(?=})";
            foreach (Match m in Regex.Matches(text, Reg))
            {
                matchVale.Add(m.Value);
            }
            return matchVale;
        }
    }
}
