﻿using jfYu.Core.Redis.Extensions;
using jfYu.Core.Redis.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jfYu.Core.Redis.Implementation
{
    /// <summary>
    /// The implementation of the Redis service.
    /// </summary>
    public partial class RedisService : IRedisService
    {
        /// <inheritdoc/>
        public async Task<bool> SortedSetAddAsync<T>(string key, T value, double score, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNull(value);
            Log(nameof(SortedSetAddAsync), key);
            return await _database.SortedSetAddAsync(key, _serializer.Serialize(value), score, when, flag).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<long> SortedSetAddAllAsync<T>(string key, Dictionary<T, double> values, CommandFlags flag = CommandFlags.None) where T : notnull
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            if (values == null || values.Count <= 0)
                throw new ArgumentNullException(nameof(values));
            Log(nameof(SortedSetAddAllAsync), key);
            return await _database.SortedSetAddAsync(key, values.Select(x => new SortedSetEntry(_serializer.Serialize(x.Key), x.Value)).ToArray(), flag).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<long> SortedSetRemoveAsync<T>(string key, List<T> values, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            values.ThrowListIfNullOrEmpty();
            Log(nameof(SortedSetRemoveAsync), key);
            return await _database.SortedSetRemoveAsync(key, values.Select(x => (RedisValue)_serializer.Serialize(x)).ToArray(), flag).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<double> SortedSetIncrementScoreAsync<T>(string key, T value, double increment, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNull(value);
            Log(nameof(SortedSetIncrementScoreAsync), key);
            return await _database.SortedSetIncrementAsync(key, _serializer.Serialize(value), increment, flag).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<long?> SortedSetRankAsync<T>(string key, T value, Order order = Order.Ascending, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNull(value);
            Log(nameof(SortedSetRankAsync), key);
            return await _database.SortedSetRankAsync(key, _serializer.Serialize(value), order, flag).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<List<RedisValue>> SortedSetRangeByRankAsync(string key, long start, long stop, Order order = Order.Ascending, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            Log(nameof(SortedSetRangeByRankAsync), key);
            return [.. await _database.SortedSetRangeByRankAsync(key, start, stop, order, flag).ConfigureAwait(false)];
        }

        /// <inheritdoc/>
        public async Task<List<RedisValue>> SortedSetRangeByScoreAsync(string key, double min, double max, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            Log(nameof(SortedSetRangeByScoreAsync), key);
            return [.. await _database.SortedSetRangeByScoreAsync(key, min, max, exclude, order, skip, take, flag).ConfigureAwait(false)];
        }

        /// <inheritdoc/>
        public async Task<long> SortedSetCountAsync(string key, double min, double max, Exclude exclude = Exclude.None, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            Log(nameof(SortedSetCountAsync), key);
            return await _database.SortedSetLengthAsync(key, min, max, exclude, flag).ConfigureAwait(false);
        }
    }
}