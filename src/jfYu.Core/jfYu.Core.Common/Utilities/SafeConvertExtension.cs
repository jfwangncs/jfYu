using System;

namespace jfYu.Core.Common.Utilities
{
    public static class SafeConvertExtension
    {
        #region Int

        public static int? SafeToIntNullable(this object value, int? defaultValue = null)
        {
            if (value == null)
                return defaultValue;

            int i;
            return int.TryParse(value.ToString(), out i) ? i : defaultValue;
        }

        public static int SafeToInt(this object value, int defaultValue = 0)
        {
            if (value == null)
                return defaultValue;

            int i;
            return int.TryParse(value.ToString(), out i) ? i : defaultValue;
        }

        #endregion

        #region Long

        public static long? SafeToLongNullable(this object value, long? defaultValue = null)
        {
            if (value == null)
                return defaultValue;
            long i;
            return long.TryParse(value.ToString(), out i) ? i : defaultValue;
        }

        public static long SafeToLong(this object value, long defaultValue = 0)
        {
            if (value == null)
                return defaultValue;
            long i;
            return long.TryParse(value.ToString(), out i) ? i : defaultValue;
        }

        #endregion

        #region String

        public static string SafeToString(this object value, string defaultValue = "")
        {
            return value == null ? defaultValue : value.ToString();
        }

        #endregion

        #region Float

        public static float SafeToFloat(this object value, float defaultValue = 0.0f)
        {
            if (value == null)
                return defaultValue;

            float i;
            return float.TryParse(value.ToString(), out i) ? i : defaultValue;
        }

        #endregion

        #region Double

        public static double SafeToDouble(this object value, double defaultValue = 0.0d)
        {
            if (value == null)
                return defaultValue;

            double i;
            return double.TryParse(value.ToString(), out i) ? i : defaultValue;
        }

        /// <summary>
        /// 转换字符串的对应数值
        /// </summary>
        /// <param name="value">需要转换的字符串</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>对应数值</returns>
        public static double SafeToDouble(this string value, double defaultValue = double.MinValue)
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            double result;
            return double.TryParse(value, out result) ? result : defaultValue;
        }

        #endregion

        #region Decimal

        public static decimal SafeToDecimal(this object value, decimal defaultValue = 0.0m)
        {
            if (value == null)
                return defaultValue;

            decimal i;
            return decimal.TryParse(value.ToString(), out i) ? i : defaultValue;
        }

        #endregion

        #region Guid

        public static Guid SafeToGuid(this object value)
        {
            return Guid.TryParse(value.ToString(), out Guid i) ? i : default(Guid);
        }

        /// <summary>
        /// 将目标字符串转换为Guid, 转换失败返回默认值
        /// </summary>
        /// <param name="value">需要转换的字符串</param>
        /// <param name="defaultValue">转换失败的默认值</param>
        /// <returns></returns>
        public static Guid SafeToGuid(this string value, Guid defaultValue = default(Guid))
        {
            return Guid.TryParse(value, out Guid result) ? result : defaultValue;
        }

        #endregion

        #region Bool
        public static bool SafeToBoolean(this object value, bool defaultValue = false)
        {
            if (value == null)
                return defaultValue;

            if (value.ToString() == "1" || value.ToString() == "0")
            {
                return value.ToString() != "0";
            }

            return bool.TryParse(value.ToString(), out bool b) ? b : defaultValue;
        }
        #endregion

        #region Datetime
        /// <summary>
        /// 将目标字符串转换为DateTime, 转换失败返回默认值
        /// </summary>
        /// <param name="value">需要转换的DateTime值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static DateTime SafeToDateTime(this string value, DateTime defaultValue = default(DateTime))
        {
            return DateTime.TryParse(value, out DateTime result) ? result : defaultValue;
        }

        /// <summary>
        /// 多租赁的 object 转ToDateTime
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defualtValue"></param>
        /// <returns></returns>
        public static DateTime SafeToDateTime(this object value, DateTime defaultValue = default(DateTime))
        {
            if (value == null)
                return defaultValue;

            DateTime i;
            if (DateTime.TryParse(value.ToString(), out i))
                return i;
            else
                return defaultValue;
        } 
        #endregion
    }
}
