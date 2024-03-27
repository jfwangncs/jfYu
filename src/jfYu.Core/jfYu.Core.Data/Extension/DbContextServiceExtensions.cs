using jfYu.Core.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Reflection;
namespace jfYu.Core.Data.Extension
{
    public static class DbContextServiceExtensions
    {       
        /// <summary>
        /// injection
        /// </summary>  
        public static void AddJfYuDbContextService<T>(this IServiceCollection services, JfYuDBConfig configuration) where T : DbContext, IJfYuDbContextService
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentException.ThrowIfNullOrEmpty(configuration.ConnectionString);

            try
            {
                services.AddDbContext<T>((q, masterOptBuilder) =>
                {
                    switch (configuration.DatabaseType)
                    {
                        case DatabaseType.Mysql:
                            masterOptBuilder.UseMySql(configuration.ConnectionString, ServerVersion.AutoDetect(configuration.ConnectionString)).EnableDetailedErrors();
                            break;
                        case DatabaseType.Sqlite:
                            masterOptBuilder.UseSqlite(configuration.ConnectionString).EnableDetailedErrors();
                            break;
                        default:
                        case DatabaseType.SqlServer:
                            masterOptBuilder.UseSqlServer(configuration.ConnectionString).EnableDetailedErrors();
                            break;
                    }
                });


                if (configuration.ReadOnlyConfigs != null && configuration.ReadOnlyConfigs.Count > 0)
                {
                    services.AddScoped<IContextRead, T>(services =>
                    {
                        int index = new Random().Next(configuration.ReadOnlyConfigs.Count);
                        var readonlyConfig = configuration.ReadOnlyConfigs[index];
                        var optionsBuilder = new DbContextOptionsBuilder<T>();
                        switch (readonlyConfig.DatabaseType)
                        {
                            case DatabaseType.Mysql:
                                optionsBuilder.UseMySql(readonlyConfig.ConnectionString, ServerVersion.AutoDetect(readonlyConfig.ConnectionString)).EnableDetailedErrors();
                                break;
                            case DatabaseType.Sqlite:
                                optionsBuilder.UseSqlite(readonlyConfig.ConnectionString).EnableDetailedErrors();
                                break;
                            default:
                            case DatabaseType.SqlServer:
                                optionsBuilder.UseSqlServer(readonlyConfig.ConnectionString).EnableDetailedErrors();
                                break;
                        }
                        var dbContextType = typeof(T);
                        var dbContext = Activator.CreateInstance(dbContextType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, [optionsBuilder.Options], CultureInfo.InvariantCulture);
                        return (T)(dbContext ?? throw new ArgumentNullException(nameof(T)));
                    });
                }
                else
                {
                    services.AddScoped<IContextWrite, T>(services =>
                    {                    
                        return services.GetService<T>() ?? throw new ArgumentNullException(nameof(T));
                    });
                    services.AddScoped<IContextRead, T>(services =>
                    {
                        return services.GetService<T>() ?? throw new ArgumentNullException(nameof(T));
                    });
                }

                services.AddScoped(typeof(ReadonlyDBContext<>));

                //register service
                services.AddScoped(typeof(IService<,>), typeof(Service<,>));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
