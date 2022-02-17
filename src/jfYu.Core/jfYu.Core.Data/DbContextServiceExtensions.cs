using Autofac;
using jfYu.Core.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace jfYu.Core.Data
{
    public static class DbContextServiceExtensions
    {

        /// <summary>
        /// IOC注册
        /// </summary>  
        public static void AddDbContextService<T>(this ContainerBuilder services) where T : DbContext
        {
            // 主从数据库配置
            DatabaseConfiguration Config = new DatabaseConfiguration();
            try
            {
                Config = AppConfig.Configuration.GetSection("ConnectionStrings").Get<DatabaseConfiguration>();

            }
            catch (Exception ex)
            {
                throw new Exception($"连接字符串配置错误:{ex.Message} - {ex.StackTrace}");
            }

            try
            {
                RegisterService<T>(services, Config);
            }
            catch (Exception ex)
            {
                throw new Exception($"配置错误无法实例化主从连接:{ex.Message} - {ex.StackTrace}");
            }



        }

        /// <summary>
        /// IOC注册
        /// </summary>   
        public static void AddDbContextService<T>(this ContainerBuilder services, DatabaseConfiguration databaseConfiguration) where T : DbContext
        {
            try
            {
                RegisterService<T>(services, databaseConfiguration);
            }
            catch (Exception ex)
            {
                throw new Exception($"配置错误无法实例化主从连接:{ex.Message} - {ex.StackTrace}");
            }
        }

        private static void RegisterService<T>(ContainerBuilder services, DatabaseConfiguration config) where T : DbContext
        {
            if (config == null)
                throw new Exception($"读取配置为空。");
            //注册MasterDBContext
            var masterOptBuilder = new DbContextOptionsBuilder<T>();
            if (config.DatabaseType.Equals(DatabaseType.SqlServer))
                masterOptBuilder.UseSqlServer(config.MasterConnectionString);
            else if (config.DatabaseType.Equals(DatabaseType.Mysql))
                masterOptBuilder.UseMySql(ServerVersion.AutoDetect(config.MasterConnectionString));
            services.RegisterType<T>().AsSelf().InstancePerDependency().WithParameter("options", masterOptBuilder.Options).Named<T>("MasterContext");

            //注册SalveDBContext  
            int slaveCount = config.SlaveConnectionStrings.Count;
            if (slaveCount > 0)
            {
                for (int i = 0; i < slaveCount; i++)
                {
                    var SlaveOptBuilder = new DbContextOptionsBuilder<T>();
                    string SlaveConnectionString = config.SlaveConnectionStrings[i].ConnectionString;
                    if (config.SlaveConnectionStrings[i].DatabaseType.Equals(DatabaseType.SqlServer))
                        SlaveOptBuilder.UseSqlServer(SlaveConnectionString);
                    else if (config.SlaveConnectionStrings[i].DatabaseType.Equals(DatabaseType.Mysql))
                        SlaveOptBuilder.UseMySql(ServerVersion.AutoDetect(config.MasterConnectionString));
                    services.RegisterType<T>().AsSelf().InstancePerDependency().WithParameter("options", SlaveOptBuilder.Options).Named<T>($"SlaveContext{i + 1}");
                }
            }

            var master = new Func<ParameterInfo, IComponentContext, object>((p, c) => c.ResolveNamed<T>("MasterContext"));
            var salves = new Func<ParameterInfo, IComponentContext, object>((p, c) =>
             {
                 var salves = new List<T>();
                 for (int j = 0; j < slaveCount; j++)
                 {
                     string slaveContextName = $"SlaveContext{j + 1}";
                     salves.Add(c.ResolveNamed<T>(slaveContextName));
                 }
                 return salves;
             });

            services.RegisterType<DbContextService<T>>()
                .WithParameter((p, c) => p.Name == "master", master)
                .WithParameter((p, c) => p.Name == "salves", salves)
                .WithParameter("configuration", config)
                .AsImplementedInterfaces().InstancePerDependency();
        }

    }
}
