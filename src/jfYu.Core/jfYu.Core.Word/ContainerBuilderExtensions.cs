using Autofac;

namespace jfYu.Core.Word
{
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// IOC注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddJfYuWord(this ContainerBuilder services)
        {
            services.RegisterType<jfYuWord>().As<jfYuWord>().AsImplementedInterfaces();
        }
    }
}
