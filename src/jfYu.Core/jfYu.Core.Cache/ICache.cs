using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jfYu.Core.Cache
{
    public interface ICache
    {
        public CacheType CacheType { get; }

        #region 判断缓存

        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>是否成功</returns>
        bool Has(string key);

        /// <summary>
        /// 验证缓存项是否存在（异步方式）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>是否成功</returns>
        Task<bool> HasAsync(string key);

        #endregion

        #region 添加缓存
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="IsNx">不存在该key添加缓存，否则失败，默认不开启</param>
        /// <returns>是否成功</returns>
        bool Add(string key, object value, bool IsNx = false);


        /// <summary>
        /// 添加缓存（异步方式）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="IsNx">不存在该key添加缓存，否则失败，默认不开启</param>
        /// <returns>是否成功</returns>
        Task<bool> AddAsync(string key, object value, bool IsNx = false);



        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiration">过期时间</param>
        /// <param name="IsNx">不存在该key添加缓存，否则失败，默认不开启</param>
        /// <returns>是否成功</returns>
        bool Add(string key, object value, TimeSpan expiration, bool IsNx = false);


        /// <summary>
        /// 添加缓存（异步方式）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiration">过期时间</param>
        /// <param name="IsNx">不存在该key添加缓存，否则失败，默认不开启</param>
        /// <returns>是否成功</returns>
        Task<bool> AddAsync(string key, object value, TimeSpan expiration, bool IsNx = false);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="seconds">过期秒数</param>
        /// <param name="IsNx">不存在该key添加缓存，否则失败，默认不开启</param>
        /// <returns>是否成功</returns>
        bool Add(string key, object value, int seconds, bool IsNx = false);


        /// <summary>
        /// 添加缓存（异步方式）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="seconds">过期秒数</param>
        /// <param name="IsNx">不存在该key添加缓存，否则失败，默认不开启</param>
        /// <returns>是否成功</returns>
        Task<bool> AddAsync(string key, object value, int seconds, bool IsNx = false);

        #endregion

        #region 删除缓存

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>是否成功</returns>
        bool Remove(string key);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>是否成功</returns>
        Task<bool> RemoveAsync(string key);

        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="key">缓存Key集合</param>
        /// <returns>失败key集合</returns>
        IEnumerable<string> RemoveRange(IEnumerable<string> keys);

        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        /// <returns>失败key集合</returns>
        Task<IEnumerable<string>> RemoveRangeAsync(IEnumerable<string> keys);

        #endregion

        #region 获取缓存

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>是否成功</returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>是否成功</returns>
        Task<T> GetAsync<T>(string key) where T : class;

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>是否成功</returns>
        object Get(string key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>是否成功</returns>
        Task<object> GetAsync(string key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>是否成功</returns>
        string GetString(string key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>是否成功</returns>
        Task<string> GetStringAsync(string key);


        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>是否成功</returns>
        int GetInt(string key);


        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns>是否成功</returns>
        Task<int> GetIntAsync(string key);

        /// <summary>
        /// 获取缓存集合
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        /// <returns>是否成功</returns>
        IDictionary<string, object> GetRange(IEnumerable<string> keys);

        /// <summary>
        /// 获取缓存集合
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        /// <returns>是否成功</returns>
        Task<IDictionary<string, object>> GetRangeAsync(IEnumerable<string> keys);

        #endregion

        #region 修改缓存

        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <returns>是否成功</returns>
        bool Replace(string key, object value);


        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <returns>是否成功</returns>
        Task<bool> ReplaceAsync(string key, object value);

        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <param name="expiration">过期时间</param>
        /// <returns>是否成功</returns>
        bool Replace(string key, object value, TimeSpan expiration);


        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <param name="expiration">过期时间</param>
        /// <returns>是否成功</returns>
        Task<bool> ReplaceAsync(string key, object value, TimeSpan expiration);


        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <param name="seconds">过期秒数</param>
        /// <returns>是否成功</returns>
        bool Replace(string key, object value, int seconds);


        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <param name="seconds">过期秒数</param>
        /// <returns>是否成功</returns>
        Task<bool> ReplaceAsync(string key, object value, int seconds);
        #endregion
    }
}
