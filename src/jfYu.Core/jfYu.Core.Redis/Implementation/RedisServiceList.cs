﻿using jfYu.Core.Redis.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jfYu.Core.Redis.Implementation
{
    public partial class RedisService : IRedisService
    {
        public async Task<long> ListAddAsync<T>(string key, T value, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNull(value);
            Log( nameof(ListAddAsync), key);
            var entryBytes = _serializer.Serialize(value);
            return await _database.ListRightPushAsync(key, entryBytes, when, flag);
        }

        public async Task<long> ListAddToLeftAsync<T>(string key, T value, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNull(value);            
            Log(nameof(ListAddToLeftAsync), key);
            var entryBytes = _serializer.Serialize(value);
            return await _database.ListLeftPushAsync(key, entryBytes, when, flag);
        }

        public async Task<T?> ListPopFromRightAsync<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            Log(nameof(ListPopFromRightAsync), key);
            var valueBytes = await _database.ListRightPopAsync(key, flag);
            return !valueBytes.HasValue ? default : Serializer.Deserialize<T>(valueBytes!);
        }

        public async Task<T?> ListPopFromLeftAsync<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            Log(nameof(ListPopFromLeftAsync), key);
            var valueBytes = await _database.ListLeftPopAsync(key, flag);
            return !valueBytes.HasValue ? default : Serializer.Deserialize<T>(valueBytes!);
        }

        public async Task<long> ListLengthAsync(string key, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            Log(nameof(ListLengthAsync), key);
            return await _database.ListLengthAsync(key, flag);
        }

        public async Task<long> ListRemoveAsync(string key, string value, int count)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            Log(nameof(ListRemoveAsync), key);
            var entryBytes = _serializer.Serialize(value);
            return await _database.ListRemoveAsync(key, entryBytes, count);

        }

        public async Task<List<RedisValue>> ListGetRangeAsync(string key, int start = 0, int stop = -1, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            Log(nameof(ListGetRangeAsync), key);
            return [.. (await _database.ListRangeAsync(key, start, stop, flag))];
        }       
    }
}
