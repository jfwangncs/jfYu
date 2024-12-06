using jfYu.Core.Redis.Extensions;
using jfYu.Core.Redis.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jfYu.Core.Redis.Implementation
{
    public partial class RedisService : IRedisService
    {
        public async Task<bool> SetAddAsync<T>(string key, T value, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNull(value);
            Log( nameof(SetAddAsync), key);
            var entryBytes = _serializer.Serialize(value);
            return await _database.SetAddAsync(key, entryBytes, flag);
        }

        public async Task<long> SetAddAllAsync<T>(string key, List<T> values, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            values.ThrowListIfNullOrEmpty();
            Log( nameof(SetAddAllAsync), key);
            return await _database.SetAddAsync(key, values
                    .Select(item => Serializer.Serialize(item))
                    .Select(x => (RedisValue)x)
                    .ToArray(), flag);
        }

        public async Task<bool> SetRemoveAsync<T>(string key, T value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNull(value);
            Log( nameof(SetRemoveAsync), key);
            var entryBytes = _serializer.Serialize(value);
            return await _database.SetRemoveAsync(key, entryBytes);
        }

        public async Task<long> SetRemoveAllAsync<T>(string key, List<T> values, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            values.ThrowListIfNullOrEmpty();
            values.ForEach(value => ArgumentNullException.ThrowIfNull(value));
            Log( nameof(SetRemoveAllAsync), key);
            return await _database.SetRemoveAsync(key, values
                   .Select(item => Serializer.Serialize(item))
                   .Select(x => (RedisValue)x)
                   .ToArray(), flag);
        }

        public async Task<bool> SetContainsAsync<T>(string key, T value, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNull(value);
            Log( nameof(SetContainsAsync), key);
            var entryBytes = _serializer.Serialize(value);
            return await _database.SetContainsAsync(key, entryBytes);
        }

        public async Task<List<RedisValue>> SetMembersAsync(string key, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            Log( nameof(SetMembersAsync), key);
            return [.. await _database.SetMembersAsync(key, flag)];
        }

        public async Task<long> SetLengthAsync(string key, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            Log( nameof(SetLengthAsync), key);
            return await _database.SetLengthAsync(key, flag);
        }

        public async Task<RedisValue> SetRandomMemberAsync(string key, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            Log( nameof(SetRandomMemberAsync), key);
            return await _database.SetRandomMemberAsync(key, flag);
        }

        public async Task<List<RedisValue>> SetRandomMembersAsync(string key, int count = 1, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            Log( nameof(SetRandomMembersAsync), key);
            return [.. await _database.SetRandomMembersAsync(key, count, flag)];
        }
    }
}
