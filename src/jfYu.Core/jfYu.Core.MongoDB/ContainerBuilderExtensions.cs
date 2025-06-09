using Autofac;
using jfYu.Core.Configuration;


namespace jfYu.Core.MongoDB
{
    public static class ContainerBuilderExtensions
    {

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddMongoDB(this ContainerBuilder services)
        {
            services.Register(q => new MongoDBService()).As<MongoDBService>().InstancePerLifetimeScope();
        }                
    }
}
