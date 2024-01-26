using jfYu.Core.Office.Excel;
using Microsoft.Extensions.DependencyInjection;

namespace jfYu.Core.Office
{
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// IOC注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddJfYuExcel(this IServiceCollection services)
        {
            services.AddTransient<IJfYuExcel, JfYuExcel>();
        }
    }
}
