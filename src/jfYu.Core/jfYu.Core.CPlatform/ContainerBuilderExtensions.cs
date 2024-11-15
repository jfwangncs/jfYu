using Autofac;
using System.Linq;
using System.Reflection;

namespace jfYu.Core.CPlatform
{
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// IOC注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddCPlatform(this ContainerBuilder services)
        {
            //获取执行方法的方法Assembly
            var EntryAssembly = Assembly.GetEntryAssembly();
            var BaseControllerType = typeof(IBaseController);         

            //注册controller
            services.RegisterAssemblyTypes(EntryAssembly)
                .Where(q => q.GetInterfaces().Contains(BaseControllerType))
                .PropertiesAutowired();
            services.RegisterAssemblyTypes(EntryAssembly.GetReferencedAssemblies().Select(q => Assembly.Load(q)).ToArray())
                .Where(t => t.GetInterfaces().Contains(BaseControllerType))
                .PropertiesAutowired();
        }
    }
}
