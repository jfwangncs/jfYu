using jfYu.Core.Office.Excel;
using jfYu.Core.Office.Word;
using Microsoft.Extensions.DependencyInjection;

namespace jfYu.Core.Office
{
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Ioc
        /// </summary>
        /// <param name="services"></param>
        public static void AddJfYuExcel(this IServiceCollection services)
        {
            services.AddTransient<IJfYuExcel, JfYuExcel>();
        }

        // <summary>
        /// Ioc
        /// </summary>
        /// <param name="services"></param>
        public static void AddJfYuWord(this IServiceCollection services)
        {
            services.AddTransient<IJfYuWord, JfYuWord>();
        }
    }
}
