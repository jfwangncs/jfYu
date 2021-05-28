using jfYu.Core.Common.Configurations;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace jfYu.Core.MongoDB
{
    public class MongoDBService
    {
        private MongoClientSettings config;
        private IMongoDatabase db;
        public MongoDBService()
        {
            try
            {
                var MongoDBConfig = AppConfig.Configuration.GetSection("MongoDB").Get<MongoDBConfig>();
                if (string.IsNullOrEmpty(MongoDBConfig.MongoUrl) || string.IsNullOrEmpty(MongoDBConfig.DbName))
                {
                    throw new Exception("未找到mongodb相关配置数据");
                }
                else
                {
                    config = MongoClientSettings.FromUrl(MongoUrl.Create(MongoDBConfig.MongoUrl));
                    //最大连接池
                    config.MaxConnectionPoolSize = MongoDBConfig.MaxConnectionPoolSize;
                    //最大闲置时间
                    config.MaxConnectionIdleTime = TimeSpan.FromSeconds(MongoDBConfig.MaxConnectionIdleTime);
                    //最大存活时间
                    config.MaxConnectionLifeTime = TimeSpan.FromSeconds(MongoDBConfig.MaxConnectionLifeTime);
                    //链接时间
                    config.ConnectTimeout = TimeSpan.FromSeconds(MongoDBConfig.ConnectTimeout);
                    //等待队列大小
                    config.WaitQueueSize = MongoDBConfig.WaitQueueSize;
                    //socket超时时间
                    config.SocketTimeout = TimeSpan.FromSeconds(MongoDBConfig.SocketTimeout);
                    //队列等待时间
                    config.WaitQueueTimeout = TimeSpan.FromSeconds(MongoDBConfig.WaitQueueTimeout);
                    var client = new MongoClient(config);
                    db = client.GetDatabase(MongoDBConfig.DbName);
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"错误的mongodb配置数据:{ex.Message}-{ex.StackTrace}", ex);
            }
        }

        #region method
        /// <summary>
        /// 插入指定对象到数据库中
        /// </summary>
        /// <param name="entity">指定的对象</param>
        public T Insert<T>(T entity) where T : MongoEntity
        {

            var collection = db.GetCollection<T>(typeof(T).Name);
            var flag = ObjectId.GenerateNewId();
            entity.GetType().GetProperty("Id").SetValue(entity, flag);
            entity.State = 1;
            entity.CreateTime = DateTime.Now;
            entity.UpdateTime = DateTime.Now;
            collection.InsertOne(entity);
            return QueryOne<T>(q => q.Id == flag);
        }
        public async Task InsertAsync<T>(T entity) where T : MongoEntity
        {

            var collection = db.GetCollection<T>(typeof(T).Name);
            var flag = ObjectId.GenerateNewId();
            entity.GetType().GetProperty("Id").SetValue(entity, flag);
            entity.State = 1;
            entity.CreateTime = DateTime.Now;
            entity.UpdateTime = DateTime.Now;
            await collection.InsertOneAsync(entity);
        }
        public void InsertBatch<T>(IEnumerable<T> list) where T : MongoEntity
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            list.ToList().ForEach(entity =>
            {
                entity.GetType().GetProperty("Id").SetValue(entity, ObjectId.GenerateNewId());
                entity.State = 1;
                entity.CreateTime = DateTime.Now;
                entity.UpdateTime = DateTime.Now;

            });
            collection.InsertMany(list);
        }
        public async Task InsertBatchAsync<T>(IEnumerable<T> list) where T : MongoEntity
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            list.ToList().ForEach(entity =>
            {
                entity.GetType().GetProperty("Id").SetValue(entity, ObjectId.GenerateNewId());
                entity.State = 1;
                entity.CreateTime = DateTime.Now;
                entity.UpdateTime = DateTime.Now;

            });
            await collection.InsertManyAsync(list);
        }
        public void Modify<T>(string id, string field, string value) where T : MongoEntity
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            ObjectId.TryParse(id, out ObjectId Id);
            var filter = Builders<T>.Filter.Eq("Id", Id);
            var updated = Builders<T>.Update.Set(field, value).Set("UpdateTime", DateTime.Now);
            collection.UpdateOne(filter, updated);

        }
        public async void ModifyAsync<T>(string id, string field, string value) where T : MongoEntity
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            ObjectId.TryParse(id, out ObjectId Id);
            var filter = Builders<T>.Filter.Eq("Id", Id);
            var updated = Builders<T>.Update.Set(field, value).Set("UpdateTime", DateTime.Now);
            await collection.UpdateOneAsync(filter, updated);
        }
        public bool Update<T>(T entity) where T : MongoEntity
        {
            bool result = false;
            var collection = db.GetCollection<T>(typeof(T).Name);
            if (entity != null)
            {
                entity.UpdateTime = DateTime.Now;
                var update = collection.ReplaceOne(s => s.Id == entity.Id, entity, new UpdateOptions() { IsUpsert = true });
                result = update != null && update.ModifiedCount > 0;
            }
            return result;
        }
        public async Task<bool> UpdateAsync<T>(T entity) where T : MongoEntity
        {

            bool result = false;
            var collection = db.GetCollection<T>(typeof(T).Name);
            if (entity != null)
            {
                entity.UpdateTime = DateTime.Now;
                var update = await collection.ReplaceOneAsync(s => s.Id == entity.Id, entity, new UpdateOptions() { IsUpsert = true });
                result = update != null && update.ModifiedCount > 0;
            }
            return result;
        }

        public bool SoftDelete<T>(string id) where T : MongoEntity
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            var result = collection.Find(q => q.Id == ObjectId.Parse(id)).SingleOrDefault();
            if (result == null)
                return false;
            result.State = 0;
            return Update(result);
        }
        public async Task<bool> SoftDeleteAsync<T>(string id) where T : MongoEntity
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            var result =await collection.Find(q => q.Id == ObjectId.Parse(id)).SingleOrDefaultAsync();
            if (result == null)
                return false;
            result.State = 0;
            return await UpdateAsync(result);
        }

        public bool Delete<T>(string id) where T : MongoEntity
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            var result = collection.DeleteOne(q => q.Id == ObjectId.Parse(id));
            return result != null && result.DeletedCount > 0;
        }
        public async Task<bool> DeleteAsync<T>(string id) where T : MongoEntity
        {
            var collection = db.GetCollection<T>(typeof(T).Name);
            var result = await collection.DeleteOneAsync(q => q.Id == ObjectId.Parse(id));
            return result != null && result.DeletedCount > 0;
        }
        public T QueryOne<T>(Expression<Func<T, bool>> criteria) where T : MongoEntity
        {
            criteria ??= q => true;
            var collection = db.GetCollection<T>(typeof(T).Name);
            return collection.Find(criteria).ToList().FirstOrDefault();
        }
        public async Task<T> QueryOneAsync<T>(Expression<Func<T, bool>> criteria) where T : MongoEntity
        {
            criteria ??= q => true;
            var collection = db.GetCollection<T>(typeof(T).Name);
            return await collection.Find(criteria).FirstOrDefaultAsync();
        }
        public IQueryable<T> QueryCollection<T>(Expression<Func<T, bool>> criteria = null) where T : MongoEntity
        {
            criteria ??= q => true;
            return db.GetCollection<T>(typeof(T).Name).AsQueryable().Where(criteria).AsQueryable();
        }
        public async Task<IQueryable<T>> QueryCollectionAsync<T>(Expression<Func<T, bool>> criteria = null) where T : MongoEntity
        {
            criteria ??= q => true;
            return await Task.Run(() => db.GetCollection<T>(typeof(T).Name).AsQueryable().Where(criteria).AsQueryable());
        }

        /// <summary>  
        /// 获取当前本地时间戳  
        /// </summary>  
        /// <returns></returns>        
        private int GetCurrentTimeUnix()
        {

            TimeSpan cha = DateTime.Now - TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 8, 0, 0), TimeZoneInfo.Local);
            int t = (int)cha.TotalSeconds;
            return t;
        }
        #endregion
    }
}
