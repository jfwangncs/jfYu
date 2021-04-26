using Autofac;
using jfYu.Core.Common.Configurations;
using Microsoft.EntityFrameworkCore;
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
                Config = AppConfig.GetSection("ConnectionStrings").GetBindData<DatabaseConfiguration>();

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

        private static void RegisterService<T>(ContainerBuilder services,DatabaseConfiguration config) where T : DbContext
        {
            //注册MasterDBContext
            var masterOptBuilder = new DbContextOptionsBuilder<T>();
            if (config.DatabaseType.Equals(DatabaseType.SqlServer))
                masterOptBuilder.UseLazyLoadingProxies().UseSqlServer(config.MasterConnectionString);
            else if (config.DatabaseType.Equals(DatabaseType.Mysql))
                masterOptBuilder.UseLazyLoadingProxies().UseMySQL(config.MasterConnectionString);
            services.RegisterType<T>().AsSelf().InstancePerLifetimeScope().WithParameter("options", masterOptBuilder.Options).Named<T>("MasterContext");

            //注册SalveDBContext  
            int slaveCount = config.SlaveConnectionStrings.Count;
            if (slaveCount > 0)
            {
                for (int i = 0; i < slaveCount; i++)
                {
                    var SlaveOptBuilder = new DbContextOptionsBuilder<T>();
                    string SlaveConnectionString = config.SlaveConnectionStrings[i].ConnectionString;
                    if (config.DatabaseType.Equals(DatabaseType.SqlServer))
                        SlaveOptBuilder.UseLazyLoadingProxies().UseSqlServer(SlaveConnectionString);
                    else if (config.DatabaseType.Equals(DatabaseType.Mysql))
                        SlaveOptBuilder.UseLazyLoadingProxies().UseMySQL(SlaveConnectionString);
                    services.RegisterType<T>().AsSelf().InstancePerLifetimeScope().WithParameter("options", SlaveOptBuilder.Options).Named<T>($"SlaveContext{i + 1}");
                }
            }

            Random r = new Random();
            var mParaVal = new Func<ParameterInfo, IComponentContext, object>((p, c) => c.ResolveNamed<T>("MasterContext"));
            var salveContexts = new List<Func<ParameterInfo, IComponentContext, object>>();
            for (int j = 0; j < slaveCount; j++)
            {
                string slaveContextName = $"SlaveContext{j + 1}";
                salveContexts.Add(new Func<ParameterInfo, IComponentContext, object>((p, c) => c.ResolveNamed<T>(slaveContextName)));
            }

            services.RegisterType<DbContextService<T>>()
                .WithParameter((p, c) => p.Name == "MasterContext", mParaVal)
                .WithParameter((p, c) => p.Name == "SalveContext", slaveCount <= 0 ? mParaVal : salveContexts[r.Next(0, slaveCount)])
                .AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
