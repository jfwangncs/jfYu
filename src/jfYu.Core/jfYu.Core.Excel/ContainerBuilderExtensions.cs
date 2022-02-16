using Autofac;

namespace jfYu.Core.Excel
{
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// IOC注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddJfYuExcel(this ContainerBuilder services)
        {
            services.Register(q=>new JfYuExcel()).As<JfYuExcel>().SingleInstance();
        }
    }
}
