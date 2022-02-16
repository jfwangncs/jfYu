using jfYu.Core.Common.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
    }
}
