using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace jfYu.Core.Data
{
    public static class EntityFrameworkCoreExtension
    {
        public static IQueryable<T> SqlQuery<T>(this DatabaseFacade facade, string sql, params object[] parameters) where T : class, new()
        {
            var conn = facade.GetDbConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddRange(parameters);
            var reader = cmd.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            conn.Close();
            return dt.ToModels<T>().AsQueryable();
        }
        public static IQueryable<T> SqlQueryInterpolated<T>(this DatabaseFacade facade, FormattableString sql) where T : class,new()
        {


            var conn = facade.GetDbConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            var parameters = new List<SqlParameter>();
            var arguments = new List<string>();
            for (int i = 0; i < sql.ArgumentCount; i++)
            {
                parameters.Add(new SqlParameter($"@arg{i}", sql.GetArgument(i)));
                arguments.Add($"@arg{i}");
            };
            cmd.CommandText = string.Format(sql.Format, arguments.ToArray());
            cmd.Parameters.AddRange(parameters.ToArray());
            var reader = cmd.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            conn.Close();
            return dt.ToModels<T>().AsQueryable();
        }

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
