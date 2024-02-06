using System;
using System.Threading.Tasks;

namespace jfYu.Core.Cache
{
    public interface ICacheService
    {
        /// <summary>
        /// add
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expiry">expire,default:1 min.</param>  
        Task AddAsync<T>(string key, T value, TimeSpan? expiry = null);

        /// <summary>
        /// get
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="key">key</param>
        /// <returns>value</returns> 
        Task<string?> GetAsync(string key);

        /// <summary>
        /// get
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>string value</returns>
        Task<T?> GetAsync<T>(string key) where T : class;

        /// <summary>
        /// get
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>int value</returns> 
        Task<int?> GetIntAsync(string key);

        /// <summary>
        /// remove
        /// </summary>
        /// <param name="key">key</param> 
        Task RemoveAsync(string key);
    }
}