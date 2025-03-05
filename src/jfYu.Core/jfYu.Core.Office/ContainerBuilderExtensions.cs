using jfYu.Core.Office.Excel;
using jfYu.Core.Office.Excel.Write.Implementation;
using jfYu.Core.Office.Excel.Write.Interface;
using jfYu.Core.Office.Word;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Data.Common;

namespace jfYu.Core.Office
{
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Injection
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddJfYuExcel(this IServiceCollection services, Action<JfYuExcelOption>? setupAction = null)
        {
            var options = new JfYuExcelOption(); 
            services.Configure<JfYuExcelOption>(opts =>
            { 
                opts.RowAccessSize = options.RowAccessSize;
                opts.SheetMaxRecord = options.SheetMaxRecord;                 
                setupAction?.Invoke(opts);
            });
            services.AddScoped<IJfYuExcel, JfYuExcel>();
            services.AddScoped<IJfYuExcelWriterFactory, JfYuExcelWriterFactory>();
            services.AddScoped<IJfYuExcelWrite<DataTable>, DataTableWriter>();
            services.AddScoped<IJfYuExcelWrite<DbDataReader>, DbDataReaderWriter>();
            services.AddScoped(typeof(IJfYuExcelWrite<>), typeof(ListWriter<>));
            return services;
        }

        /// <summary>
        /// Injection
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddJfYuWord(this IServiceCollection services)
        {
            services.AddScoped<IJfYuWord, JfYuWord>();
            return services;
        }
    }
}
