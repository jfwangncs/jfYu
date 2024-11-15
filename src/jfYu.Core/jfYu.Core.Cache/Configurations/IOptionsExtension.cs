using Microsoft.Extensions.DependencyInjection;

namespace jfYu.Core.Cache.Configurations
{
    /// <summary>
    ///options extension.
    /// </summary>
    public interface IOptionsExtension
    {
        /// <summary>
        /// Adds the services.
        /// </summary>
        /// <param name="services">Services.</param>
        void AddServices(IServiceCollection services);
    }
}
