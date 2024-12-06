using jfYu.Core.Redis.Interface;
using static jfYu.Core.Test.Redis.RedisBaseTests;

namespace jfYu.Core.Test.Redis
{
    [Collection("Redis")]
    public class RedisLockTest(IRedisService redisService)
    {
        private readonly IRedisService _redisService = redisService;

        #region LockTakeAsync
        [Theory]
        [ClassData(typeof(NullKeyExpectData))]
        public async Task LockTakeAsync_WhenkeyIsNull_ShouldThrowArgumentException(string key)
        {
            var ex = await Record.ExceptionAsync(async () => await _redisService.LockTakeAsync(key));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task LockTakeAsync_WhenKeyNotExist_ReturnsTrue()
        {
            string key = "testLock";
            bool result = await _redisService.LockTakeAsync(key);
            Assert.True(result);
            await _redisService.LockReleaseAsync(key);
        }
        [Fact]
        public async Task LockTakeAsync_WhenKeyExist_ReturnsFalse()
        {
            string key = "testLock";
            await _redisService.LockTakeAsync(key);
            bool result = await _redisService.LockTakeAsync(key);
            Assert.False(result);
            await _redisService.LockReleaseAsync(key);
        }

        [Fact]
        public async Task LockTakeAsync_WhenKeyExpires_ReturnsTrue()
        {
            string key = "testLock";
            await _redisService.LockTakeAsync(key, TimeSpan.FromSeconds(3));
            bool result = await _redisService.LockTakeAsync(key);
            Assert.False(result);
            await Task.Delay(4000);
            result = await _redisService.LockTakeAsync(key);
            Assert.True(result);
            await _redisService.LockReleaseAsync(key);
        }
        #endregion

        #region LockReleaseAsync

        [Theory]
        [ClassData(typeof(NullKeyExpectData))]
        public async Task LockReleaseAsync_WhenkeyIsNull_ShouldThrowArgumentException(string key)
        {
            var ex = await Record.ExceptionAsync(async () => await _redisService.LockReleaseAsync(key));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }
        [Fact]
        public async Task LockReleaseAsync_WhenKeyNotExist_ReturnsFalse()
        {
            string key = "testLock";
            bool result = await _redisService.LockReleaseAsync(key);
            Assert.False(result);
        }

        [Fact]
        public async Task LockReleaseAsync_WhenKeyExist_ReturnsTrue()
        {
            string key = "testLock";
            await _redisService.LockTakeAsync(key);
            bool result = await _redisService.LockReleaseAsync(key);
            Assert.True(result);
        }
        #endregion

        #region HighConcurrency
        [Fact]
        public async Task HighConcurrency_LockCorrectly()
        {
            const string LockKey = "test:lock";
            const int NumberOfTasks = 100;
            const int LockTimeoutInSeconds = 5;
            const int IncrementCount = 1000;
            // Arrange 
            var taskResults = new Task[NumberOfTasks];
            int count = 0;
            var records = new List<int>();
            var tasks = Enumerable.Range(0, NumberOfTasks).Select(i => Task.Run(async () =>
            {
                var locked = false;
                do
                {
                    if (locked = await _redisService.LockTakeAsync(LockKey, TimeSpan.FromSeconds(LockTimeoutInSeconds)))
                    {
                        for (int i = 0; i < IncrementCount; i++)
                        {
                            count++;
                        }
                        await _redisService.RemoveAsync(LockKey);
                    }

                } while (!locked);

            })).ToList();

            await Task.WhenAll(tasks);
            // Assert
            Assert.True(count == NumberOfTasks * IncrementCount);
        }

        [Fact]
        public async Task HighConcurrency_ReleaseTwice_LockFaild()
        {
            const string LockKey = "test:lock";
            const int NumberOfTasks = 100;
            const int LockTimeoutInSeconds = 5;
            const int IncrementCount = 1000;
            // Arrange 
            var taskResults = new Task[NumberOfTasks];
            int count = 0;
            var records = new List<int>();
            var tasks = Enumerable.Range(0, NumberOfTasks).Select(i => Task.Run(async () =>
            {
                var locked = false;
                do
                {
                    if (locked = await _redisService.LockTakeAsync(LockKey, TimeSpan.FromSeconds(LockTimeoutInSeconds)))
                    {
                        await Task.Delay(500);
                        for (int i = 0; i < IncrementCount; i++)
                        {
                            count++;
                        }
                        await _redisService.RemoveAsync(LockKey);
                        await Task.Delay(500);
                        await _redisService.RemoveAsync(LockKey);
                    }

                } while (!locked);

            })).ToList();

            await Task.WhenAll(tasks);
            // Assert
            Assert.True(count < NumberOfTasks * IncrementCount);

        } 
        #endregion
    }
}
