using jfYu.Core.Common.SnowFlake;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace jfYu.Core.Data
{
    public class Service<T, Q> : IService<T> where T : BaseEntity
        where Q : DbContext
    {

        protected Expression<Func<T, bool>> exprTrue = q => true;

        public Q Master { get; }

        public Q Slave { get; }

        public List<Q> Slaves { get; }
        public ISnowFlake SnowFlake { get; set; }
        public Service(IDbContextService<Q> contextService)
        {

            Master = contextService.Master;
            Slave = contextService.Slave;
            Slaves = contextService.Slaves;
        }
        public virtual bool Add(T entity)
        {
            entity.Id = SnowFlake.NextId();
            Master.Add(entity);
            return Master.SaveChanges() > 0;
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            entity.Id = SnowFlake.NextId();
            await Master.AddAsync(entity);
            return (await Master.SaveChangesAsync()) > 0;
        }

        public virtual bool AddRange(List<T> list)
        {
            list.ForEach(q => q.Id = SnowFlake.NextId());
            Master.AddRange(list);
            return Master.SaveChanges() > 0;
        }

        public virtual async Task<bool> AddRangeAsync(List<T> list)
        {
            list.ForEach(q => q.Id = SnowFlake.NextId());
            await Master.AddRangeAsync(list);
            return (await Master.SaveChangesAsync()) > 0;
        }

        public virtual bool Update(T entity)
        {
            entity.UpdateTime = DateTime.Now;
            Master.Update(entity);
            return Master.SaveChanges() > 0;
        }
        public virtual async Task<bool> UpdateAsync(T entity)
        {
            entity.UpdateTime = DateTime.Now;
            Master.Update(entity);
            return (await Master.SaveChangesAsync()) > 0;
        }

        public virtual bool Update(Expression<Func<T, bool>> predicate = null, Action<T> scalar = null)
        {
            predicate ??= exprTrue;
            if (scalar == null)
                return false;
            var data = Slave.Set<T>().Where(predicate).ToList();
            data.ForEach(scalar);
            return Master.SaveChanges() > 0;
        }

        public virtual async Task<bool> UpdateAsync(Expression<Func<T, bool>> predicate = null, Action<T> scalar = null)
        {
            predicate ??= exprTrue;
            if (scalar == null)
                return false;
            var data = Slave.Set<T>().Where(predicate).ToList();
            data.ForEach(scalar);
            return (await Master.SaveChangesAsync()) > 0;
        }
        public virtual bool UpdateRange(List<T> list)
        {
            list.ForEach(q =>
            {
                q.UpdateTime = DateTime.Now;
            });
            Master.UpdateRange(list);
            return Master.SaveChanges() > 0;
        }

        public virtual async Task<bool> UpdateRangeAsync(List<T> list)
        {
            list.ForEach(q =>
            {
                q.UpdateTime = DateTime.Now;
            });
            Master.UpdateRange(list);
            return (await Master.SaveChangesAsync()) > 0;
        }

        #region 删除
        public virtual bool Remove(long id)
        {
            if (IsExist(id))
            {
                var entity = GetById(id);
                entity.UpdateTime = DateTime.Now;
                entity.State = StateEnum.Disable;
                return Master.SaveChanges() > 0;
            }
            return false;
        }
        public virtual bool Remove(string id)
        {
            if (IsExist(id))
            {
                var entity = GetById(id);
                entity.UpdateTime = DateTime.Now;
                entity.State = StateEnum.Disable;
                return Master.SaveChanges() > 0;
            }
            return false;
        }
        public virtual bool Remove(Expression<Func<T, bool>> predicate = null)
        {
            var lists = GetList(predicate);
            foreach (var entity in lists)
            {
                entity.UpdateTime = DateTime.Now;
                entity.State = StateEnum.Disable;
            }
            return Master.SaveChanges() > 0;
        }
        public virtual async Task<bool> RemoveAsync(long id)
        {
            if (await IsExistAsync(id))
            {
                var entity = await GetByIdAsync(id);
                entity.UpdateTime = DateTime.Now;
                entity.State = StateEnum.Disable;
                return (await Master.SaveChangesAsync()) > 0;
            }
            return false;
        }
        public virtual async Task<bool> RemoveAsync(string id)
        {
            if (await IsExistAsync(id))
            {
                var entity = await GetByIdAsync(id);
                entity.UpdateTime = DateTime.Now;
                entity.State = StateEnum.Disable;
                return (await Master.SaveChangesAsync()) > 0;
            }
            return false;
        }
        public virtual async Task<bool> RemoveAsync(Expression<Func<T, bool>> predicate = null)
        {
            var lists = await GetListAsync(predicate);
            foreach (var entity in lists)
            {
                entity.UpdateTime = DateTime.Now;
                entity.State = StateEnum.Disable;
            }
            return (await Master.SaveChangesAsync()) > 0;
        }

        public virtual bool HardRemove(long id)
        {
            if (IsExist(id))
            {
                var entity = GetById(id);
                Master.Remove(entity);
                return Master.SaveChanges() > 0;
            }
            return false;
        }
        public virtual bool HardRemove(string id)
        {
            if (IsExist(id))
            {
                var entity = GetById(id);
                Master.Remove(entity);
                return Master.SaveChanges() > 0;
            }
            return false;
        }

        public virtual async Task<bool> HardRemoveAsync(long id)
        {
            if (await IsExistAsync(id))
            {
                var entity = await GetByIdAsync(id);
                Master.Remove(entity);
                return (await Master.SaveChangesAsync()) > 0;
            }
            return false;
        }
        public virtual async Task<bool> HardRemoveAsync(string id)
        {
            if (await IsExistAsync(id))
            {
                var entity = await GetByIdAsync(id);
                Master.Remove(entity);
                return (await Master.SaveChangesAsync()) > 0;
            }
            return false;
        }
        #endregion

        #region 是否有数据
        public virtual bool IsExist(long id)
        {
            return Slave.Set<T>().Any(q => q.Id.Equals(id));
        }
        public virtual bool IsExist(string id)
        {
            try
            {
                var _id = long.Parse(id);
                return Slave.Set<T>().Any(q => q.Id.Equals(id));
            }
            catch (Exception)
            {
                return false;
            }

        }

        public virtual bool IsExist(Expression<Func<T, bool>> predicate = null)
        {
            return Slave.Set<T>().Any(predicate);
        }
        public virtual async Task<bool> IsExistAsync(long id)
        {
            return await Slave.Set<T>().AnyAsync(q => q.Id.Equals(id));
        }
        public virtual async Task<bool> IsExistAsync(string id)
        {
            try
            {
                var _id = long.Parse(id);
                return await Slave.Set<T>().AnyAsync(q => q.Id.Equals(id));
            }
            catch (Exception)
            {
                return false;
            }
        }
        public virtual async Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await Slave.Set<T>().AnyAsync(predicate);
        }

        #endregion

        #region 根据id获取实体
        public virtual T GetById(long id)
        {
            return Slave.Set<T>().FirstOrDefault(q => q.Id.Equals(id));
        }
        public virtual T GetById(string id)
        {
            try
            {
                var _id = long.Parse(id);
                return Slave.Set<T>().FirstOrDefault(q => q.Id.Equals(_id));
            }
            catch (Exception)
            {

                return null;
            }

        }
        public virtual async Task<T> GetByIdAsync(long id)
        {
            return await Slave.Set<T>().FirstOrDefaultAsync(q => q.Id.Equals(id));
        }
        public virtual async Task<T> GetByIdAsync(string id)
        {
            try
            {
                var _id = long.Parse(id);
                return await Slave.Set<T>().FirstOrDefaultAsync(q => q.Id.Equals(_id));
            }
            catch (Exception)
            {

                return null;
            }
        }
        #endregion

        #region 获取列表
        public virtual IQueryable<T> GetList(Expression<Func<T, bool>> predicate = null)
        {
            predicate ??= exprTrue;
            return Slave.Set<T>().Where(predicate).AsQueryable();
        }
        public virtual async Task<IQueryable<T>> GetListAsync(Expression<Func<T, bool>> predicate = null)
        {
            predicate ??= exprTrue;
            return await Task.Run(() => Slave.Set<T>().Where(predicate).AsQueryable());
        }
        public virtual IQueryable<T1> GetList<T1>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, T1>> scalar = null)
        {
            predicate ??= exprTrue;
            if (scalar == null)
                return new List<T1>().AsQueryable();
            return Slave.Set<T>().Where(predicate).Select(scalar).AsQueryable();
        }
        public virtual async Task<IQueryable<T1>> GetListAsync<T1>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, T1>> scalar = null)
        {
            predicate ??= exprTrue;
            if (scalar == null)
                return new List<T1>().AsQueryable();
            return await Task.Run(() => Slave.Set<T>().Where(predicate).Select(scalar).AsQueryable());
        }
        #endregion

    }
}
