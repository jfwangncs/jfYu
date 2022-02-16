using Autofac;
using System.Linq;
using System.Reflection;

namespace jfYu.Core.Data
{
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// IOC注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddDataService(this ContainerBuilder services)
        {
            //获取执行方法的方法Assembly
            var EntryAssembly = Assembly.GetEntryAssembly();
            var ServiceKeyType = typeof(IServiceKey);

            services.RegisterAssemblyTypes(ServiceKeyType.Assembly).PropertiesAutowired();
            //获取所有Service并使用属性注入       
            services.RegisterAssemblyTypes(EntryAssembly.GetReferencedAssemblies().Select(q => Assembly.Load(q)).ToArray())
                .Where(t => t.GetInterfaces().Contains(ServiceKeyType))
                .AsImplementedInterfaces()
                .PropertiesAutowired();
            services.RegisterAssemblyTypes(EntryAssembly)
              .Where(t => t.GetInterfaces().Contains(ServiceKeyType))
              .AsImplementedInterfaces()
              .PropertiesAutowired();          
        }
    }
}
