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
        /// <param name="seconds">expire seconds,default:60s</param> 
        Task AddAsync<T>(string key, T value, int seconds = 60);

        /// <summary>
        /// add
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expiration">expiretime,default:now add 1 min.</param>  
        Task AddAsync<T>(string key, T value, DateTime? expiration);

        /// <summary>
        /// get
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="key">key</param>
        /// <returns>value</returns> 
        Task<string> GetAsync(string key);

        /// <summary>
        /// get
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>string value</returns>
        Task<T> GetAsync<T>(string key) where T : class;

        /// <summary>
        /// get
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>int value</returns> 
        Task<int> GetIntAsync(string key);

        /// <summary>
        /// remove all 
        /// </summary> 
        Task RemoveAllAsync();

        /// <summary>
        /// remove
        /// </summary>
        /// <param name="key">key</param> 
        Task RemoveAsync(string key);
    }
}