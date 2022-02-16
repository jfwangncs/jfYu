using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace jfYu.Core.Common.Utilities
{
    public static class ModelMap
    {
        public static List<T> ToModels<T>(this DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(ToModel<T>(r));
            }
            return list;

        }

        public static T ToModel<T>(this DataRow row) where T : new()
        {
            T item = new T();
            // go through each column
            foreach (DataColumn c in row.Table.Columns)
            {
                // find the property for the column
                PropertyInfo p = item.GetType().GetProperty(c.ColumnName);
                try
                { // if exists, set the value
                    if (p != null && row[c] != DBNull.Value)
                    {
                        p.SetValue(item, row[c], null);
                    }
                }
                catch (ArgumentException)
                {
                    throw new Exception($"{p.Name}格式和数据库不一致,数据库格式{c.DataType.Name}，Model格式{p.PropertyType.Name}");
                }
                catch (Exception ex)
                {
                    throw new Exception("转换异常", ex);
                }
            }
            return item;

        }
    }
}
